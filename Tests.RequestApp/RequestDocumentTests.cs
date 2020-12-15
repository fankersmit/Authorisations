using System;
using System.Text.Json;
using FluentAssertions;
using Requests.Shared.Domain;
using RequestsApp.Infrastructure;
using Tests.Helpers;
using Xunit;

namespace Test.RequestApp
{
    public class RequestDocumentTests
    {
        [Fact]
        public void CanCreateDocumentFromRequest()
        {
            // arrange
            var request = DomainTypesFactory.Instance.CreateAccountRequest();

            // act
            var document = RequestDocumentFactory.Create(request, Commands.NoOp);
            // assert
            document.Should().BeOfType<RequestDocument>();
            document.Request.Should().Be(request);
            document.ID.Should().Be(request.ID);
        }
        
        [Fact]
        public void FactoryCreatesFullDocument()
        {
            // arrange
            var request = DomainTypesFactory.Instance.CreateAccountRequest();
            var expectedLastName = request.Applicant.LastName;
            var expectedCommand = Commands.Submit;
            
            // act
            var document = RequestDocumentFactory.Create(request, expectedCommand);
            var actualLastName = GetApplicantLastNameFromDocument(document.Document);
            
            // assert
            document.Request.Should().Be(request);
            document.ID.Should().Be(request.ID);
            document.TimeStamp.Should().BeGreaterThan(0);
            document.TimeStamp.Should().BeLessThan(DateTime.UtcNow.Ticks);
            document.Document.Should().NotBeNull();
            document.Document.Should().BeOfType<JsonDocument>();
            actualLastName.Should().Be(expectedLastName); //check sample of content  
            document.Command.Should().Be(expectedCommand);
        }

        [Fact]
        public void CanGetJsonDocumentFromRequestDocument()
        {
            // arrange
            var request = DomainTypesFactory.Instance.CreateAccountRequest();
            var document = RequestDocumentFactory.Create(request, Commands.NoOp);

            // act
            var jsonDocument = document.Document;
            
            // assert
            jsonDocument.Should().BeOfType<JsonDocument>();
        }
        
        // helper methods
        private string GetApplicantLastNameFromDocument(JsonDocument document)
        {
            var applicant = document.RootElement.GetProperty("Applicant");
            var  lastName = applicant.GetProperty("LastName").GetString();
            return lastName;
        }
    }
}

    