using System.Collections.Generic;
using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public class ResponseBuilder
    {
        public Response Create(Queries type)
        {
            return new Response(type);
        }

        public Response Create(Queries type, string keyName, string IdValue)
        {
            var response = Create(type);
            response.Add(keyName, IdValue);
            return response;
        }

        public Response Create(Queries type, Dictionary<string, string> range )
        {
            var response = Create(type);
            response.Add(range);
            return response;
        }
    }
}
