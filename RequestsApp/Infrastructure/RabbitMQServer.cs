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
        private readonly ILogger _logger;
        private const string HostName = "localhost";
        private const string RequestsInfo_QueueName = "rpc_queue";
        private const string RequestHandling_QueueName = "publish_queue";
        private IList<EventingBasicConsumer> _consumers;
        private readonly Random _random; 
        
        
        // properties
        public ICommandHandler CommandHandler => _commandHandler;

        // ctors
        public RabbitMQServer( ILogger<RabbitMQServer> logger, ICommandHandler commandHandler)
        {
            _logger = logger;
            _commandHandler = commandHandler;
            
            // phony return value creation, remove after implementing Query handling 
            _random = new Random( DateTime.Now.Minute);          
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
           
            consumers.Add(CreateRequestsInfoQueue( connection, RequestsInfo_QueueName ));
            
            // create more queues as program evolves
            consumers.Add(CreateRequestHandlingQueue( connection , RequestHandling_QueueName ));
            return consumers;
        }

        private EventingBasicConsumer CreateRequestHandlingQueue(IConnection connection, string requestHandlingQueueName)
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
                var message = Encoding.UTF8.GetString(body);

                // retrieve request and command from queue
                var request = body.DeSerializeFromJson<AccountRequest>();
                var command = body.DeSerializeFromJson<Commands>();
                // handle message
                _commandHandler.Handle(request, command);
                //_requestHandler.ProcessMessage(message);
                _logger.LogInformation($"handled {command.ToString()} command for reuqest with ID {request.Id} at {DateTime.UtcNow}");
                
            };
            channel.BasicConsume(queue: requestHandlingQueueName,
                autoAck: true,
                consumer: consumer);

            _logger.LogInformation($"Created Request handling queue at {DateTime.UtcNow}");
            return consumer;
        }

        private EventingBasicConsumer CreateRequestsInfoQueue(IConnection connection, string queueName)
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
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation($"Received message {message} at {DateTime.UtcNow}" );
                    response =  $"{message}:{_random.Next(0,40000)}";
                    //response = CommandHandler.ResponseFor(message);
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
    }
}