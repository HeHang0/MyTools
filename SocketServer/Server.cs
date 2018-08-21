using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    public class Server
    {
        public delegate void ReceiveMessageHandler(string name, string message);
        public event ReceiveMessageHandler OnReceiveMessage;
        TcpListener listener;
        int port;
        List<ReadWriteObject> rwoList = new List<ReadWriteObject>();

        public Server(int port)
        {
            this.port = port;
        }

        public bool StartServer()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                AcceptConnect();
                OnReceiveMessage?.Invoke("ServerSuccess", listener.Server.AddressFamily.ToString());
            }
            catch (Exception e)
            {
                OnReceiveMessage?.Invoke("ServerError", e.Message);
                return false;
            }
            return true;
        }

        private void AcceptConnect()
        {
            AsyncCallback callback = new AsyncCallback(AcceptTcpClientCallback);
            listener.BeginAcceptTcpClient(callback, listener);
        }

        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener myListener = ar.AsyncState as TcpListener;
            TcpClient client = myListener.EndAcceptTcpClient(ar);
            ReadWriteObject readWriteObject = new ReadWriteObject(client);
            rwoList.Add(readWriteObject);
            readWriteObject.BeginRead(ReadCallback);
        }

        public void SendData(byte[] sendBytes)
        {
            foreach (var item in rwoList)
            {
                item.BeginWrite(SendCallback, sendBytes);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            ReadWriteObject readWriteObject = ar.AsyncState as ReadWriteObject;
            readWriteObject?.EndWrite(ar);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            ReadWriteObject readWriteObject = null;
            try
            {
                readWriteObject = ar.AsyncState as ReadWriteObject;
                int count = readWriteObject.EndRead(ar);

                if (count == 0)
                {
                    readWriteObject.Close();
                    readWriteObject = null;
                    return;
                }

                if (readWriteObject != null)
                {
                    if (count > 0)
                    {
                        DealWithFeedBack(readWriteObject, count);
                    }
                }

            }
            catch (Exception e)
            {
                OnReceiveMessage?.Invoke("ServerError", e.Message);
                if (readWriteObject != null) rwoList.Remove(readWriteObject);
            }

            if (readWriteObject != null)
            {
                try
                {
                    readWriteObject.BeginRead(ReadCallback);
                }
                catch (Exception e)
                {
                    OnReceiveMessage?.Invoke("ServerError", e.Message);
                    if (readWriteObject != null) rwoList.Remove(readWriteObject);
                }

            }
        }

        private void DealWithFeedBack(ReadWriteObject rwObj, int count)
        {
            byte[] b = new byte[count];
            Array.Copy(rwObj.ReadBytes, 0, b, 0, count);
            OnReceiveMessage?.Invoke(rwObj.RemoteEndPoint, Encoding.UTF8.GetString(b));
        }

        class ReadWriteObject
        {
            TcpClient client;
            NetworkStream netStream;
            public byte[] ReadBytes { get; private set; }

            public ReadWriteObject(TcpClient client)
            {
                this.client = client;
                netStream = client.GetStream();
                //writeBytes = new byte[client.SendBufferSize];
            }

            public string RemoteEndPoint => client?.Client.RemoteEndPoint.ToString() ?? "";

            public void BeginRead(AsyncCallback readCallback)
            {
                if (client == null)
                {
                    return;
                }
                ReadBytes = new byte[client.ReceiveBufferSize];
                netStream.BeginRead(ReadBytes,
                    0, ReadBytes.Length, readCallback, this);
            }

            public int EndRead(IAsyncResult ar)
            {
                return netStream.EndRead(ar);
            }

            public void BeginWrite(AsyncCallback readCallback, byte[] writeBytes)
            {
                if (client == null)
                {
                    return;
                }
                //writeBytes = new byte[client.SendBufferSize];
                netStream.BeginWrite(writeBytes,
                    0, writeBytes.Length, readCallback, this);
                netStream.Flush();
            }

            public void EndWrite(IAsyncResult ar)
            {
                netStream.EndWrite(ar);
            }

            public void Close()
            {
                netStream.Close();
                client.Close();
            }
        }
    }


}
