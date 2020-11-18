using System;
using System.IO;
using System.Text.Json;
using Authorisations.Models;
using Xunit;

namespace Tests.Models
{
    public class AccountRequestTests
    {
        [Fact(Skip="tested enough for now")]
        public void CanSafeRequestToFile()
        { 
            // arrange
            var ar = CreateAccountRequest();
            var path = Path.GetTempPath();
            var extension = "json";
            // act
            ar.Submit();
            ar.SaveTo(path);
            var file = Path.Combine(path,$"{ar.RequestID}.{extension}");
            // assert
            Assert.True( File.Exists(file));
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

        private AccountRequest CreateAccountRequest()
        {
            var fn = "Joke";
            var ln = "de Graaf";
            var contract = CreateContract();
            var startDate = DateTime.Now.Date;
            return new AccountRequest(fn, ln,contract);    
        }

        private Contract CreateContract()
        {
            // create org
            var name = "ZorgMij";
            var description = "Voor al uw geestelijke en lichamelijke pijntjes";
            var organisation = new Organisation(name, description);
            var email = "joke.deGraaf@zorgmij.eu";
            var startDate = DateTime.Now.Subtract(new TimeSpan(48, 0, 0));
            var endDate = DateTime.Now.AddYears(1);
            return new Contract(organisation, email,startDate, endDate); 
        }
    }
}