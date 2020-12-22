using System.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using Requests.Shared.Domain;
using RequestsApp.Domain;
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
        public void HandlerExecutesSubmitCommand()
        {
            // arrange
            var server = CreateRMQServer();
            var request = Factory.CreateAccountRequest();
            var version = request.Version;
            var requestId = request.ID;
            var command = Commands.Submit;
            
            // act
            server.Run(Fixture.Context); // wire up event handling
            server.CommandHandler.Handle(request, command); // submit and save to store
            
            var db = Fixture.Context; 
            var actual = db.RequestDocuments.Find(requestId, version+1);
            // assert
            actual.Request.Status.Should().Be(RequestStatus.Submitted);
        }

        [Fact]
        public void CanInjectRequestHandlersIntoBroker()
        {
            // Arrange
            var context = Fixture.Context;
            ICommandHandler requestHandler = new CommandHandler();
            IQueryHandler queryHandler = new QueryHandler(context);
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<RabbitMQServer>();

            // Act
            var rabbitMQServer = new RabbitMQServer(logger, requestHandler,queryHandler );
            rabbitMQServer.Run( Fixture.Context);

            // Assert
            Assert.Equal(requestHandler,  rabbitMQServer.CommandHandler);
            Assert.Equal(queryHandler, rabbitMQServer.QueryHandler);
        }
       
        [Theory(Skip="Works for now, reactivate when integration test scenarios change")]
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
        public void CanSaveRequestDocumentToDatabase()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var requestDocument = RequestDocumentFactory.Create(request, Commands.Submit);
            var requestId = request.ID;
            var db = Fixture.Context; 
            // act
            db.Add(requestDocument);
            db.SaveChanges();
            var actual = db.RequestDocuments.Find(requestId, requestDocument.Version);
            // assert
            actual.Request.Should().BeEquivalentTo(request);
        }
        
        // private helper method
        RabbitMQServer CreateRMQServer()
        {
            // Arrange
            var context = Fixture.Context;
            ICommandHandler requestHandler = new CommandHandler();
            IQueryHandler queryHandler = new QueryHandler(context);
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<RabbitMQServer>();

            // Act
            var rabbitMQServer = new RabbitMQServer(logger, requestHandler,queryHandler );
            return rabbitMQServer;
        }
    }
}