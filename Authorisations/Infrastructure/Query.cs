using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Requests.Shared.Domain;

namespace Authorisations.Infrastructure
{
    public class Query
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

        public byte[] AsUTF8Bytes => _queryEntries.SerializeToJson();

        // ctors
        private Query()
        {
            _queryEntries = new Dictionary<string, string>();
        }
        
        public Query( Queries type ) : this()
        {
            _queryEntries.Add("Query", type.ToString("g"));
        }

        // methods
        public int Add(string argumentName, string argumentValue)
        {
            _queryEntries.Add(argumentName, argumentValue);
            return _queryEntries.Count;
        }

        // helper methods
    }
}
