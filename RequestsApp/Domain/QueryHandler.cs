using System.Collections.Generic;
using System.Linq;
using Requests.Domain;
using Requests.Shared.Domain;
using RequestsApp.Infrastructure;

namespace RequestsApp.Domain
{
    public class QueryHandler : IQueryHandler
    {

        // used with under-consideration query
        private IList<Commands> commandsIncluded = new List<Commands>
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

        public int RequestsUnderConsideration(RequestType requestType)
        {
            return _context.RequestDocuments.Count(
                p => commandsIncluded.Contains(p.Command));
        }

        public int AllRequestsUnderConsideration()
        {
            return _context.RequestDocuments.Count(
                p => commandsIncluded.Contains(p.Command));
        }
    }
}