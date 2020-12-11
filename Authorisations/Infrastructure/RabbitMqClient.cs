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
    public class RabbitMqClient : IRabbitMqClient
    {
        // client fields
        private readonly ILogger<RabbitMqClient> _logger;
        // rabbitmmq  fields
        private readonly IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly Dictionary<string, IModel> _channels;
        
        // rpc channel
        private EventingBasicConsumer _rpcConsumer;
        private BlockingCollection<string> _respQueue = new BlockingCollection<string>();
        private IBasicProperties _props;
        private const string _rpcQueueName = "rpc_queue";
        
        // publish channel
        private const string _publishQueueName = "publish_queue";
        
        public RabbitMqClient( ILogger<RabbitMqClient> logger )
        {
            _logger = logger;
            // we use a different channel for rpc and publish command: submit,confirm, etc,..
            _channels = new Dictionary<string, IModel>();
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
            
            // rpc channel
            _props = CreateRpcChannel(_connection, _rpcQueueName);
            _rpcConsumer = CreateRpcConsumer(_channels[ _rpcQueueName], _props, _respQueue);
            
            // publish
            CreatePublishChannel(_connection, _publishQueueName);
        }

        private void CreatePublishChannel(IConnection connection, string publishQueueName)
        {
            // create and register channel
            IModel channel =  connection.CreateModel();
            channel.QueueDeclare(queue: publishQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channels.Add(publishQueueName, channel);
        }

        private EventingBasicConsumer CreateRpcConsumer(IModel channel, IBasicProperties props, BlockingCollection<string> respQueue )
        {
            var consumer = new EventingBasicConsumer(channel);
            var correlationId = props.CorrelationId;
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
            return consumer;
        }

        IBasicProperties CreateRpcChannel( IConnection connection, string queueName)
        { 
            // create and register channel
            IModel channel =  connection.CreateModel();
            var replyQueueName = channel.QueueDeclare().QueueName;
            _channels.Add(queueName, channel);
            // create props for consumer
            var props = channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = replyQueueName;
            return props;
        }   
        
        public string Call(byte[]  messageBytes )
        {
           _channels[ _rpcQueueName].BasicPublish(
                "",
                routingKey:  _rpcQueueName,
                basicProperties: _props,
                body: messageBytes);

            _channels[ _rpcQueueName].BasicConsume(
                consumer: _rpcConsumer,
                queue: _props.ReplyTo,
                autoAck: true);

            return _respQueue.Take();
        }
        
        public void Post( byte[] message )
        {
           _channels[_publishQueueName].BasicPublish(
                exchange: "", routingKey: _publishQueueName, 
                basicProperties: null, body: message);
           _logger.LogInformation(System.Text.Encoding.Default.GetString(message));
        }
        
        public void Register()
        {
        }
        
        public void Deregister()
        {
            foreach(  var channel in _channels )
            {
                var cnl = channel.Value;
                {
                    if (!cnl.IsClosed) cnl.Close();
                }
            }
            this._connection.Close();
        }

        public void Close()
        {
            _connection.Close();
        }
    }
}