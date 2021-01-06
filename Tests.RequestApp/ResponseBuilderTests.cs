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

namespace Test.RequestApp
{
    public class ResponseBuilderTests
    {
        [Fact]
        public void CanCreateResponseWithArgumentRange()
        {
            // arrange
            var builder = new ResponseBuilder();
            var type = Queries.CurrentStatus;
            const string IdValue = "52da30b2-503a-4923-85f4-7548b1259a8c";
            const string keyName = "ID";
            var range = new Dictionary<string, string>() {{ keyName, IdValue}};

            // act
            Response response = builder.Create(type, range);

            // assert
            response.QueryType.Should().Be(type);
            response.Arguments.Count.Should().Be(1);
            response.Arguments[keyName].Should().Be(IdValue);
        }

        [Fact]
        public void CanCreateResponseWithArgument()
        {
            // arrange
            var builder = new ResponseBuilder();
            var type = Queries.CurrentStatus;
            const string keyName = "ID";
            const string IdValue = "52da30b2-503a-4923-85f4-7548b1259a8c";

            // act
            Response response = builder.Create(type, keyName, IdValue);

            // assert
            response.QueryType.Should().Be(type);
            response.Arguments.Count.Should().Be(1);
            response.Arguments[keyName].Should().Be(IdValue);
        }

        [Theory]
        [InlineData(Queries.Ping)]
        [InlineData(Queries.History)]
        [InlineData(Queries.Invalid)]
        [InlineData(Queries.CurrentStatus)]
        [InlineData(Queries.HasStatus)]
        [InlineData(Queries.WithStatus)]
        [InlineData(Queries.UnderConsideration)]
        [InlineData(Queries.Request)]
        public void CanCreateResponseWithBuilder( Queries queryType )
        {
            // arrange
            var builder = new ResponseBuilder();

            // act
            Response response = builder.Create(queryType);

            // assert
            response.QueryType.Should().Be(queryType);
            response.Arguments.Count.Should().Be(0);
        }
    }
}
