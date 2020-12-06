using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Authorisations;
using FluentAssertions;
using Tests.Helpers;

namespace Tests.Controllers
{
    public class AuthorisationsControllerTests : IClassFixture<RequestsAppFixture>, IClassFixture< WebApplicationFactory<Startup>> 
    {
        // fields
        private readonly DomainTypesFactory _requestFactory;
        private readonly HttpClient _client;
        private readonly string _root = "api/authorisations";
        private RequestsAppFixture _fixture;

        // ctors
        public AuthorisationsControllerTests( RequestsAppFixture fixture, WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _requestFactory = DomainTypesFactory.Instance;
            // start requestApp if needed
            _fixture = fixture;
        }
   
       [Fact]
       public async Task  After_Submit_UnderConsideration_HasChanged()
        {
            // arrange
            var response =  await _client.GetAsync($"{_root}/requests/under-consideration/account");
            var ruc = await DeserializeJson<RequestsUnderConsideration>(response.Content);
            
            var request = _requestFactory.CreateAccountRequest();
            var body = Encoding.UTF8.GetString(request.ToJson()); 
            var content = new StringContent( body, Encoding.UTF8, "application/json");
 
            // act
            await _client.PostAsync($"{_root}/request/submit/account", content);
            // get new under_consideration count
            response  =  await _client.GetAsync($"{_root}/requests/under-consideration/account");
            var newRuc = await DeserializeJson<RequestsUnderConsideration>(response.Content);

            // assert
            newRuc.Count.Should().BeGreaterThan(ruc.Count); // one more items under consideration
        }

        [Theory]
        [InlineData("account", HttpStatusCode.Accepted )]
        [InlineData("organisation", HttpStatusCode.NotFound )]
        [InlineData("product", HttpStatusCode.NotFound )]
        [InlineData("Henk", HttpStatusCode.NotFound )]
        public async Task Submit_ReturnsAccepted( string route, HttpStatusCode expected)
        {
            // arrange
            var request = _requestFactory.CreateAccountRequest();
            var body = Encoding.UTF8.GetString(request.ToJson()); 
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
        [InlineData("", HttpStatusCode.OK, 0 )]
        [InlineData("/account", HttpStatusCode.OK, 0 )]
        [InlineData("/product", HttpStatusCode.OK, 0 )]
        [InlineData("/organisation", HttpStatusCode.OK, 0 )]
        public async Task GetRequestUnderConsideration_Returns_SuccessAndCount(string route, HttpStatusCode statusCode, int count )
        {
            // arrange, act
            var response = await _client.GetAsync($"{_root}/requests/under-consideration{route}");
            // assert
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch( HttpRequestException e )
            {
                Assert.False( 0==0, e.Message );
            }
            Assert.NotNull(response.Content);
            Assert.Equal(statusCode, response.StatusCode);
            var ruc = await DeserializeJson<RequestsUnderConsideration>(response.Content);
            Assert.Equal(count, ruc.Count);
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
            T responseObject = JsonSerializer.Deserialize<T>(
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
            public string RequstType { get; set; }  
            public int Count { get; set; }
        }
        
    }
}