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
        public void CreatedResponseWithResponseContainsRequestInJson()
        {
            // arrange
            var builder = new ResponseBuilder();
            const string IdValue = "daf21b8d-42a1-4b37-a265-2748d2f430f7";
            const string keyName = "ID";
            var range = new Dictionary<string, string>() {{ keyName, IdValue}};
            var serializedRequest = "{ \"ID\": \"daf21b8d-42a1-4b37-a265-2748d2f430f7\", \"Version\": 2, \"DateCreated\": \"2020-12-22T21:24:39.7380242Z\", \"DateLastUpdated\": \"2020-12-22T21:24:43.6182682Z\", \"Status\": \"Submitted\", \"Remarks\": \"No remarks.\", \"Applicant\": {  \"ID\": 45373,  \"FirstName\": \"Alica\",  \"LastName\": \"Mo Osei\",  \"Salutation\": \"Mevr.\" }, \"Contract\": {  \"ID\": \"662939ed-6f92-414b-98cd-266a6c31e1bd\",  \"Organisation\": { \"ID\": \"6021cffe-5a50-4726-9656-fc30b13d0220\", \"Name\": \"ZorgMij\", \"Description\": \"\"  },  \"AuthorizerMailAddress\": \"mad.deschank@gmail.com\",  \"StartDate\": \"2020-12-20T22:24:39.738007+01:00\",  \"EndDate\": \"2021-12-22T22:24:39.7380131+01:00\",  \"Products\": [ {  \"ID\": 105,  \"Name\": \"MijnAGB\",  \"Description\": \"Self service van mijn persoonlijjke AGB code\",  \"StartDate\": \"2020-12-21T00:00:00+01:00\",  \"EndDate\": \"2023-12-22T00:00:00+01:00\" }, {  \"ID\": 123,  \"Name\": \"MijnVektis\",  \"Description\": \"Self service van mijn vektis account\",  \"StartDate\": \"2020-12-21T00:00:00+01:00\",  \"EndDate\": \"2023-12-22T00:00:00+01:00\" }  ] }}";

            // act
            var response = builder.Create(Queries.Request, range, serializedRequest);
            var jsonBytes = response.AsUTF8Bytes;
            var jsonDocument = JsonDocument.Parse(jsonBytes);

            // assert
            Action act = () => jsonDocument.RootElement.GetProperty("Request");
            act.Should().NotThrow<KeyNotFoundException>();
            var  element = jsonDocument.RootElement.GetProperty("Request");
            element.ValueKind.Should().Be(JsonValueKind.Object);
        }

        [Theory]
        [InlineData("") ]
        [InlineData(null)]
        public void CreateFailsWithNullOrEmptyRequestString( string serializedRequest)
        {
            // arrange
            var builder = new ResponseBuilder();
            const string IdValue = "daf21b8d-42a1-4b37-a265-2748d2f430f7";
            const string keyName = "ID";
            var range = new Dictionary<string, string>() {{ keyName, IdValue}};

            // act
            Action act = () => builder.Create(Queries.Request, range, serializedRequest);;

            // assert
            act.Should().Throw<ArgumentException>()
                .Where( e => e.Message.Contains("json"));
        }

        [Fact]
        public void CanCreateRequestResponse()
        {
            // arrange
            var builder = new ResponseBuilder();
            const string IdValue = "daf21b8d-42a1-4b37-a265-2748d2f430f7";
            const string keyName = "ID";
            var range = new Dictionary<string, string>() {{ keyName, IdValue}};
            var serializedRequest = "{ \"ID\": \"daf21b8d-42a1-4b37-a265-2748d2f430f7\", \"Version\": 2, \"DateCreated\": \"2020-12-22T21:24:39.7380242Z\", \"DateLastUpdated\": \"2020-12-22T21:24:43.6182682Z\", \"Status\": \"Submitted\", \"Remarks\": \"No remarks.\", \"Applicant\": {  \"ID\": 45373,  \"FirstName\": \"Alica\",  \"LastName\": \"Mo Osei\",  \"Salutation\": \"Mevr.\" }, \"Contract\": {  \"ID\": \"662939ed-6f92-414b-98cd-266a6c31e1bd\",  \"Organisation\": { \"ID\": \"6021cffe-5a50-4726-9656-fc30b13d0220\", \"Name\": \"ZorgMij\", \"Description\": \"\"  },  \"AuthorizerMailAddress\": \"mad.deschank@gmail.com\",  \"StartDate\": \"2020-12-20T22:24:39.738007+01:00\",  \"EndDate\": \"2021-12-22T22:24:39.7380131+01:00\",  \"Products\": [ {  \"ID\": 105,  \"Name\": \"MijnAGB\",  \"Description\": \"Self service van mijn persoonlijjke AGB code\",  \"StartDate\": \"2020-12-21T00:00:00+01:00\",  \"EndDate\": \"2023-12-22T00:00:00+01:00\" }, {  \"ID\": 123,  \"Name\": \"MijnVektis\",  \"Description\": \"Self service van mijn vektis account\",  \"StartDate\": \"2020-12-21T00:00:00+01:00\",  \"EndDate\": \"2023-12-22T00:00:00+01:00\" }  ] }}";

            // act
            var response = builder.Create(Queries.Request, range, serializedRequest);

            // assert
            response.Should().BeOfType<RequestResponse>();
        }

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
