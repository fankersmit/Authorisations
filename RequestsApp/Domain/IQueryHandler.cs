using System.Collections.Generic;
using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public interface IQueryHandler
    {
        public byte[] QueryFor(Queries query, Dictionary<string, string> args);
    }
}
