using System;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入用户名：");
            var name = Console.ReadLine();
            SignalRClient src = new SignalRClient("https://localhost:44368/chatHub", name);
            src.OnReceiveMessage += OnReceiveMessage;
            Console.WriteLine("输入\\q退出");
            string messgae = "";
            while (true)
            {
                //Console.Write($"{name} : ");
                messgae = Console.ReadLine();
                if (messgae == "\\q")
                {
                    break;
                }
                Task<(bool isSend, string info)> task =  src.SendMessageAsync(messgae);
                task.ContinueWith(r =>
                {
                    if (!r.Result.isSend)
                    {
                        Console.WriteLine(r.Result.info);
                    }
                });
            }
        }

        private static void OnReceiveMessage(string name, string message)
        {
            Console.WriteLine($"{name} : {message}");
        }
    }
}
