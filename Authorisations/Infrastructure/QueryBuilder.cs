using System;
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

        public Query BuildQueryFor(Queries queryType, string additionalInfo )
        {
            Query newQuery;

            switch(queryType)
            {
                case  Queries.UnderConsideration:
                    newQuery = new Query(Queries.UnderConsideration);
                    newQuery.Add("Type", additionalInfo);
                    break;

                case Queries.Ping:
                    newQuery = new Query(Queries.Ping);
                    break;

                case Queries.History:
                case Queries.Invalid:
                case Queries.Request:
                case Queries.CurrentStatus:
                case Queries.HasStatus:
                case Queries.WithStatus:
                default:
                throw new NotImplementedException();
            }
            return newQuery;
        }
    }
}
