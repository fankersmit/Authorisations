using System;
using System.Linq;
using Requests.Shared.Domain;

namespace Authorisations.Models
{
    public static class RequestChecker
    {
        public static bool IsKnownRequestType(string description)
        {
            if( description == null ) return false;
            var rt = Enum.GetNames(typeof(RequestType));
            rt = rt.Select(s => s.ToLowerInvariant()).ToArray();
            return rt.Contains(description.ToLower());
        }
    }
}