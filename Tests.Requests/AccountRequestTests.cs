using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Authorisations.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit;
using Requests.Domain;
using Requests.Shared.Domain;
using Tests.Helpers;
  
namespace Tests.Requests
{
    public class PersonConverter : JsonConverter<Person>
    {
        public override Person Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<Person>(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Person person, JsonSerializerOptions options)
        {
            writer.WriteStringValue( JsonSerializer.Serialize(person,options) );
        }
    }

    public class AccountRequestTests
    {
        readonly DomainTypesFactory factory = DomainTypesFactory.Instance;
        readonly ModelTypesFactory _requestModelFactory = ModelTypesFactory.Instance;

        [Fact]
        public void CanDeSerializeRequestFromModel()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            model.Command = Commands.Submit;
            
            // act
            var body = model.SerializeToJson<RequestModel>();
            var options = new JsonSerializerOptions()
            {
                Converters =
                {
                    new PersonConverter()
                }
            };
            var request = JsonSerializer.Deserialize<AccountRequest>(Encoding.UTF8.GetString(body), options);
            var command = body.DeSerializeFromJson<Commands>( "Command");
            
            // assert
            request.Applicant.Should().NotBeNull();
            request.Contract.Should().NotBeNull();
            request.Contract.Products.Should().NotBeNull();
            request.Should().BeOfType<AccountRequest>();
            command.Should().BeOfType<Commands>();
            command.Should().Be(Commands.Submit);
        }
        
        [Fact]
        public void NewRequestHasFieldCorrectLyInitialized()
        {
            // arrange
            var applicant = factory.CreateApplicant();
            var org = factory.CreateOrganisation();
            var contract = factory.CreateContract(org);
            var request = factory.CreateAccountRequest(applicant, contract);
            // assert
            Assert.Equal(DateTime.UtcNow.Date, request.DateCreated.Date);
            Assert.Equal(DateTime.UtcNow.Date, request.DateLastUpdated.Date);
            Assert.Equal(RequestStatus.New, request.Status);
            Assert.NotEqual(Guid.Empty, request.ID);
            Assert.Equal(string.Empty, request.Remarks);
        }

        [Fact(Skip = "tested enough for now")]
        public void CanSaveRequestToFile()
        {
            // arrange
            var ar = factory.CreateAccountRequest();
            var path = Path.GetTempPath();
            var extension = "json";
            // act
            ar.Submit();
            ar.SaveTo(path);
            var file = Path.Combine(path, $"{ar.ID}.{extension}");
            // assert
            Assert.True(File.Exists(file));
        }

        [Fact]
        public void CanSerializeToJson()
        {
            // arrange
            var ar = factory.CreateAccountRequest();
            // act
            var jsonString = ar.ToJson();
            var result = JsonSerializer.Deserialize<AccountRequest>(jsonString);
            // Assert
            Assert.IsType<AccountRequest>(result);
        }
    }
}