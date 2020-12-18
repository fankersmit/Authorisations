using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Authorisations.Models;
using FluentAssertions;
using Xunit;
using Requests.Domain;
using Requests.Shared.Domain;
using RequestsApp.Infrastructure;
using Tests.Helpers;
namespace Tests.Requests
{
    public class RequestBuilderTests
    {
        private readonly DomainTypesFactory factory = DomainTypesFactory.Instance;
        private readonly ModelTypesFactory _requestModelFactory = ModelTypesFactory.Instance;

        [Fact]
        public void CanRetrieve_Request_FromJsonDocument()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var expected = model;
                
            var jsonDocument = SerializeAndCreateJsonDocmuent(model);
            var builder = new RequestFromJsonBuilder(null);

            // act
            var request = builder.GetRequest(jsonDocument.RootElement);
            
            // assert
            request.ID.Should().Be(expected.ID);
            request.DateCreated.Should().Be(expected.DateCreated);
            request.DateLastUpdated.Should().Be(expected.DateLastUpdated);
            request.Status.Should().Be(expected.Status);
            request.Contract.ID.Should().Be(expected.Contract.ID);
            request.Applicant.ID.Should().Be(expected.Applicant.ID);
        }

        [Fact]
        public void CanRetrieve_Command_FromJsonDocument()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var expected = model.Command;
            var jsonDocument = SerializeAndCreateJsonDocmuent(model);
            var builder = new RequestFromJsonBuilder(null);

            // act
            Commands command = builder.GetCommand(jsonDocument.RootElement.GetProperty("Command"));
            
            // assert
            command.Should().Be(expected);
        }

        [Fact]
        public void CanRetrieve_Organisation_FromJsonDocument()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var expected = model.Contract.Organisation;
            var jsonDocument = SerializeAndCreateJsonDocmuent(model);
            var builder = new RequestFromJsonBuilder(null);

            // act
            var jsonElement = jsonDocument.RootElement.GetProperty("Contract").GetProperty("Organisation");
            var organisation = builder.GetOrganisation(jsonElement);
            
            // assert
            organisation.ID.Should().Be(expected.ID);
            organisation.Name.Should().Be(expected.Name);
            organisation.Description.Should().Be(expected.Description);
        }

        [Fact]
        public void CanRetrieve_Contract_FromJsonDocument()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var expected = model.Contract;
            var jsonDocument = SerializeAndCreateJsonDocmuent(model);
            var builder = new RequestFromJsonBuilder(null);

            // act
            var jsonElement = jsonDocument.RootElement.GetProperty("Contract");
            Contract contract = builder.GetContract(jsonElement);
            
            // assert
            contract.ID.Should().Be(expected.ID);
        }

        [Fact]
        public void CanRetrieve_Applicant_FromJsonDocument()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var expected = model.Applicant;
            var jsonDocument = SerializeAndCreateJsonDocmuent(model);
            var builder = new RequestFromJsonBuilder(null);
            
            // act
            var jsonElement = jsonDocument.RootElement.GetProperty("Applicant");
            Person applicant = builder.GetApplicant(jsonElement);
            
            // assert
            applicant.FirstName.Should().BeEquivalentTo(expected.FirstName);
            applicant.LastName.Should().BeEquivalentTo(expected.LastName);
            applicant.Salutation.Should().BeEquivalentTo(expected.Salutation);
            applicant.ID.Should().Be(expected.ID);
        }

        [Fact]
        public void GetProductsThrowsWithWrongArgument()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var jsonDocument = SerializeAndCreateJsonDocmuent(model);
            var builder = new RequestFromJsonBuilder(null);
            
            // act
            var jsonElement = jsonDocument.RootElement.GetProperty("Contract");
            Action act = () => builder.GetProducts(jsonElement);

            // assert
            act.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void CanRetrieve_Products_FromDocument()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var jsonDocument = SerializeAndCreateJsonDocmuent(model);
            var builder = new RequestFromJsonBuilder(null);
            
            // act
            var jsonElement = jsonDocument.RootElement.GetProperty("Contract").GetProperty("Products");
            IList<Product> products = builder.GetProducts(jsonElement);
            
            // assert
            products.Count.Should().BeGreaterThan(0);
            products[0].ID.Should().NotBe(0);
            products[0].Name.Should().NotBeNullOrEmpty();
            products[0].Description.Should().NotBeNullOrEmpty();
        }

        // helper methods
        private JsonDocument SerializeAndCreateJsonDocmuent(RequestModel model)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            options.Converters.Add(new JsonStringEnumConverter()); 
            var body = JsonSerializer.Serialize(model, options);
            
            return  JsonDocument.Parse(body);
        }
    }
}