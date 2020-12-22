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
using Requests.Shared;
using FluentAssertions;
using FluentAssertions.Extensions;
using Requests.Domain;
using Requests.Shared.Domain;
using Tests.Helpers;

namespace Tests.Controllers
{
    public class AuthorisationsControllerTests : IClassFixture<RequestsAppFixture>, IClassFixture< WebApplicationFactory<Startup>> 
    {
        // fields
        private readonly ModelTypesFactory _requestModelsFactory;
        private readonly HttpClient _client;
        private readonly string _root = "api/authorisations";
        private RequestsAppFixture _fixture;

        // ctors
        public AuthorisationsControllerTests( RequestsAppFixture fixture, WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _requestModelsFactory = ModelTypesFactory.Instance;
            // start requestApp if needed
            _fixture = fixture;
            _fixture.StartRequestsApp();
        }
   
       [Fact]
       public async void After_Submit_UnderConsideration_IsIncremented()
       {
            const string requestPath = "/requests/under-consideration/account";
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

        [Theory]
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

        [Theory]
        [InlineData("properties", HttpStatusCode.BadRequest )]
        [InlineData("2345-3456", HttpStatusCode.BadRequest )]
        public async Task GetUnderConsideration_Wrong_RequestType_Returns_BadRequest(string route, HttpStatusCode expected)
        {
            // arrange, act
            var response = await _client.GetAsync($"{_root}/requests/under-consideration/{route}");
            // assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Theory]
        [InlineData("", HttpStatusCode.OK, 19 )]
        [InlineData("/account", HttpStatusCode.OK, 19 )]
        [InlineData("/product", HttpStatusCode.OK, 0 )]
        [InlineData("/organisation", HttpStatusCode.OK, 0 )]
        public async Task GetRequestUnderConsideration_Returns_SuccessAndCount(string route, HttpStatusCode statusCode, int count )
        {
            // arrange, act
            var response = await _client.GetAsync($"{_root}/requests/under-consideration{route}");
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(content);

            // assert
            Action act = () => response.EnsureSuccessStatusCode(); 
            act.Should().NotThrow<HttpRequestException>();
            response.StatusCode.Should().Be(statusCode);
            
            int actualCount = dict.Values.First(); //only one item in collection
            actualCount.Should().BeGreaterOrEqualTo(count);
        }

        [Fact]
        public async Task GetRoot_ReturnsSuccessAndStatusUp()
        {
            // arrange
            var expected = "Up";
            // act
            var response = await _client.GetAsync($"{_root}");
            
            // assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
            var runningStatus = await DeserializeJson<RunningStatus>(response.Content);
            Assert.Equal(expected, runningStatus.Status);
        }

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