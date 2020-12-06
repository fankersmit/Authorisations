using System;
using System.Linq;

namespace Requests.Shared.Domain
{
    public static class RequestTypeChecker
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