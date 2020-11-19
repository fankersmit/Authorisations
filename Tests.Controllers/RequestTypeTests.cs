using System;
using System.IO;
using System.Text.Json;
using Authorisations.Models;
using Xunit;

namespace Tests.Controllers
{
    public class RequestTypeTests
    {
        [Theory]
        [InlineData( RequestType.Account, "account")]
        [InlineData( RequestType.Organisation, "organisation")]
        [InlineData( RequestType.Product, "product")]
        void CanToStringRequestType(RequestType requestType, string expected )
        {
            // arrange
            var rt = requestType;
            // act
            var description = rt.ToString("G").ToLower();
            // assert
            Assert.Equal(expected, description);
        }

        [Theory]
        [InlineData("account", true)]
        [InlineData("organisation", true)]
        [InlineData("product", true)]
        [InlineData("Account", true)]
        [InlineData("Organisation", true)]
        [InlineData("Product", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("henk", false)]
        void CanCheckForExistingRequestType( string request, bool expected)
        {
            // arrange, act
            var actual = RequestChecker.IsKnownRequestType(request);
            // assert
            Assert.Equal(expected, actual);

        }
    }
}