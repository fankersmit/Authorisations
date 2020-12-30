using System;
using System.Collections.Generic;
using System.Linq;
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

        // ctors
        public QueryHandler(RequestDbContext requestContext)
        {
            _context = requestContext;
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

        private byte[] GetRequestsWithStatus(Guid requestId, Dictionary<string, string> args, RequestStatus[] status)
        {
            var resultsDict = new Dictionary<string, string>();

            var selectedDocuments = _context.RequestDocuments
                .AsEnumerable()
                .Where( p => p.Request.Status == status[0])
                .Select(p => new
                    {
                        p.ID,
                        p.Version
                    })
                .ToList();

            var responseDict = resultsDict.Concat(args).ToDictionary(p => p.Key, p => p.Value);
            var count = 1;
            foreach (var item in selectedDocuments)
            {
                responseDict.Add( $"{count++:D3}", $"ID: {item.ID} version: {item.Version}" );
            }

            return JsonSerializer.SerializeToUtf8Bytes(responseDict);
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
                    Enum.TryParse<RequestStatus>(args["Status"], out status);
                }

                queryResult = runner( requestId, args, status );
            }
            catch (Exception e) when ( e is ArgumentException || e is FormatException )
            {
                queryResult = BuildFailureResponse(query, requestId.ToString(), e);
            }
            return queryResult;
        }

        private byte[] BuildHasStatusResponse(bool hasStatus, Dictionary<string, string> args)
        {
            var resultsDict = new Dictionary<string, string>
            {
                { "Query", "HasStatus" },
            };
            var responseDict = resultsDict.Concat(args).ToDictionary(p => p.Key, p => p.Value);
            responseDict.Add("Result", hasStatus ? "true" : "false");
            return JsonSerializer.SerializeToUtf8Bytes(responseDict);
        }

        private byte[] HasRequestStatus(  Guid requestID, Dictionary<string,string> args, params RequestStatus[] status  )
        {
            IList<RequestDocument>  selectedDocuments = GetRequestHistory(requestID);
            var json = JsonDocument.Parse(selectedDocuments.Last().SerializedRequest);
            var builder = new RequestFromJsonBuilder(null); // no logger
            var currentStatus = builder.GetRequest(json.RootElement).Status;
            bool hasStatus = (currentStatus == status[0]);
            return BuildHasStatusResponse(hasStatus, args );
        }

        private byte[] BuildUnderConsiderationResponse(string requestType, int count)
        {
            var resultsDict = new Dictionary<string, string>
            {
                { "Query", "UnderConsideration" },
                { "Type", requestType },
                { "Count", count.ToString()}
            };
            return JsonSerializer.SerializeToUtf8Bytes(resultsDict);
        }

        private byte[] BuildPingResult()
        {
            var resultsDict = new Dictionary<string, string>
            {
                { "Query", "Ping" },
                { "Store" , "Up" },
                { "RequestHandler", "Up"},
                { "Broker", "Up"}
            };
            return JsonSerializer.SerializeToUtf8Bytes(resultsDict);
        }

        private byte[] BuildCurrentStatusResponse( RequestStatus current, string requestID)
        {
            var resultsDict = new Dictionary<string, string>
            {
                { "Query", "CurrentStatus" },
                { "ID", requestID},
                { "Status", current.ToString("g")}
            };
            return JsonSerializer.SerializeToUtf8Bytes(resultsDict);
        }

        private byte[] BuildFailureResponse( Queries query, string requestID, Exception exception )
        {
            var resultsDict = new Dictionary<string, string>
            {
                {"Query", query.ToString()},
                {"ID", requestID},
                {"Failure",  $"{exception.Message}"}
            };
            return JsonSerializer.SerializeToUtf8Bytes(resultsDict);
        }

        private byte[] GetCurrentStatusForRequest( Guid requestID, Dictionary<string,string> args, params RequestStatus[] status  )
        {
            IList<RequestDocument> selectedDocuments = GetRequestHistory(requestID);

            var json = JsonDocument.Parse(selectedDocuments.Last().SerializedRequest);
            var builder = new RequestFromJsonBuilder(null);
            var request = builder.GetRequest(json.RootElement);
            return  BuildCurrentStatusResponse(request.Status, requestID.ToString());
        } 

        // TODO: implement querying for  organisation and product requests
        // argument is ignored for now, we return all
        private byte[] GetRequestsUnderConsideration( Guid requestID, Dictionary<string,string> args, params RequestStatus[] status  )
        {
            int count = _context.RequestDocuments.Count(p => _commandsIncluded.Contains(p.Command));
            return BuildUnderConsiderationResponse(args["Type"], count);
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
