using System;
using Authorisations.Infrastructure;
using FluentAssertions;
using Requests.Shared.Domain;
using Xunit;

namespace Tests.Authorisations
{
    public class QueryBuilderTests
    {
        [Fact]
        public void CanBuild_CurrentStatus_Query()
        {
            // arrange
            const string guid = "2401c7db-ec16-4afa-a8cb-c39d07050457";
            var builder = new QueryBuilder();
            var requestId = Guid.Parse(guid);
            // act
            var query = builder.BuildQueryFor(Queries.CurrentStatus, requestId);
            // assert
            query.QueryType.Should().Be(Queries.CurrentStatus);
            query.Arguments.Count.Should().Be(1);
            query.Arguments["ID"].Should().Be(guid);
        }

        [Fact]
        public void CanBuild_Ping_Query()
        {
            // arrange
            var builder = new QueryBuilder();

            // act
            var query = builder.BuildQueryFor(Queries.Ping, "None");

            // assert
            query.QueryType.Should().Be(Queries.Ping);
            query.Arguments.Count.Should().Be(0);
        }

    }
}
