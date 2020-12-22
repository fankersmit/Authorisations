using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Requests.Domain;
using Requests.Shared.Domain;
using RequestsApp.Domain;
using RequestsApp.Infrastructure;
using Tests.Helpers;

namespace Tests.Requests
{
    public class QueryHandlerTests: IClassFixture<SqliteInMemoryFixture>
    {
        // properties
        public DomainTypesFactory Factory { get; }
        public SqliteInMemoryFixture Fixture { get; }

        // ctors
        public QueryHandlerTests(SqliteInMemoryFixture fixture)
        {
            Factory = DomainTypesFactory.Instance;
            Fixture = fixture;
        }

        [Fact]
        public void UnderConsideration_Returns_Count()
        {
            var handler = new QueryHandler(Fixture.Context);
            // act
            var count = handler.AllRequestsUnderConsideration();
            
            // assert
            count.Should().NotBe(0);
        }

    }
}