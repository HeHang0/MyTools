using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ef = new ExchangeFanoutReceiver("RabbitMQTestExchange"))
            {
                ef.OnReceiveMessage += OnReceiveMessage;
                Console.WriteLine("按【Enter】退出！");
                Console.ReadLine();
            }
        }

        private static void OnReceiveMessage(string message)
        {
            Console.WriteLine(" 【x】 收到 【{0}】", message);
        }
    }
}
