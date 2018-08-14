using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQSender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" 输入【\\q】退出！");
            using (var ef = new ExchangeFanoutSender("RabbitMQTestExchange"))
            {
                while (true)
                {
                    string message = Console.ReadLine();
                    if (message == "\\q")
                    {
                        break;
                    }
                    ef.SendMessage(message);
                    Console.WriteLine(" 【x】 发送 【{0}】", message);
                }
            }
        }
    }
}
