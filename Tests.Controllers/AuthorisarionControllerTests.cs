using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using AuthorisationRequest;

namespace Tests.Controllers
{
    [Collection("Integration Tests")]
    public class AuthorisationControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly string _root = "api/authorisationrequest";

        public AuthorisationControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
        
        [Theory]
        [InlineData("requests", HttpStatusCode.OK, 0 )]
        [InlineData("requests/account", HttpStatusCode.OK, 0 )]
        [InlineData("requests/product", HttpStatusCode.OK, 0 )]
        [InlineData("requests/organisation", HttpStatusCode.OK, 0 )]
        public async Task GetRequestUnderConsideration_ReturnsSuccessAndCount(string route, HttpStatusCode statusCode, int count )
        {
            // arrange
            var client = _factory.CreateClient();
            // act
            var response = await client.GetAsync($"{_root}/{route}/under-consideration");
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
            var client = _factory.CreateClient();
            var expected = "Up";
            // act
            var response = await client.GetAsync($"{_root}");
            
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