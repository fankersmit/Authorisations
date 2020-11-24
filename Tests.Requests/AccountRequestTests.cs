using System;
using System.IO;
using System.Text.Json;
using Xunit;
using Requests.Domain;
using Tests.Helpers;
  
namespace Tests.Requests
{
    
    public class AccountRequestTests
    {
        readonly DomainTypesFactory factory = DomainTypesFactory.Instance;
        
        [Fact]
        public void NewRequestHasFieldCorrectLyInitialized()
        {
            // arrange
            var applicant = factory.CreateApplicant();
            var org = factory.CreateOrganisation();
            var contract = factory.CreateContract(org);
            var request = factory.CreateAccountRequest(applicant, contract);
            // assert
            Assert.Equal(DateTime.Now.Date, request.DateCreated.Date);
            Assert.Equal(DateTime.Now.Date, request.DateLastUpdated.Date);
            Assert.Equal(RequestStatus.New, request.Status);
            Assert.NotEqual(Guid.Empty, request.Id);
            Assert.Equal(string.Empty, request.Remarks);
        }

        [Fact(Skip = "tested enough for now")]
        public void CanSafeRequestToFile()
        {
            // arrange
            var ar = factory.CreateAccountRequest();
            var path = Path.GetTempPath();
            var extension = "json";
            // act
            ar.Submit();
            ar.SaveTo(path);
            var file = Path.Combine(path, $"{ar.Id}.{extension}");
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