using System;
using System.IO;
using System.Net.Mail;
using System.Text.Json;
using Requests.Domain;
using Xunit;
  
namespace Tests.Requests
{
    
    public class AccountRequestTests
    {
        [Fact]
        public void NewRequestHasFieldCorrectLyInitialized()
        {
            // arrange
            var applicant = CreateApplicant();
            var contract = CreateContract();
            // act
            var request = CreateAccountRequest(applicant, contract);
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
            var ar = CreateAccountRequest();
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
            var ar = CreateAccountRequest();
            // act
            var jsonString = ar.ToJson();
            var result = JsonSerializer.Deserialize<AccountRequest>(jsonString);
            // Assert
            Assert.IsType<AccountRequest>(result);
        }

        private Person CreateApplicant()
        {
            var fn = "Joke";
            var ln = "de Graaf";
            return new Person(fn, ln); 
        }
        

        private AccountRequest CreateAccountRequest( Person applicant, Contract contract )
        {
            return new AccountRequest(applicant, contract);
        }
        
        private AccountRequest CreateAccountRequest()
        {
            return new AccountRequest(CreateApplicant(), CreateContract());
        }

        private Contract CreateContract()
        {
            // create org
            var name = "ZorgMij";
            var description = "Voor al uw geestelijke en lichamelijke pijntjes";
            var organisation = new Organisation(name, description);
            var email = new MailAddress("joke.deGraaf@zorgmij.eu");
            var startDate = DateTime.Now.Subtract(new TimeSpan(48, 0, 0));
            var endDate = DateTime.Now.AddYears(1);
            return new Contract(organisation, email, startDate, endDate);
        }
    }
}