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
        public void Can_Retrieve_Requests_WithStatus()
        {
            // arrange
            var keys = new string[] { "Query", "Status" }; // keys in response
            var length = keys.Length;
            var handler = new QueryHandler(Fixture.Context);
            var args = new Dictionary<string, string>()
            {
                { "Status", RequestStatus.Submitted.ToString() }
            };
            // act
            var result = handler.QueryFor(Queries.WithStatus, args);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            // assert
            foreach (var s in keys)
            {
                resultDict.Should().ContainKey(s);
            }
            resultDict.Keys.Count.Should().BeGreaterThan(3);
        }

        [Fact]
        public void Can_Determine_If_Request_HasStatus()
        {
            // arrange
            var keys = new string[] { "Query", "ID", "Status", "Result"}; //keys response
            var length = keys.Length;
            var handler = new QueryHandler(Fixture.Context);

            // get a request to work with
            var requestDocument = Fixture.Context.RequestDocuments.First();
            var request = requestDocument.Request;
            var args = new Dictionary<string, string>()
            {
                { "ID", request.ID.ToString()},
                { "Status", request.Status.ToString() }
            };

            // act
            var result = handler.QueryFor(Queries.HasStatus, args);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            var hasStatus = bool.Parse(resultDict["Result"]);

            // assert
            foreach (var s in keys)
            {
                resultDict.Should().ContainKey(s);
            }
            hasStatus.Should().BeTrue();
        }

        [Theory]
        [InlineData( Queries.Request) ]
        [InlineData( Queries.HasStatus)]
        [InlineData( Queries.CurrentStatus)]
        [InlineData(Queries.History)]
        public  void On_ID_NotFound_FailureResponse_IsReturned( Queries queryType)
        {
            // arrange
            var keys = new string[] { "Query", "ID", "Failure" }; //keys response
            var length = keys.Length;

            const string nonExistingKey = "52da30b2-503a-4923-85f4-7548b1259a8c";
            var handler = new QueryHandler(Fixture.Context);
            var args = new Dictionary<string, string>() {{"ID", nonExistingKey}};
            if (queryType == Queries.HasStatus)
            {
                args.Add("Status", RequestStatus.Concluded.ToString());
            }

            // act
            var result = handler.QueryFor(queryType, args);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

            // assert
            foreach (var s in keys)
            {
                resultDict.Should().ContainKey(s);
            }
            resultDict["Failure"].Should().Contain(nonExistingKey);
            resultDict["ID"].Should().BeEquivalentTo(nonExistingKey);
        }

        [Fact]
        public void CanGet_CurrentStatus_Request()
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

        [Theory]
        [InlineData("All", 3)]
        [InlineData("Account", 3)]
        [InlineData("Product", 3)]
        [InlineData("Organisation", 3)]
        public void UnderConsideration_Returns_Count(string requestType, int count)
        {
            // arrange
            var handler = new QueryHandler(Fixture.Context);
            var builder = new QueryBuilder();
            var query = builder.BuildQueryFor(Queries.UnderConsideration, requestType);

            // act
            var result = handler.QueryFor(query.QueryType,  query.Arguments);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

            // assert
            resultDict.Count.Should().Be(count);
        }

    }
}
