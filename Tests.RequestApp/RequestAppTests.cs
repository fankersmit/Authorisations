using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;
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
    public class RequestAppTests
    {
        // properties
        public DomainTypesFactory Factory { get;  }
        public SqliteInMemoryFixture Fixture { get; }
        
        //ctors
        public RequestAppTests()
        {
            Fixture = new SqliteInMemoryFixture();
            Factory = DomainTypesFactory.Instance; 
        }

        [Fact]
        public void QueryHandler_Executes_QueryFor()
        {
            // arrange
            var server = CreateRMQServer( Fixture);
            var requestdocument = Fixture.Context.RequestDocuments.First();
            var request = requestdocument.Request;
            var requestId = request.ID.ToString();
            var query = Queries.CurrentStatus;
            var args =  new Dictionary<string, string> {{"ID", requestId}};

            // act
            var resultBytes  = server.QueryHandler.QueryFor(query, args); // submit and save to store
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(resultBytes);
            // assert
            result["ID"].Should().BeEquivalentTo(requestId);
            result["Status"].Should().Be("Submitted");
            server.Stop();
        }

        [Fact]
        public void CommandHandler_Executes_SubmitCommand()
        {
            // arrange
            var server = CreateRMQServer(Fixture);
            var request = Factory.CreateAccountRequest();
            var version = request.Version;
            var requestId = request.ID;
            var command = Commands.Submit;
            
            // act
            server.CommandHandler.Handle(request, command); // submit and save to store
            var db = Fixture.Context;
            var actual = db.RequestDocuments.Find(requestId, version+1);

            // assert
            actual.Request.Status.Should().Be(RequestStatus.Submitted);
            server.Stop();
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
            var rabbitMQServer = new RabbitMQServer(logger);
            rabbitMQServer.Run( requestHandler, queryHandler);

            // Assert
            Assert.Equal(requestHandler,  rabbitMQServer.CommandHandler);
            Assert.Equal(queryHandler, rabbitMQServer.QueryHandler);
            rabbitMQServer.Stop();
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
        RabbitMQServer CreateRMQServer( SqliteInMemoryFixture fixture)
        {
            // Arrange
            ICommandHandler requestHandler = new CommandHandler();
            requestHandler.CommandHandled += fixture.Context.OnCommandExecuted;
            IQueryHandler queryHandler = new QueryHandler(fixture.Context);
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<RabbitMQServer>();

            // Act
            var rabbitMQServer = new RabbitMQServer(logger);
            rabbitMQServer.Run(requestHandler, queryHandler);
            return rabbitMQServer;
        }
    }
}
