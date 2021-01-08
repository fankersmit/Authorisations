using System;
using System.Collections.Generic;
using Requests.Shared.Domain;

namespace Authorisations.Infrastructure
{
    public class QueryBuilder
    {
        public Query BuildQueryFor(Queries queryType, Guid requestId)
        {
            var newQuery = new Query(queryType);
            newQuery.Add("ID", requestId.ToString());
            return newQuery;
        }

        public Query BuildQueryFor(Queries queryType, Dictionary<string, string> args )
        {
            var newQuery = new Query(queryType);
            foreach (var (keyName, valueName) in args)
            {
                newQuery.Add(keyName, valueName );
            }
            return newQuery;
        }

        public Query BuildQueryFor(Queries queryType, string additionalInfo = "None" )
        {
            Query newQuery;

            switch(queryType)
            {
                case  Queries.UnderConsideration:
                    newQuery = new Query(Queries.UnderConsideration);
                    newQuery.Add("Type", additionalInfo);
                    break;

                case Queries.Ping:
                    // additional info ignored
                    newQuery = new Query(Queries.Ping);
                    break;

                case Queries.WithStatus:
                    newQuery = new Query(Queries.WithStatus);
                    newQuery.Add("Status", additionalInfo);
                    break;

                case Queries.HasStatus:
                    newQuery = new Query(Queries.HasStatus);
                    newQuery.Add("Status", additionalInfo);
                    break;

                case Queries.History:
                case Queries.Invalid:
                case Queries.Request:
                case Queries.CurrentStatus:

                default:
                throw new NotImplementedException();
            }
            return newQuery;
        }
    }
}
