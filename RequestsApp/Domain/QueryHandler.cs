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
    public class QueryHandler : IQueryHandler, IDisposable
    {
        // used with under-consideration query
        private readonly IList<Commands> _commandsIncluded = new List<Commands>
        {
            Commands.Submit,
            Commands.NoOp,
            Commands.Confirm,
            Commands.Cancel
        };
        
        private readonly RequestDbContext _context;

        public QueryHandler(RequestDbContext requestContext)
        {
            _context = requestContext;
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
                        Guid requestId = Guid.Parse(args["ID"]);
                        try
                        {
                            var currentStatus = GetCurrentStatusForRequest(requestId);
                            queryResult = BuildSuccessResponse(currentStatus, args["ID"]);
                        }
                        catch (ArgumentException e)
                        {
                            queryResult = BuildFailureResponse(query, args["ID"], e);
                        }
                        break;

                    // query if request has specific status with ID and Status  
                    case Queries.HasStatus:
                        break;   
                    // query total number of requests being processed, but not finished
                    case Queries.UnderConsideration:
                        var count  = GetRequestsUnderConsideration(args["Type"]);
                        queryResult = BuildUnderConsiderationResponse(args["Type"], count);
                        break;

                    // query for all requests with a certain status
                    case Queries.WithStatus:
                        break;         
                    // query for current info on request 
                    case Queries.Request:
                        break;   
                    // query for all processing done for request with ID
                    case Queries.History:
                        break; 
            }

            return queryResult;
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

        private byte[] BuildSuccessResponse( RequestStatus current, string requestID)
        {
            var resultsDict = new Dictionary<string, string>
            {
                {"ID", requestID},
                {"Status", current.ToString("g")}
            };
            return JsonSerializer.SerializeToUtf8Bytes(resultsDict);
        }

        private byte[]  BuildFailureResponse( Queries query, string requestID, ArgumentException exception )
        {
            var resultsDict = new Dictionary<string, string>
            {
                {"Query", query.ToString()},
                {"ID", requestID},
                {"Failure",  $"{exception.Message}"}
            };
            return JsonSerializer.SerializeToUtf8Bytes(resultsDict);
        }

        private RequestStatus GetCurrentStatusForRequest( Guid requestID )
        {
            IList<RequestDocument> selectedDocuments;
            selectedDocuments = _context.RequestDocuments
                    .Where(p => p.ID == requestID)
                    .OrderBy(p => p.TimeStamp)
                    .ToList();

            if (!selectedDocuments.Any())
            {
                // request not found?  Exception? 404?
                throw new ArgumentException($"Request with ID: {requestID} not found.");
            }

            var json = JsonDocument.Parse(selectedDocuments.Last().SerializedRequest);
            var builder = new RequestFromJsonBuilder(null);
            var request = builder.GetRequest(json.RootElement);
            return request.Status;
        } 

        // TODO: implement querying for  organisation and product requests
        // argument is ignored for now, we return all
        public int GetRequestsUnderConsideration( string requestType )
        {

            return  _context.RequestDocuments.Count(
                p => _commandsIncluded.Contains(p.Command));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }


}
