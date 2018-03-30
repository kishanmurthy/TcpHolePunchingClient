using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace TcpHolePunchingClient
{
    class TcpHole
    { 
        private TcpClient tcpClient;
        private TcpListener tcpListener;
        private bool IsSet;
        private NetworkStream networkStream;
        public TcpHole()
        {
            IsSet = false;
        }


        public void Accept(IPEndPoint iPEndPoint)
        {
            tcpListener = new TcpListener(iPEndPoint);
            tcpListener.ExclusiveAddressUse = false;
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            tcpListener.Start(1);
            
           while (!IsSet)
           {
                if (tcpListener.Pending())
                {
                    IsSet = true;
                    var socket = tcpListener.AcceptSocket();
                    networkStream = new NetworkStream(socket);
                }
                    Thread.Sleep(10);
           }

        }


        public void Connect(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            var client = new TcpClient(localEndPoint);
            client.Client.ExclusiveAddressUse = false;
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            while (!IsSet)
            {
                try
                {
                    client.Connect(remoteEndPoint);
                    IsSet = true;
                    networkStream = client.GetStream();
                }
                catch (Exception e)
                {
                    Thread.Sleep(10);
                    continue;
                }
            }

            

        }

        public NetworkStream PunchHole(IPEndPoint localEndPoint1, IPEndPoint remoteEndPoint1 , IPEndPoint localEndPoint2 , IPEndPoint remoteEndPoint2)
        {
            Thread[] threads;

            threads = new Thread[1];
            threads[1] = new Thread(() => Accept(remoteEndPoint1));
            /*
            if (localEndPoint1.ToString() != remoteEndPoint1.ToString())
            {
                threads = new Thread[4];
                threads[0] = new Thread(() => Accept(localEndPoint1));
                threads[1] = new Thread(() => Accept(remoteEndPoint1));
                threads[2] = new Thread(() => Connect(localEndPoint1, localEndPoint2));
                threads[3] = new Thread(() => Connect(localEndPoint1, remoteEndPoint2));

            }
            else
            {
                threads = new Thread[2];
                threads[0] = new Thread(() => Accept(localEndPoint1));
                threads[1] = new Thread(() => Connect(localEndPoint1, remoteEndPoint2));
            }
            */

            for (int i = 0; i < threads.Length; i++)
                threads[i].Start();

            for (int i = 0; i < threads.Length; i++)
                threads[i].Join();
            
            return networkStream;
        }

    }
}
