using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Google.Protobuf;

namespace SocketClient
{
    public class Client
    {
        public delegate void ReceiveMessageHandler(Message message);
        public event ReceiveMessageHandler OnReceiveMessage;
        IPEndPoint serverIp;
        Socket tcpClient;

        public Client(string ip, int port)
        {
            if (!Regex.IsMatch(ip, "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)"))
            {
                throw new ArgumentException("IP地址格式错误。。。");
            }
            if (port < 0 || port > 65535)
            {
                throw new ArgumentException("端口号格式错误。。。");
            }
            serverIp = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #region 异步连接
        /// <summary>
        /// Tcp协议异步连接服务器
        /// </summary>
        public Client AsynConnect()
        {
            //主机IP  
            tcpClient.BeginConnect(serverIp, asyncResult =>
            {
                tcpClient.EndConnect(asyncResult);
                OnReceiveMessage?.Invoke(new Message() { Name= "Client", Content=$"Connected {serverIp.ToString()}" });
                AsynRecive();
            }, null);
            return this;
        }
        #endregion

        #region 异步接受消息
        /// <summary>
        /// 异步连接客户端回调函数
        /// </summary>
        /// <param name="tcpClient"></param>
        private void AsynRecive()
        {
            byte[] data = new byte[2048];
            tcpClient.BeginReceive(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                int length = tcpClient.EndReceive(asyncResult);
                if (length > 0)
                {
                    Message message = Message.Parser.ParseFrom(data, 0, length);
                    OnReceiveMessage?.Invoke(message);
                }                
                AsynRecive();
            }, null);
        }
        #endregion


        #region 异步发送消息
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="tcpClient">客户端套接字</param>
        /// <param name="message">发送消息</param>
        public void AsynSend(string message)
        {
            Message m = new Message
            {
                Name = tcpClient.LocalEndPoint.ToString(),
                Content = message
            };
            byte[] sendBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                // Save the person to a stream
                m.WriteTo(stream);
                sendBytes = stream.ToArray();
            }
            tcpClient.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, asyncResult =>
            {
                //完成发送消息
                int length = tcpClient.EndSend(asyncResult);
                OnReceiveMessage?.Invoke(new Message() { Name = "Client", Content = message });
            }, null);
        }
        #endregion

        public void Dispose()
        {
            tcpClient?.Close();
            tcpClient?.Dispose();
            tcpClient = null;
        }
    }
}
