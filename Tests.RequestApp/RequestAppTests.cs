using FluentAssertions;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using RequestsApp.Infrastructure;
using Tests.Helpers;
using Xunit;

namespace Test.RequestApp 
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
        public void CanInjectRequestHandlersIntoBroker()
        {
            // Arrange
            ICommandHandler requestHandler = new CommandHandler();
            IQueryHandler queryHandler = new QueryHandler();
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<RabbitMQServer>();

            // Act
            var rabbitMQServer = new RabbitMQServer(logger, requestHandler,queryHandler );
            rabbitMQServer.Run();

            // Assert
            Assert.Equal(requestHandler,  rabbitMQServer.CommandHandler);
            Assert.Equal(queryHandler, rabbitMQServer.QueryHandler);
        }
       
        [Theory]
        [InlineData("rider64", true)]    // works only if tests are run from within IDE
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