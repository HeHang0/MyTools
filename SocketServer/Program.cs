using System;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(520);
            server.OnReceiveMessage += OnReceiveMessage;
            server.StartServer();
            while (true)
            {
                string message = Console.ReadLine();
                if (message == "\\q")
                {
                    break;
                }
                else
                {
                    server.SendData(message);
                }
            }
        }

        private static void OnReceiveMessage(string name, string message)
        {
            Console.WriteLine($"{name} : {message}");
        }
    }
}
