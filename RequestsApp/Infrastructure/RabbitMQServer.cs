using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using Requests.Shared.Domain;
using RequestsApp.Domain;

namespace RequestsApp.Infrastructure
{
    public class RabbitMQServer
    {
        private const string HostName = "localhost";
        private const string RequestsInfo_QueueName = "auth_rpc_queue";
        private const string RequestHandling_QueueName = "publish_queue";
        
        private ICommandHandler _commandHandler;
        private IQueryHandler _queryHandler;
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly Dictionary<string, IModel> _channels;
        
        // properties
        public ICommandHandler CommandHandler => _commandHandler;
        public IQueryHandler QueryHandler => _queryHandler;

        // ctors
        public RabbitMQServer( ILogger<RabbitMQServer> logger)
        {
            _logger = logger;
            _channels = new Dictionary<string, IModel>();
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
        }

        public void Run( ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
             CreateChannels();
             _commandHandler = commandHandler;
             _queryHandler = queryHandler;
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
            consumer.Received += OnCommandReceived;
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
            consumer.Received += OnQueryReceived;
            channel.BasicConsume(queue:queryHandlingQueueName, autoAck: true, consumer: consumer);
            _channels.Add(queryHandlingQueueName, channel);
        }

        private void OnQueryReceived(object model, BasicDeliverEventArgs ea)
        {
            var channel = _channels[RequestsInfo_QueueName];
            var replyProps = channel.CreateBasicProperties();
            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            replyProps.CorrelationId = props.CorrelationId;
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($"Received message {message} at {DateTime.UtcNow}");
            
            var queryDict = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
            var query = Enum.Parse<Queries>(queryDict["Query"]);
            var args = queryDict
                .Where(s => s.Key != "Query")
                .ToDictionary(d => d.Key, d => d.Value);
            // Perform the query
            var responseBytes = _queryHandler.QueryFor(query, args);

            channel.BasicPublish(
                exchange: "",
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes);
            // note the format specifier
            _logger.LogInformation($"handled {query:g} query at {DateTime.UtcNow}");
        }

        private void OnCommandReceived(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var requestBuilder = new RequestFromJsonBuilder(null);
            var jsonDocument = JsonDocument.Parse(body);
            var request = requestBuilder.GetRequest(jsonDocument.RootElement);
            var command = requestBuilder.GetCommand(jsonDocument.RootElement.GetProperty("Command"));
            _commandHandler.Handle(request, command);
            _logger.LogInformation($"handled {command.ToString()} command for request with ID {request.ID} at {DateTime.UtcNow}");
        }
    }
}
