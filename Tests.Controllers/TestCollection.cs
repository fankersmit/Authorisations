using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Authorisation;

namespace Tests.Controllers
{
    [CollectionDefinition("Integration Tests")]
    public class TestCollection : ICollectionFixture<WebApplicationFactory<Authorisation.Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}