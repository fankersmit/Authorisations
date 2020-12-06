using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using Requests.Shared.Domain;
using RequestsApp.Domain;

namespace RequestsApp.Infrastructure
{
    public class RabbitMQServer
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;
        private readonly ILogger _logger;
        private const string HostName = "localhost";
        private const string RequestsInfo_QueueName = "rpc_queue";
        private const string RequestHandling_QueueName = "publish_queue";
        private IList<EventingBasicConsumer> _consumers;
        
        // properties
        public ICommandHandler CommandHandler => _commandHandler;
        public IQueryHandler QueryHandler => _queryHandler;

        // ctors
        public RabbitMQServer( ILogger<RabbitMQServer> logger, ICommandHandler commandHandler, IQueryHandler  queryHandler)
        {
            _logger = logger;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        public  void Run()
        {
            _consumers = CreateServer(HostName); 
            _logger.LogInformation("Started Server at {dateTime}", DateTime.UtcNow);
        }
        
        private IList<EventingBasicConsumer> CreateServer(string hostName)
        {
            var consumers = new List<EventingBasicConsumer>();
            var factory = new ConnectionFactory() {HostName = hostName};
            var connection = factory.CreateConnection();
           
            consumers.Add(CreateQueryRequestsQueue( connection, RequestsInfo_QueueName ));
            
            // create more queues as program evolves
            consumers.Add(CreateHandleRequestQueue( connection , RequestHandling_QueueName ));
            return consumers;
        }

        private EventingBasicConsumer CreateHandleRequestQueue(IConnection connection, string requestHandlingQueueName)
        {
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: requestHandlingQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var request = body.DeSerializeFromJson<AccountRequest>();
                var command = body.DeSerializeFromJson<Commands>();
                _commandHandler.Handle(request, command);
                _logger.LogInformation($"handled {command.ToString()} command for request with ID {request.Id} at {DateTime.UtcNow}");
                
            };
            channel.BasicConsume(queue: requestHandlingQueueName,
                autoAck: true,
                consumer: consumer);

            _logger.LogInformation($"Created Request handling queue at {DateTime.UtcNow}");
            return consumer;
        }

        private EventingBasicConsumer CreateQueryRequestsQueue(IConnection connection, string queueName)
        {
            var channel = connection.CreateModel();
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
                    response = GetRequestsUnderConsideration(body).ToString();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation($"Received message {message} at {DateTime.UtcNow}" );
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
            
            _logger.LogInformation($"Created Request Info handling queue at {DateTime.UtcNow}");
            return consumer;
        }

        int GetRequestsUnderConsideration(byte[] body)
        {
            int requestCount;
            var message = Encoding.UTF8.GetString(body);
            var requestType = message.Substring(message.IndexOf(':')+1);
            if ( Enum.TryParse<RequestType>(requestType, true, out var rt))
            {
                 requestCount = _queryHandler.RequestsUnderConsideration( rt ); 
            }
            else
            {
                requestCount = _queryHandler.AllRequestsUnderConsideration();                      
            }

            return requestCount;
        }
        
    }
}