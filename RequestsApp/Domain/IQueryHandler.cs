using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public interface IQueryHandler
    {
        public int RequestsUnderConsideration(RequestType requestType);
        public int AllRequestsUnderConsideration();
    }
}