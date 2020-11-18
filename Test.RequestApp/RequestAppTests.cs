using Xunit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;
using RequestsApp.Infrastructure;
using RequestsApp.Domain;

namespace Test.RequestApp
{
    public class RequestAppTests
    {
        [Fact]
        public void CanInjectRequestHandlerIntoBroker()
        {
            // Arrange
            var requestHandler = new AuthorisationRequestsHandler();
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<RabbitMQServer>();

            // Act
            var rabbitMQServer = new RabbitMQServer(logger);
            rabbitMQServer.Run(requestHandler);
            var handler = rabbitMQServer.RequestHandler; 
            // Assert
            Assert.NotNull(handler);
            Assert.Equal(requestHandler, handler);
        }
    }
}