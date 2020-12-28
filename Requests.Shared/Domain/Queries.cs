namespace Requests.Shared.Domain
{
    public enum Queries
    {
        Invalid,
        Ping,                       // see if server is up
        CurrentStatus,              // query current status of a request with ID
        HasStatus,                  // query if request has specific status with ID and Status    
        UnderConsideration,         // query total number of requests being processed, but not finished
        WithStatus,                 // query for all requests with a certain status
        Request,                    // query for current info on request
        History                     // query for all processing done for request with ID
    }
}
