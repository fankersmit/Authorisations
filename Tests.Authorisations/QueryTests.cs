using System;
using FluentAssertions;
using Xunit;
using Authorisations.Infrastructure;
using Requests.Shared.Domain;

namespace Tests.Authorisations
{
    public class QueryTests
    {
        private QueryBuilder _queryBuilder = new QueryBuilder();

        [Theory]
        [InlineData(Queries.CurrentStatus)]
        public void NewQueryAlwaysHasQueryTypeInfo(Queries type)
        {
            // arrange
            var query = new Query(type);
            // act
            var actualType = query.QueryType;
            // assert
            actualType.Should().BeEquivalentTo(type);
            query.Arguments.Count.Should().Be(0);
        }

        [Fact]
        public void CanSerializeQueryToUtf8Bytes()
        {
            // arrange
            const string guid = "2401c7db-ec16-4afa-a8cb-c39d07050457";
            var requestId = Guid.Parse(guid);
            var query = _queryBuilder.BuildQueryFor(Queries.CurrentStatus, requestId);
            // act
            var asBytes = query.AsUTF8Bytes;

            // assert
            asBytes.Should().NotBeNull();
            asBytes.Should().BeOfType<byte[]>();
        }
    }
}
