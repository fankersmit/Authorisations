using Requests.Shared.Domain;

namespace Requests.Domain
{
    public interface IQueryHandler
    {
        public int RequestsUnderConsideration(RequestType requestType);
        public int AllRequestsUnderConsideration();
    }
}