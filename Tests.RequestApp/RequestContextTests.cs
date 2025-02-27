using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

using Requests.Domain;
using Requests.Shared.Domain;
using RequestsApp.Domain;
using RequestsApp.Infrastructure;
using Tests.Helpers;

namespace Test.RequestApp
{
    public class RequestContextTests : IClassFixture<SqliteInMemoryFixture>
    {
        public SqliteInMemoryFixture Fixture { get; }
        public DomainTypesFactory Factory { get;  }
        
        public RequestContextTests(SqliteInMemoryFixture fixture)
        {
            Fixture = fixture;
            Factory = DomainTypesFactory.Instance; 
        }

        [Fact]
        public void Walking_HappyPath_CreatesFiveVersions()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var originalVersion = request.Version;
            var expectedVersion = originalVersion + 4;
            var dbContext = Fixture.Context; 
            IList<Commands> commands = new List<Commands>
            {
                Commands.Submit,
                Commands.Confirm,
                Commands.Approve,
                Commands.Conclude,
                Commands.Remove
            };
            // act
            var handler = new CommandHandler();
            handler.CommandHandled += dbContext.OnCommandExecuted;

            foreach (var command in commands)
            {
                handler.Handle(request, command);
            }

            // assert
            var  retrievedDocmuent = dbContext.RequestDocuments.Find(request.ID, expectedVersion);
            retrievedDocmuent.Version.Should().Be(expectedVersion);
        }

        [Fact]
        public void DocumentVersion_IsUpdatedWhenPersisted()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var originalVersion = request.Version;
            var expectedVersion = originalVersion + 1;
            var dbContext = Fixture.Context; 
            
            // act
            var handler = new CommandHandler();
            handler.CommandHandled += dbContext.OnCommandExecuted;
            handler.Handle(request, Commands.Submit);
 
            // assert
            var  retrievedDocmuent = dbContext.RequestDocuments.Find(request.ID, expectedVersion);
            retrievedDocmuent.Version.Should().Be(expectedVersion);
        } 
        
        [Fact]
        public void Document_Adding_IsIdemPotent()
        {
            // arrange
            var request = Factory.CreateAccountRequest();
            var originalVersion = request.Version;
            var expectedVersion = originalVersion + 1;
            var dbContext = Fixture.Context; 
            
            // act
            var handler = new CommandHandler();
            handler.CommandHandled += dbContext.OnCommandExecuted;
            handler.Handle(request, Commands.Submit);
            // second time adding same document
            handler.Handle(request, Commands.Submit); 
            // assert
            var  retrievedDocmuent = dbContext.RequestDocuments.Find(request.ID, expectedVersion);
            retrievedDocmuent.Version.Should().Be(expectedVersion);
        } 
    }
}