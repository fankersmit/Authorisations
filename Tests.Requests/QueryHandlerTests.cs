using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Authorisations.Infrastructure;
using FluentAssertions;
using Xunit;
using Requests.Shared.Domain;
using RequestsApp.Domain;
using Tests.Helpers;

namespace Tests.Requests
{
    public class QueryHandlerTests
    {
        // properties
        public DomainTypesFactory Factory { get; }
        public SqliteInMemoryFixture Fixture { get; }

        // ctors
        public QueryHandlerTests()
        {
            Factory = DomainTypesFactory.Instance;
            Fixture = new SqliteInMemoryFixture();
        }

        public void Dispose()
        {
            Fixture.Dispose();
        }

        [Fact]
        public  void OnError_FailureResponse_IsReturned()
        {
            // arrange
            const string nonExistingKey = "52da30b2-503a-4923-85f4-7548b1259a8c";
            var handler = new QueryHandler(Fixture.Context);
            var args = new Dictionary<string, string>() {{"ID", nonExistingKey}};
            // act
            var result = handler.QueryFor(Queries.CurrentStatus, args);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

            // assert
            resultDict.Should().ContainKey("ID");
            resultDict.Should().ContainKey("Failure");
            resultDict["Failure"].Should().Contain(nonExistingKey);
            resultDict["ID"].Should().BeEquivalentTo(nonExistingKey);
        }

        [Fact]
        public void CanGetCurrentRequestStatus()
        {
            // arrange
            var handler = new QueryHandler(Fixture.Context);
            var requestDocument = Fixture.Context.RequestDocuments.First();
            var request = requestDocument.Request;
            var currentStatus = request.Status;
            var args = new Dictionary<string, string>() {{"ID", request.ID.ToString()}};

            // act
            var result = handler.QueryFor(Queries.CurrentStatus, args);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            var actualStatus = Enum.Parse<RequestStatus>(resultDict["Status"]);

            // assert
            resultDict.Should().ContainKey("ID");
            resultDict.Should().ContainKey("Status");
            resultDict["ID"].Should().BeEquivalentTo(request.ID.ToString());
            actualStatus.Should().Be(currentStatus);
        }

        public void UnderConsideration_All_Returns_Count()
        {
            // arrange
            var handler = new QueryHandler(Fixture.Context);
            var builder = new QueryBuilder();
            var query = builder.BuildQueryFor(Queries.UnderConsideration, "All");

            // act
            var result = handler.QueryFor(query.QueryType,  query.Arguments);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

            // assert
            resultDict.Count.Should().Be(3);
        }

    }
}
