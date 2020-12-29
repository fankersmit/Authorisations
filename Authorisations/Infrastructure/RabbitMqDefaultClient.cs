using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Authorisations.Infrastructure
{
    public class RabbitMqDefaultClient : IRabbitMqClient
    {
        // client fields
        private readonly ILogger<RabbitMqDefaultClient> _logger;
        // rabbit mq  fields
        private readonly IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IModel _channel;

        // publish channel
        private const string _publishQueueName = "publish_queue";
        
        public RabbitMqDefaultClient( ILogger<RabbitMqDefaultClient> logger )
        {
            _logger = logger;
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
            // publish
            _channel = CreatePublishChannel(_connection, _publishQueueName);
        }

        private IModel CreatePublishChannel(IConnection connection, string publishQueueName)
        {
            // create and register channel
            var channel =  connection.CreateModel();
            channel.QueueDeclare(queue: publishQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            return channel;
        }

        public void Post( byte[] message )
        {
           _channel.BasicPublish(
                exchange: "", routingKey: _publishQueueName, 
                basicProperties: null, body: message);
           _logger.LogInformation(System.Text.Encoding.Default.GetString(message));
        }
        
        public void Register()
        {
        }
        
        public void Deregister()
        {
            if( !_channel.IsClosed)
            {
                _channel.Close();
            }
            this._connection.Close();
        }
    }
}
