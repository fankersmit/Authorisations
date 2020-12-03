using Xunit;
using Requests.Domain;
using Tests.Helpers;

namespace Tests.Requests
{
    public class RequestHandlerTests
    {
        private readonly DomainTypesFactory _factory = DomainTypesFactory.Instance;
        
        [Fact]
        public void CanSubmitRequest()
        {
            // arrange
            var expectedState = RequestStatus.Submitted;
            var request = _factory.CreateAccountRequest();
            var handler = new RequestHandler<AccountRequest>();
            
            // act
            handler.Submit(request);
            
            // assert
            // test status
            // test timestamp
            // test database 
        }
        

    }
}