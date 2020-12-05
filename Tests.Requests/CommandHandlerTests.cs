using System;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Requests.Domain;
using Requests.Shared.Domain;
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
        public void CanSubmitRequest()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var dateCreated = request.DateCreated;
            var handler = new CommandHandler();
            
            // act
            var actual = handler.Handle(request, Commands.Submit);
            
            // assert
            actual.Should().BeTrue();
            request.Status.Should().Be(RequestStatus.Submitted);
            request.DateLastUpdated.Should().BeAfter(dateCreated);
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
            subscriber.Request.Id.Equals(request.Id);
            subscriber.Request.Status.Should().Be(RequestStatus.Submitted);
        }

        [Fact]
        public void SubmittedRequestIsStored()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var copy = CreateDeepCopy(request);
            var requestId = request.Id;
            var dbContext = Fixture.Context;
            var handler = new CommandHandler();
            
            // act
            var previousCount = dbContext.AccountRequests.Count(); 
            handler.CommandHandled += dbContext.CommandExecuted;   //suscribe to command
            handler.Handle(request, Commands.Submit);
            var actual = dbContext.AccountRequests.Find(requestId);
            
            // assert
            copy.Status.Should().Be(RequestStatus.New);
            actual.Status.Should().Be(RequestStatus.Submitted);
            dbContext.AccountRequests.Count().Should().BeGreaterThan(previousCount);
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
        
        public TRequest CreateDeepCopy<TRequest>( TRequest model)
        {
            var options = new JsonSerializerOptions {WriteIndented = true};
            byte[] serialized = JsonSerializer.SerializeToUtf8Bytes<TRequest>( model, options);
            var readOnlySpan = new ReadOnlySpan<byte>(serialized);
            return JsonSerializer.Deserialize<TRequest>(readOnlySpan);
        }
    }
}