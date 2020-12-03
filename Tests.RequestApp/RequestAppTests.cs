using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Extensions.Logging;
using RequestsApp.Infrastructure;
using RequestsApp.Domain;
using Tests.Helpers;
using FluentAssertions;

namespace Tests.RequestApp 
{
    public class RequestAppTests : IClassFixture<SqliteInMemoryFixture>
    {
        // properties
        public DomainTypesFactory Factory { get;  }

        public SqliteInMemoryFixture Fixture { get; }
        
        //ctors
        public RequestAppTests(SqliteInMemoryFixture fixture)
        {
            Fixture = fixture;
            Factory = DomainTypesFactory.Instance; 
        }

        [Fact]
        public void CanInjectRequestHandlerIntoBroker()
        {
            // Arrange
            var requestHandler = new AuthorisationRequestsHandler();
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<RabbitMQServer>();

            // Act
            var rabbitMQServer = new RabbitMQServer(logger, requestHandler);
            rabbitMQServer.Run();
            var handler = rabbitMQServer.RequestHandler; 
            // Assert
            Assert.NotNull(handler);
            Assert.Equal(requestHandler, handler);
        }
       
        [Theory]
        [InlineData("RequestsApp", true)]
        [InlineData("GreatExpectations", false)]
        public void CanDetermineIfProcessIsRunning(string  processName, bool expected)
        {
            // arrange, act
            var actual = ProcessChecker.IsRunning(processName);
            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSaveRequestToDatabase()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var requestId = request.Id;
            var db = Fixture.Context; 
            // act
            db.Add(request);
            db.SaveChanges();
            var actual = db.AccountRequests.Find(requestId);
            // assert
            actual.Should().BeEquivalentTo(request);
        }
    }
}