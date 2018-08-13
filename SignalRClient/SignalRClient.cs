using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRClient
{
    public class SignalRClient
    {
        HubConnection connection;
        public delegate void ReceiveMessageHandler(string name, string message);
        public event ReceiveMessageHandler OnReceiveMessage;
        private string name;
        public SignalRClient(string url, string name)
        {
            this.name = name;
            connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
            Connect();
            connection.StartAsync();
        }

        private void Connect()
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                OnReceiveMessage?.Invoke(user, message);
            });
        }

        public async Task<(bool isSend, string info)> SendMessageAsync(string message)
        {
            var (isSend, info) = (false, "");
            try
            {
                await connection.InvokeAsync("SendMessage",
                        name, message);
                isSend = true; info = "Success";
            }
            catch (Exception ex)
            {
                isSend = false;
                info = ex.Message;
            }
            return (isSend, info);
        }
    }
}
