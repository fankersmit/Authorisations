using System;
using System.ComponentModel.Design;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;
using RequestsApp.Domain;

namespace RequestsApp.Infrastructure
{
    public class RabbitMQServer
    {
        private readonly AuthorisationRequestsHandler _requestHandler;
        private readonly ILogger _logger;
        private const string HostName = "localhost";
        private const string RequestsInfo_QueueName = "rpc_queue";
        private const string RequestHandling_QueueName = "handling_queue";
        private EventingBasicConsumer _consumer;
        
        // properties
        public AuthorisationRequestsHandler RequestHandler => _requestHandler;

        // ctors
        public RabbitMQServer( ILogger<RabbitMQServer> logger, AuthorisationRequestsHandler requestHandler)
        {
            _logger = logger;
            _requestHandler = requestHandler;
        }

        public  void Run()
        {
            _logger.LogInformation("Started Server at {dateTime}", DateTime.UtcNow);
            _consumer = CreateServer(HostName); 
        }
        
        private EventingBasicConsumer CreateServer(string hostName)
        {
            var factory = new ConnectionFactory() {HostName = hostName};
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            CreateRequestsInfoQueue( channel, RequestsInfo_QueueName );
            // create more queues as program evolves
            //CreateRequestHandlingQueue( channel, RequestHandling_QueueName );
            return _consumer;
        }

        private void CreateRequestHandlingQueue(IModel channel, string requestHandlingQueueName)
        {
            throw new NotImplementedException();
        }

        private void CreateRequestsInfoQueue(IModel channel, string queueName)
        {
            channel.QueueDeclare(queue: queueName, durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue:queueName, autoAck: false, consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                string response = null;
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation($"Received message {message} at {DateTime.UtcNow}" );
                    response = RequestHandler.ResponseFor(message);
                }
                catch (Exception e)
                {
                    if( e is  ArgumentNullException || e is ArgumentException )
                    {
                        response = "";
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                        basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag,
                        multiple: false);
                }
            };
        }
    }
}