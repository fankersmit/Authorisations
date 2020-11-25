using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Authorisations.Infrastructure
{
    public class RabbitMqRpcClient : IRabbitMqRpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMqRpcClient()
        {
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            connection = _connectionFactory.CreateConnection();
            channel = this.connection.CreateModel();           
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };   
        }
        
        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }
       
        
        public void Post( string message, string queueName )
        {
           var body = Encoding.UTF8.GetBytes(message);
           channel.QueueDeclare(
                queue: queueName, durable: false,  false, 
                autoDelete: false, arguments: null);

            channel.BasicPublish
                (exchange: "", routingKey: queueName, 
                basicProperties: null, body: body);
        }
        
        
        public void Register()
        {
        }
        
        public void Deregister()
        {
            this.connection.Close();
        }

        public void Close()
        {
            connection.Close();
        }

        
    }
}