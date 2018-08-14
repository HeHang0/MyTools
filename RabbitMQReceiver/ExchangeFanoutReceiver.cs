using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQReceiver
{
    public class ExchangeFanoutReceiver:IDisposable
    {
        public delegate void ReceiveMessageHandler(string message);
        public event ReceiveMessageHandler OnReceiveMessage;
        IConnection connection;
        IModel channel;
        public ExchangeFanoutReceiver(string exchange)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            Connect(factory, exchange);
        }

        public ExchangeFanoutReceiver(string exchange, string hostName, int port, string userName, string password)
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };
            Connect(factory, exchange);
        }

        private void Connect(ConnectionFactory factory, string exchange)
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchange,
                                    type: "fanout");
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                                    exchange: exchange,
                                    routingKey: "");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                OnReceiveMessage?.Invoke(Encoding.UTF8.GetString(ea.Body));
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void Dispose()
        {
            channel?.Close();
            connection?.Close();
        }
    }
}
