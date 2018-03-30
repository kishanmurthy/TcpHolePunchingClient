using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpHolePunchingClient
{
    internal class CommunicationLayer
    {
        private Socket socket;
        public TcpListener tcpListener;
        public TcpClient client { get; set; }
        public Stream stream { get; set; }

        public CommunicationLayer(String ip, int port)
        { 
            client = new TcpClient();
            client.Client.ExclusiveAddressUse = false;
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Connect(ip, port);
            stream = client.GetStream();
        }

        public CommunicationLayer(NetworkStream networkStream)
        {
            stream = networkStream;
        }


        public CommunicationLayer(IPEndPoint iPEndPoint)
        {
            try
            {
                tcpListener = new TcpListener(iPEndPoint);
                tcpListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e);
            }
        }  

        public CommunicationLayer(IPEndPoint iPEndPoint,IPEndPoint remoteEndPoint)
       {
            client = new TcpClient(iPEndPoint);
            client.Connect(remoteEndPoint);
       }


        public void EnableReuseSocket()
        {
            socket.ExclusiveAddressUse = false;
        }
        public void AcceptConnection()
        {
            var socket = tcpListener.AcceptSocket();
            var stream = new NetworkStream(socket);
        }

        public String ReceiveData()
        {
            var bytes = new byte[1024];
            int received = stream.Read(bytes, 0, 1024);
            SendOk();
            return Encoding.ASCII.GetString(bytes, 0, received);
        }

        public String ReceiveData(int filesize)
        {
            var buffer = new byte[1024];
            int receivedSize = 0;
            int bytesReceived;
            StringBuilder stringBuilder = new StringBuilder(filesize);

            while (receivedSize < (filesize) && (bytesReceived = stream.Read(buffer, 0, 1024)) != 0)
            {
                String msg = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                stringBuilder.Append(msg, 0, bytesReceived);
                receivedSize += bytesReceived;
            }
            SendOk();
            return stringBuilder.ToString();
        }

        public void SendData(String str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            stream.Write(bytes, 0, bytes.Length);
            ReceiveOk();
        }

        public double[][] ReceiveDataSet(int filesize)
        {
            var buffer = new byte[1024];
            var lines = new List<double[]>();
            int receivedSize = 0;
            int bytesReceived;
            StringBuilder stringBuilder = new StringBuilder(filesize);

            while (receivedSize < (filesize) && (bytesReceived = stream.Read(buffer, 0, 1024)) != 0)
            {
                String msg = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                stringBuilder.Append(msg, 0, bytesReceived);
                receivedSize += bytesReceived;
            }
            SendOk();
            var Data = stringBuilder.ToString().Split("\n");
            for (int i = 0; i < Data.Length; i++)
            {
                string[] line = Data[i].Split(',');
                var lineValues = line.Select(e => Convert.ToDouble(e)).ToArray();
                lines.Add(lineValues);
            }
            return lines.ToArray();
        }

        public void SendDataSet(String dataSet)
        {
            int i = 0;
            int rem = dataSet.Length % 1024;
            while (i < (dataSet.Length - 1024))
            {
                byte[] msg = Encoding.ASCII.GetBytes(dataSet.Substring(i, 1024));
                stream.Write(msg, 0, 1024);
                i += 1024;
            }
            byte[] msg2 = Encoding.ASCII.GetBytes(dataSet.Substring(i, rem));
            stream.Write(msg2, 0, msg2.Length);
            ReceiveOk();
        }

        public void Close()
        {
            client.Close();
        }

        private void SendOk()
        {
            var bytes = Encoding.ASCII.GetBytes("Ok");
            stream.Write(bytes, 0, bytes.Length);
        }

        private void ReceiveOk()
        {
            var recBytes = new byte[2];
            stream.Read(recBytes, 0, recBytes.Length);

            if (!(Encoding.ASCII.GetString(recBytes) == "Ok"))
            {
                throw new Exception("Ok Not received");
            }
        }
    }
}