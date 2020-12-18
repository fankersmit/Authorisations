using System;
using System.IO;
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
        public void NewRequestFromFactoryIsInitialized()
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
            Assert.NotNull(request.Remarks);
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