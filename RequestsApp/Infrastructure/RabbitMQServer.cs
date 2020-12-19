using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Infrastructure
{
    public class RabbitMQServer
    {
        private const string HostName = "localhost";
        private const string RequestsInfo_QueueName = "rpc_queue";
        private const string RequestHandling_QueueName = "publish_queue";
        
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly Dictionary<string, IModel> _channels;
        
        // properties
        public ICommandHandler CommandHandler => _commandHandler;
        public IQueryHandler QueryHandler => _queryHandler;

        // ctors
        public RabbitMQServer( ILogger<RabbitMQServer> logger, ICommandHandler commandHandler, IQueryHandler  queryHandler)
        {
            _logger = logger;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _channels = new Dictionary<string, IModel>();
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
        }

        public void Run( RequestDbContext context )
        {
            // wire up event handling
            _commandHandler.CommandHandled += context.OnCommandExecuted;
            
            CreateChannels(); 
            _logger.LogInformation("Started Server at {dateTime}", DateTime.UtcNow);
        }
        
        
        public void Stop()
        {
            foreach( KeyValuePair<string, IModel> entry in  _channels)
            {
                var channel = entry.Value;
                if ( channel.IsOpen ) channel.Close(); 
            }
            _connection.Close();
            _logger.LogInformation("Stopped Server at {dateTime}", DateTime.UtcNow);
        }

        private void  CreateChannels()
        {
            CreateChannelHandlingCommands( _connection , RequestHandling_QueueName );
            _logger.LogInformation($"Created Request handling queue at {DateTime.UtcNow}");
            CreateChannelHandlingQueries( _connection, RequestsInfo_QueueName );
            _logger.LogInformation($"Created Request Info handling queue at {DateTime.UtcNow}");
            // create more channels as program evolves
        }

        private void CreateChannelHandlingCommands(IConnection connection, string commandHandlingQueueName)
        {
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: commandHandlingQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnConsumerOnReceived;
            channel.CallbackException += (chann, args) =>
            {
                _logger.LogError(args.Exception, args.Exception.Message);
            };
            channel.BasicConsume(queue: commandHandlingQueueName,
                autoAck: true,
                consumer: consumer);

            _channels.Add(commandHandlingQueueName, channel);
        }

        private void CreateChannelHandlingQueries(IConnection connection, string queryHandlingQueueName)
        {
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queryHandlingQueueName, durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue:queryHandlingQueueName, autoAck: false, consumer: consumer);

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
                    _logger.LogInformation($"Received message {message} at {DateTime.UtcNow}");
                }
                catch (Exception e)
                {
                    if (e is ArgumentNullException || e is ArgumentException)
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
                _channels.Add(queryHandlingQueueName, channel);
            };

        }

        private void OnConsumerOnReceived(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var requestBuilder = new RequestFromJsonBuilder(null);
            var jsonDocument = JsonDocument.Parse(body);
            var request = requestBuilder.GetRequest(jsonDocument.RootElement);
            var command = requestBuilder.GetCommand(jsonDocument.RootElement.GetProperty("Command"));
            _commandHandler.Handle(request, command);
            _logger.LogInformation($"handled {command.ToString()} command for request with ID {request.ID} at {DateTime.UtcNow}");
        }
        
        private int GetRequestsUnderConsideration(byte[] body)
        {
            int requestCount;
            var message = Encoding.UTF8.GetString(body);
            var requestType = message.Substring(message.IndexOf('-')+1);
            if ( Enum.TryParse<RequestType>(requestType, true, out var rt))
            {
                 requestCount = _queryHandler.RequestsUnderConsideration( rt ); 
            }
            else
            {
                requestCount = _queryHandler.AllRequestsUnderConsideration();                      
            }
            _logger.LogInformation($"handled {requestType} query at {DateTime.UtcNow}");
            return requestCount;
        }
        
    }
}