using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Authorisations;
using Tests.Helpers;

namespace Tests.Controllers
{
    [Collection("Integration Tests")]
    public class AuthorisationsControllerTests
    {
        private readonly DomainTypesFactory _requestFactory;
        private readonly HttpClient _client;
        private readonly string _root = "api/authorisations";

        public AuthorisationsControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _requestFactory = DomainTypesFactory.Instance;
        }

        [Theory]
        [InlineData("account", HttpStatusCode.Accepted )]
        [InlineData("organisation", HttpStatusCode.Accepted )]
        [InlineData("product", HttpStatusCode.Accepted )]
        [InlineData("Henk", HttpStatusCode.NotFound )]
        public async Task Submit_ReturnsAccepted( string route, HttpStatusCode expected)
        {
            // arrange
            var request = _requestFactory.CreateAccountRequest();
            var content = new StringContent( request.ToJson(), Encoding.Default, "application/json");
            // act
            var response = await _client.PostAsync($"{_root}/request/{route}/submit", content);
            // assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Theory]
        [InlineData("requests", HttpStatusCode.OK )]
        [InlineData("requests/properties", HttpStatusCode.BadRequest )]
        [InlineData("requests/2345-3456", HttpStatusCode.BadRequest )]
        public async Task GetWithWrongRequestType_ReturnsBadRequest(string route, HttpStatusCode expected)
        {
            // arrange, act
            var response = await _client.GetAsync($"{_root}/{route}/under-consideration");
            // assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Theory]
        [InlineData("requests", HttpStatusCode.OK, 0 )]
        [InlineData("requests/account", HttpStatusCode.OK, 0 )]
        [InlineData("requests/product", HttpStatusCode.OK, 0 )]
        [InlineData("requests/organisation", HttpStatusCode.OK, 0 )]
        public async Task GetRequestUnderConsideration_ReturnsSuccessAndCount(string route, HttpStatusCode statusCode, int count )
        {
            // arrange, act
            var response = await _client.GetAsync($"{_root}/{route}/under-consideration");
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