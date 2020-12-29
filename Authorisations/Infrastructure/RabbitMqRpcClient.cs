using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
namespace Authorisations.Infrastructure
{
    public class RabbitMqRpcClient: IRabbitMqClient
    {
        // client fields
        private readonly ILogger<RabbitMqDefaultClient> _logger;
        // rabbit Mq  fields
        private readonly IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IModel _rpcChannel;

        // rpc channel
        private const string _rpcQueueName = "auth_rpc_queue";

        public RabbitMqRpcClient( ILogger<RabbitMqDefaultClient> logger )
        {
            _logger = logger;
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
            _rpcChannel = CreateRpcChannel(_connection, _rpcQueueName);
        }

        IModel CreateRpcChannel( IConnection connection, string queueName)
        {
            // create and register channel
            IModel channel = connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, false, null);
            return channel;
        }

        private EventingBasicConsumer CreateConsumer(IModel channel, IBasicProperties props, BlockingCollection<byte[]> respQueue )
        {
            var consumer = new EventingBasicConsumer(channel);
            var correlationId = props.CorrelationId;
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(body);
                }
                channel.BasicCancel(consumer.ConsumerTags[0]);
            };
            return consumer;
        }

        public byte[] Call(byte[]  messageBytes )
        {
            var channel = _rpcChannel;
            // create  temp reply queue, used once
            var replyQueueName = channel.QueueDeclare(
                queue:"",
                exclusive:true,
                durable:false,
                autoDelete:true ).QueueName;

            var props = channel.CreateBasicProperties();
            // every call needs unique correlation ID
            props.CorrelationId = Guid.NewGuid().ToString();
            // every call needs unique  reply-to channel
            props.ReplyTo = replyQueueName;

            var tempQueue = new BlockingCollection<byte[]>();
            var consumer = CreateConsumer(channel, props, tempQueue);
            channel.BasicPublish("",  _rpcQueueName,  props, messageBytes); //send
            channel.BasicConsume(consumer, replyQueueName, true); //receive
            return tempQueue.Take();
        }

        public void Register()
        {
        }

        public void Deregister()
        {
            _connection.Close();
        }
    }
}
