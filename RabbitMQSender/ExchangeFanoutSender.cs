using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQSender
{
    public class ExchangeFanoutSender : IDisposable
    {
        IConnection connection;
        IModel channel;
        public ExchangeFanoutSender(string exchange)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            Connect(factory, exchange);
        }

        public ExchangeFanoutSender(string exchange, string hostName, int port, string userName, string password)
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

            channel.ExchangeDeclare(exchange: exchange, type: "fanout");
        }

        public (bool, string) SendMessage(string message)
        {
            var (IsSend, Message) = (true, "");
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "RabbitMQTestExchange",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
            }
            catch (Exception ex)
            {
                IsSend = false;
                Message = ex.Message;
            }
            return (IsSend, Message);
        }

        public void Dispose()
        {
            channel?.Close();
            connection?.Close();
        }
    }
}
