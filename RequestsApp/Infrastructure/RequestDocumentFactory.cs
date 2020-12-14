using System;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Infrastructure
{
    public static class RequestDocumentFactory
    {
        public static RequestDocument Create(RequestBase request)
        {
            return new RequestDocument(request);
        }
        
        public static RequestDocument Create(RequestBase request, Commands command)
        {
            return new RequestDocument(request, command);
        }
    }
}