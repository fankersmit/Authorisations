using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Requests.Shared.Domain;
using RequestsApp.Infrastructure;

namespace RequestsApp.Domain
{

    internal delegate byte[] QueryRunner(Guid requestId, Dictionary<string, string> args, params RequestStatus[] status);

    public class QueryHandler : IQueryHandler, IDisposable
    {
        // fields, used with under-consideration query
        private readonly IList<Commands> _commandsIncluded = new List<Commands>
        {
            Commands.Submit,
            Commands.NoOp,
            Commands.Confirm,
            Commands.Cancel
        };
        
        private readonly RequestDbContext _context;
        private readonly ResponseBuilder _responseBuilder;

        // ctors
        public QueryHandler(RequestDbContext requestContext)
        {
            _context = requestContext;
            _responseBuilder = new ResponseBuilder();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public byte[] QueryFor( Queries query, Dictionary<string,string> args )
        {
            byte[] queryResult = null;

            switch (query)
            {
                    case Queries.Invalid:
                        break;
                    // see if server is up
                    case Queries.Ping:
                        queryResult = BuildPingResult();
                        break;  
                    // query current status of a request with ID
                    case Queries.CurrentStatus:
                        queryResult = BuildResult( GetCurrentStatusForRequest, query, args);
                        break;

                    // query if request has specific status with ID and Status  
                    case Queries.HasStatus:
                        queryResult = BuildResult(HasRequestStatus, query, args );
                        break;

                    // query total number of requests being processed, but not finished
                    case Queries.UnderConsideration:
                        queryResult = BuildResult(GetRequestsUnderConsideration, query, args);
                        break;

                    // query for all requests with a certain status
                    case Queries.WithStatus:
                        queryResult = BuildResult( GetRequestsWithStatus, query, args);
                        break;
                    
                    // query for current info on request 
                    case Queries.Request:
                        queryResult = BuildResult( GetLatestRequestInfo, query, args);
                        break;

                    // query for all processing done for request with ID
                    case Queries.History:
                        try
                        {
                            Guid requestId = Guid.Parse(args["ID"]);
                            GetRequestHistory(requestId);
                        }
                        catch (Exception e) when ( e is ArgumentException || e is FormatException )
                        {
                            queryResult = BuildFailureResponse(query, args["ID"], e);
                        }
                        break; 
            }
            return queryResult;
        }

        private byte[] GetLatestRequestInfo(Guid requestID, Dictionary<string, string> args, RequestStatus[] status)
        {
            Response response;
            try
            {
                var selectedDocuments = _context.RequestDocuments
                    .OrderByDescending(p => p.TimeStamp)
                    .Where( p => p.ID == requestID)
                    .ToList();
                var  selectedDocument =  selectedDocuments.First();

                response = _responseBuilder.Create(Queries.Request, args, selectedDocument.SerializedRequest);
            }
            catch (Exception e)
            {
                // request not found
                throw new ArgumentException($"Request with ID: {requestID} not found.");
            }
            return response.AsUTF8Bytes;
        }

        private byte[] GetRequestsWithStatus(Guid requestID, Dictionary<string, string> args, RequestStatus[] status)
        {
            var response = _responseBuilder.Create(Queries.WithStatus, args);

            var selectedDocuments = _context.RequestDocuments
                .AsEnumerable()
                .Where( p => p.SerializedRequest.Contains( status[0].ToString()))
                .Select(p => new
                    {
                        p.ID,
                        p.Version
                    })
                .ToList();

            var count = 1;
            foreach (var item in selectedDocuments)
            {
                response.Add( $"{count++:D3}", $"ID: {item.ID} version: {item.Version}" );
            }
            return response.AsUTF8Bytes;
        }

        // private helper methods
        private byte[] BuildResult( QueryRunner runner, Queries query, Dictionary<string,string> args)
        {
            byte[] queryResult = null;
            Guid requestId = Guid.Empty;
            var status = RequestStatus.New;

            try
            {
                if (args.ContainsKey("ID"))
                {
                    requestId = Guid.Parse(args["ID"]);
                }
                if (args.ContainsKey("Status"))
                {
                    Enum.TryParse(args["Status"], out status);
                }

                // here  the delegate is invoked
                queryResult = runner( requestId, args, status );
            }
            catch (Exception e) when ( e is ArgumentException || e is FormatException )
            {
                queryResult = BuildFailureResponse(query, requestId.ToString(), e);
            }
            return queryResult;
        }

        private byte[] HasRequestStatus(  Guid requestID, Dictionary<string,string> args, params RequestStatus[] status  )
        {
            IList<RequestDocument>  selectedDocuments = GetRequestHistory(requestID);
            var json = JsonDocument.Parse(selectedDocuments.Last().SerializedRequest);
            var builder = new RequestFromJsonBuilder(null); // no logger
            var currentStatus = builder.GetRequest(json.RootElement).Status;
            bool hasStatus = (currentStatus == status[0]);
            var responseDict = args;
            responseDict.Add("Result", hasStatus ? "true" : "false");
            var response = _responseBuilder.Create(Queries.HasStatus, responseDict);
            return response.AsUTF8Bytes;
        }

        private byte[] BuildPingResult()
        {
            var response = _responseBuilder.Create(Queries.Ping, new Dictionary<string, string>()
            {
                { "Store" , "Up" },
                { "RequestHandler", "Up"},
                { "Broker", "Up"}
            });
            return response.AsUTF8Bytes;
        }

        private byte[] BuildFailureResponse( Queries query, string requestID, Exception exception )
        {
            var response = _responseBuilder.Create(query, new Dictionary<string, string>
            {
                {"ID", requestID},
                {"Failure",  $"{exception.Message}"}
            });
            return response.AsUTF8Bytes;
        }

        private byte[] GetCurrentStatusForRequest( Guid requestID, Dictionary<string,string> args, params RequestStatus[] status  )
        {
            IList<RequestDocument> selectedDocuments = GetRequestHistory(requestID);

            var json = JsonDocument.Parse(selectedDocuments.Last().SerializedRequest);
            var builder = new RequestFromJsonBuilder(null);
            var request = builder.GetRequest(json.RootElement);
            var resultsDict = new Dictionary<string, string>() {
                { "ID", requestID.ToString() },
                { "Status", request.Status.ToString("g")}
            };
            var response = _responseBuilder.Create(Queries.CurrentStatus, resultsDict);
            return response.AsUTF8Bytes;
        }

        // TODO: implement querying for  organisation and product requests
        // argument is ignored for now, we return all
        private byte[] GetRequestsUnderConsideration(Guid requestID, Dictionary<string, string> args,
            params RequestStatus[] status)
        {
            int count = _context.RequestDocuments.Count(p => _commandsIncluded.Contains(p.Command));
            var response = _responseBuilder.Create(Queries.UnderConsideration);
            response.Add("Type", args["Type"]);
            response.Add("Count", count.ToString());
            return response.AsUTF8Bytes;
        }

        private IList<RequestDocument> GetRequestHistory(Guid requestID)
        {
            var selectedDocuments = _context.RequestDocuments
                .Where(p => p.ID == requestID)
                .OrderBy(p => p.TimeStamp)
                .ToList();

            if (!selectedDocuments.Any())
            {
                // request not found
                throw new ArgumentException($"Request with ID: {requestID} not found.");
            }

            return selectedDocuments;
        }
    }
}
