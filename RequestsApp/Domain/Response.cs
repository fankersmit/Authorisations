using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public class Response
    {
        // fields
        private readonly Dictionary<string, string> _queryEntries;

        // properties
        public Queries QueryType => Enum.Parse<Queries>(_queryEntries["Query"]);

        public Dictionary<string, string> Arguments {
            get
            {
                return _queryEntries
                    .Where(s => s.Key != "Query")
                    .ToDictionary(
                        d => d.Key, d => d.Value);
            }
        }

        public byte[] AsUTF8Bytes => JsonSerializer.SerializeToUtf8Bytes(_queryEntries);

        // ctors
        private Response()
        {
            _queryEntries = new Dictionary<string, string>();
        }

        public Response( Queries type ) : this()
        {
            _queryEntries.Add("Query", type.ToString("g"));
        }

        // methods
        public int Add(string argumentName, string argumentValue)
        {
            _queryEntries.Add(argumentName, argumentValue);
            return _queryEntries.Count;
        }

        public int Add(Dictionary<string, string> range )
        {
            foreach (var kv in range)
            {
                _queryEntries.Add(kv.Key, kv.Value);
            }
            return _queryEntries.Count;
        }

        // helper methods
    }
}
