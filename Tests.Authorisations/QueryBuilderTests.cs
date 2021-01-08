using System;
using System.Collections.Generic;
using Authorisations.Infrastructure;
using FluentAssertions;
using Requests.Shared.Domain;
using Xunit;

namespace Tests.Authorisations
{
    public class QueryBuilderTests
    {
        [Fact]
        public void CanBuild_HasStatus_Query()
        {
            // arrange
            var builder = new QueryBuilder();
            var requestStatus = RequestStatus.Confirmed;
            const string guid = "2401c7db-ec16-4afa-a8cb-c39d07050457";
            var args = new Dictionary<string, string>()
            {
                {"Status", RequestStatus.Confirmed.ToString()},
                {"ID", guid}
            };

            // act
            var query = builder.BuildQueryFor(Queries.HasStatus, args);

            // assert
            query.QueryType.Should().Be(Queries.HasStatus);
            query.Arguments.Count.Should().Be(2);
            query.Arguments["Status"].Should().Be(requestStatus.ToString());
        }

        [Fact]
        public void CanBuild_WithStatus_Query()
        {
            // arrange
            var builder = new QueryBuilder();
            var requestStatus = RequestStatus.Submitted;

            // act
            var query = builder.BuildQueryFor(Queries.WithStatus, requestStatus.ToString());

            // assert
            query.QueryType.Should().Be(Queries.WithStatus);
            query.Arguments.Count.Should().Be(1);
            query.Arguments["Status"].Should().Be(requestStatus.ToString());
        }

        [Theory]
        [InlineData("All")]
        [InlineData("Account")]
        [InlineData("Organisation")]
        [InlineData("Product")]
        public void CanBuild_UnderConsideration_Query(string queryType)
        {
            // arrange
             var builder = new QueryBuilder();
            // act
            var query = builder.BuildQueryFor(Queries.UnderConsideration, queryType);
            // assert
            query.QueryType.Should().Be(Queries.UnderConsideration);
            query.Arguments.Count.Should().Be(1);
            query.Arguments["Type"].Should().Be(queryType);
        }

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
