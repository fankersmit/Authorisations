using Xunit;
using Authorisations.Models;
using Tests.Helpers;
using FluentAssertions;

namespace Tests.Controllers
{
    public class ModelTests
    {
        private ModelTypesFactory _factory = ModelTypesFactory.Instance;
        
        [Fact]
        public void CanSerialise_PersonModel_ToJson_AndBack()
        {
            // arrange
            PersonModel  model = _factory.CreateApplicant();
            // act
            var json = model.SerializeToJson();
            var model2 = json.DeSerializeFromJson<PersonModel>();
            // assert
            Assert.Equal(model.FirstName, model2.FirstName);
            Assert.Equal(model.LastName, model2.LastName);
        }

        [Fact]
        public void CanSerialise_OrganisationModel_ToJson_AndBack()
        {
            // arrange
            OrganisationModel model = _factory.CreateOrganisation();
            // act
            var json = model.SerializeToJson<OrganisationModel>();
            var model2 = json.DeSerializeFromJson<OrganisationModel>();
            // assert
            model2.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void CanSerialise_ProductModel_ToJson_AndBack()
        {
            // arrange
            var model = _factory.CreateProduct();
            // act
            var json = model.SerializeToJson<ProductModel>();
            var model2 = json.DeSerializeFromJson<ProductModel>();
            // assert
            model2.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void CanSerialise_ContractModel_ToJson_AndBack()
        {
            // arrange
            var organisation  = _factory.CreateOrganisation();
            var model = _factory.CreateContract(organisation);
            // act
            var json = model.SerializeToJson<ContractModel>();
            var model2 = json.DeSerializeFromJson<ContractModel>();
            // assert
            model2.Should().BeEquivalentTo(model);
        }
        
        [Fact]
        public void CanSerialise_RequestModel_ToJson_AndBack()
        {
            // arrange
            var model = _factory.CreateRequest();
            // act
            var json = model.SerializeToJson<RequestModel>();
            var model2 = json.DeSerializeFromJson<RequestModel>();
            // assert
            model2.Should().BeEquivalentTo(model);
        }
    }
}