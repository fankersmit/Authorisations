using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Xunit;
using Requests.Domain;
using Requests.Shared.Domain;
using Tests.Helpers;


namespace Tests.Requests
{
    public class AccountRequestTests
    {
        private readonly DomainTypesFactory factory = DomainTypesFactory.Instance;
        private readonly ModelTypesFactory _requestModelFactory = ModelTypesFactory.Instance;

        [Fact]
        public void CanConstituteRequestFromModel()
        {
            // arrange
            var model = _requestModelFactory.CreateRequest();
            var applicant_firstName = model.Applicant.FirstName;
            var applicant_ID = model.Applicant.ID;
            model.Command = Commands.Submit;
           
            // act
            var options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            };
            options.Converters.Add(new JsonStringEnumConverter()); 
            var body = JsonSerializer.Serialize(model, options);
            
            var requestDocument = JsonDocument.Parse(body);
            var request = Reconstitute(requestDocument);
            var command = UTF8Encoding.UTF8.GetBytes(body).DeSerializeFromJson<Commands>("Command");

            // assert
            request.Applicant.Should().NotBeNull();
            request.Applicant.ID.Should().Be(applicant_ID);
            request.Applicant.FirstName.Should().Be(applicant_firstName);
            //request.Contract.Should().NotBeNull();
            //request.Contract.Products.Should().NotBeNull();
            request.Should().BeOfType<AccountRequest>();
            command.Should().BeOfType<Commands>();
            command.Should().Be(Commands.Submit);
        }

        private AccountRequest Reconstitute(JsonDocument requestDocument) 
        {
             var result = new AccountRequest();

            PropertyInfo[] properties = typeof(AccountRequest).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Type t = property.PropertyType;
                switch (t.Name)
                {
                    case "Person":
                        var personelement = requestDocument.RootElement.GetProperty("Applicant");
                        var applicant = new Person("","","");
                        applicant.Reconstitute(personelement); 
                        property.SetValue(result, applicant);
                        break;
                    case "RequestStatus":
                        var enumValue = requestDocument.RootElement.GetProperty(property.Name).GetString();
                        property.SetValue(result, Enum.Parse<RequestStatus>(enumValue));
                        break;
                    case "Guid":
                        var guidValue = requestDocument.RootElement.GetProperty(property.Name).GetGuid();
                        property.SetValue(result, guidValue);
                        break;
                    case "String":
                        var stringValue = requestDocument.RootElement.GetProperty(property.Name).GetString();
                        property.SetValue(result, stringValue);
                        break;
                    case "DateTime":
                        var dateValue = requestDocument.RootElement.GetProperty(property.Name).GetDateTime();
                        property.SetValue(result, dateValue);
                        break;
                    case "Int32":
                        var intValue = requestDocument.RootElement.GetProperty(property.Name).GetInt32();
                        property.SetValue(result, intValue);
                        break;
                    default:
                        //throw new InvalidOperationException();
                        break;
                }
            }
            
            return result;
            
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
    }
}