using System;
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
    public class CommandHandlerTests : IClassFixture<SqliteInMemoryFixture>
    {
        // properties
        public DomainTypesFactory Factory { get; }
        public SqliteInMemoryFixture Fixture { get; }

        // ctors
        public CommandHandlerTests(SqliteInMemoryFixture fixture)
        {
            Factory = DomainTypesFactory.Instance;
            Fixture = fixture;
        }

        [Fact]
        public void VersionIsUpdated_HandlingCommand()
        {
            // arrange
            const int expected = 2; 
            var request = Factory.CreateAccountRequest();
            var version = request.Version;
            var dateCreated = request.DateCreated;
            var handler = new CommandHandler();
            
            // act
            var actual = handler.Handle(request, Commands.Submit);
            
            // assert
            request.Version.Should().NotBe(version);
            request.Version.Should().Be(expected);
        }
       
        [Fact]
        public void CanHandleSubmittedRequest()
        {
            var request = Factory.CreateAccountRequest();
            var handler = new CommandHandler();
            var subscriber = new CommandHandlerEventSubscriber();
            
            // act
            handler.CommandHandled += subscriber.SubmitCommandExecuted;
            var actual = handler.Handle(request, Commands.Submit);

            // assert
            actual.Should().BeTrue();
            subscriber.Request.ID.Equals(request.ID);
            subscriber.Request.Status.Should().Be(RequestStatus.Submitted);
        }

        [Fact]
        public void OnSubmittedCommand_RequestIsStored()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var copy = CreateDeepCopy(request);
            var requestId = request.ID;
            var dbContext = Fixture.Context;
            var handler = new CommandHandler();
            
            // act
            var previousCount = dbContext.RequestDocuments.Count(); 
            handler.CommandHandled += dbContext.OnCommandExecuted;   //subcribe to command
            handler.Handle(request, Commands.Submit);
            var actual = dbContext.RequestDocuments.Find(requestId,request.Version);
            
            // assert
            copy.Status.Should().Be(RequestStatus.New);
            actual.Request.Status.Should().Be(RequestStatus.Submitted);
            dbContext.RequestDocuments.Count().Should().BeGreaterThan(previousCount);
        }

        // helper classes and methods
        internal class CommandHandlerEventSubscriber
        {
            // ctors
            internal CommandHandlerEventSubscriber()
            {
                ReactedToEventNotification = false;
            }
            
            // props
            internal bool ReactedToEventNotification { get; set; }
            internal RequestBase Request { get; set; }

            // methods
            internal void SubmitCommandExecuted( object sender, CommandHandledEventArgs eventArgs )
            {
                ReactedToEventNotification = true;
                Request = eventArgs.Request;
            }
        }
        
        public RequestBase CreateDeepCopy( RequestBase model)
        {
            var json = model.SerializeToJson();
            var document = JsonDocument.Parse(json);
            var builder = new RequestFromJsonBuilder(null);
            return builder.GetRequest(document.RootElement);
        }
    }
}