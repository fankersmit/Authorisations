using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using Authorisations;
using FluentAssertions;
using Requests.Shared.Domain;
using Tests.Helpers;

namespace Tests.Authorisations
{
    public class AuthorisationsControllerTests : IClassFixture<RequestsAppFixture>, IClassFixture< WebApplicationFactory<Startup>> 
    {
        // fields
        private readonly ModelTypesFactory _requestModelsFactory;
        private readonly HttpClient _client;
        private readonly string _root = "api/authorisations";
        private readonly RequestsAppFixture _fixture;
        private readonly WebApplicationFactory<Startup> _factory;

        // ctors
        public AuthorisationsControllerTests( RequestsAppFixture fixture, WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _requestModelsFactory = ModelTypesFactory.Instance;
            // start requestApp if needed
            _fixture = fixture;
        }

        //dtors
        ~AuthorisationsControllerTests()
        {
            _factory.Dispose();
            _fixture.Dispose();
        }

        public void Dispose()
        {
            _factory.Dispose();
            _fixture.Dispose();
        }

        [Fact()]
        public async Task Can_Ping_ApiStatus()
        {
            // arrange
            var expectedKeys = new  string[] {"Query" , "Webserver" , "Broker" , "RequestHandler" , "Store" };

            // act
            var response = await _client.GetAsync($"{_root}/ping");
            var body = await response.Content.ReadAsStringAsync();
            var resultDict = JsonSerializer.Deserialize<Dictionary<string,string>>(body);

            // assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
            resultDict.Count.Should().Be(5);
            foreach (var expectedKey in expectedKeys)
            {
                resultDict.ContainsKey(expectedKey).Should().BeTrue();
            }

        }
        /*
        [Fact]
        public async Task Query_Returns_Json_For_Negative_Result()
        {
        }

        [Fact]
        public async Task Query_Returns_Json_For_Positive_Result()
        {
        }
        */
#region tests to redesign
        [Theory]
        [InlineData("f9782b64-a7f5-46f1-84ec-d58e5e0de030")]
        [InlineData("10b79e1f-60b4-48a8-8697-08349a16dea7")]
        [InlineData("89f35353-18d1-4ba6-a21e-223bbc8918e2")]
        public async Task Query_Returns_NotFound_For_NonExisting_ID(string requestId)
        {
            // arrange
            var expected = HttpStatusCode.NotFound;
            // act
            var response = await _client.GetAsync($"{_root}/request/{requestId}/status");
            // assert
            Assert.Equal(expected, response.StatusCode);
        }

        // TODO
        [Theory]
        [InlineData("f9782b64-a7f5-46f1-84ecd58e5e0de030")]
        [InlineData("10b79e1f-60b4-48a8-8697-08349a6dea7")]
        [InlineData("f35353-18d1-4ba6-a21e-223bbc8918e2")]
        public async Task Query_Returns_BadRequest_For_NonGuid_ID( string requestId)
        {
            // arrange
            var expected = HttpStatusCode.BadRequest;
            // act
            var response = await _client.GetAsync($"{_root}/request/{requestId}/status");
            // assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Fact(Skip="redesign needed")]
        public async Task After_Submit_UnderConsideration_IsIncremented()
       {
            const string requestPath = "/requests/under-consideration/account/count";
            const string submitPath = "/request/submit/account";
            
            // arrange
            var response = await _client.GetAsync($"{_root}{requestPath}");
            var content = await response.Content.ReadAsStringAsync();
            var previousCount = JsonSerializer.Deserialize<Dictionary<string,int>>(content).Values.First();
            
            var request = _requestModelsFactory.CreateRequest();
            var body = Encoding.UTF8.GetString(request.SerializeToJson()); 
            var requestContent = new StringContent( body, Encoding.UTF8, "application/json");
 
            // act, submit request
            var result = await  _client.PostAsync($"{_root}{submitPath}", requestContent);
            // get new under_consideration count
            response = await _client.GetAsync($"{_root}{requestPath}");
            content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string,int>>(content);
            // assert
            int actualCount = dict.Values.First(); //only one item in collection
            actualCount.Should().BeGreaterOrEqualTo(previousCount);
        }

        [Theory(Skip="redesign needed")]
        [InlineData("account", HttpStatusCode.Accepted )]
        [InlineData("organisation", HttpStatusCode.NotFound )]
        [InlineData("product", HttpStatusCode.NotFound )]
        [InlineData("Henk", HttpStatusCode.NotFound )]
        public async Task Submit_ReturnsAccepted( string route, HttpStatusCode expected)
        {
            // arrange
            var request =  _requestModelsFactory.CreateRequest();
            var body = Encoding.UTF8.GetString(request.SerializeToJson()); 
            var content = new StringContent( body, Encoding.UTF8, "application/json");
            // act
            var response = await _client.PostAsync($"{_root}/request/submit/{route}", content);
            // assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Theory(Skip="redesign needed")]
        [InlineData("properties", HttpStatusCode.BadRequest )]
        [InlineData("2345-3456", HttpStatusCode.BadRequest )]
        public async Task GetUnderConsideration_Wrong_RequestType_Returns_BadRequest(string route, HttpStatusCode expected)
        {
            // arrange, act
            var response = await _client.GetAsync($"{_root}/requests/under-consideration/{route}");
            // assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Theory(Skip="redesign needed")]
        [InlineData("", HttpStatusCode.OK, 19 )]
        [InlineData("/account", HttpStatusCode.OK, 19 )]
        [InlineData("/product", HttpStatusCode.OK, 0 )]
        [InlineData("/organisation", HttpStatusCode.OK, 0 )]
        public async Task GetRequestUnderConsideration_Returns_SuccessAndCount(string route, HttpStatusCode statusCode, int count )
        {
            // arrange, act
            var response = await _client.GetAsync($"{_root}/requests/under-consideration{route}");
            var content = response.Content.ReadAsStringAsync().Result;
            var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(content);

            // assert
            Action act = () => response.EnsureSuccessStatusCode(); 
            act.Should().NotThrow<HttpRequestException>();
            response.StatusCode.Should().Be(statusCode);
            
            int actualCount = dict.Values.First(); //only one item in collection
            actualCount.Should().BeGreaterOrEqualTo(count);
        }
#endregion



        // -------------------------------------------------------------
        // private helper methods and classes
        //
        private async Task<T> DeserializeJson<T>(HttpContent content)
        {
            var responseObject = JsonSerializer.Deserialize<T>(
                await content.ReadAsStringAsync(),
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            return responseObject;
        }
        
        private class RunningStatus
        {
            public string Status { get; set; }
        }
        
        private class RequestsUnderConsideration
        {
            public int Count { get; set; }
        }
    }
}
