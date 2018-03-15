using System;

namespace TcpHolePunchingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CommunicationLayer client = new CommunicationLayer("192.168.1.2",6000);
            string s = client.ReceiveData();
            Console.WriteLine("Received data from server {0}", s);
            client.SendData("Received successfully");

        }
    }
}
