using System;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("127.0.0.1", 520);
            client.OnReceiveMessage += OnReceiveMessage;
            client.AsynConnect();
            while (true)
            {
                string message = Console.ReadLine();
                if (message == "\\q")
                {
                    break;
                }
                else
                {
                    client.AsynSend(message);
                }
            }
        }

        private static void OnReceiveMessage(Message message)
        {
            Console.WriteLine($"{message.Name} : {message.Content}");
        }
    }
}
