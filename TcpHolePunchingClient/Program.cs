using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace TcpHolePunchingClient
{
    class Program
    {
        static void Main(string[] args)
        {

            CommunicationLayer client = new CommunicationLayer("192.168.1.2", 6000);
            var localEndPoint1 = client.client.Client.LocalEndPoint as IPEndPoint;
            client.SendData(localEndPoint1.ToString());


            string remoteEndPoint1String = client.ReceiveData();
            var remoteEndPoint1 = GetIpEndPoint(remoteEndPoint1String);
            string remoteEndPoint2String = client.ReceiveData();
            var remoteEndPoint2 = GetIpEndPoint(remoteEndPoint2String);
            string localEndPoint2String = client.ReceiveData();
            var localEndPoint2 = GetIpEndPoint(localEndPoint2String);

            Console.WriteLine("Received data from server {0}", remoteEndPoint1String);
            Console.WriteLine("Received data from server {0}", remoteEndPoint2String);
            Console.WriteLine("Received data from server {0}", localEndPoint2String);
            Console.WriteLine("Local End Point {0}", localEndPoint1);


            TcpHole tcpHole = new TcpHole();
            var stream = tcpHole.PunchHole(localEndPoint1, remoteEndPoint1, localEndPoint2, remoteEndPoint2);



            /*
            client.client.Client.ExclusiveAddressUse = false;


            var ip = remoteEndPoint2.Split(':')[0];
            var ipByte = ip.Split('.').Select(i => Convert.ToByte(i)).ToArray();
            var port = int.Parse(remoteEndPoint2.Split(':')[1]);
            var ipEND = new IPEndPoint(new IPAddress(ipByte), port);
            */








            /*
            int count = 0;
            CommunicationLayer cli;
            CommunicationLayer ser;

            while (true)
            {
                try
                {

                    var ip = s.Split(':')[0];
                    var ipByte = ip.Split('.').Select(i => Convert.ToByte(i)).ToArray();
                    var port = int.Parse(s.Split(':')[1]);
                    var ipEND = new IPEndPoint(new IPAddress(ipByte), port);

                     cli = new CommunicationLayer(localEndPoint, ipEND);
                    var tmp = cli.ReceiveData();
                    Console.WriteLine(tmp);
                    break;
                }
                catch (Exception ex)
                {
                    cli.Close();
                    count++;
                    var rnd = new Random();
                    Thread.Sleep(rnd.Next(100,200));
                }


                if(count>4)
                {

                    try
                    {
                        ser = new CommunicationLayer(localEndPoint);
                        ser.AcceptConnection();

                        ser.SendData("Connection established hurray from" + localEndPoint.ToString());
                    }
                    catch (Exception ex)
                    {
                        ser.Close();
                        count++;
                        var rnd = new Random();
                        Thread.Sleep(rnd.Next(100, 200));
                    }
                }


            }
            Console.ReadLine();
        }



    */


        }

        public static IPEndPoint GetIpEndPoint(string ipEndPointString)
        {
            var localEndPointList = ipEndPointString.Split(':');
            var ipAddress = localEndPointList[0].Split('.').Select(i => Convert.ToByte(i)).ToArray();
            return new IPEndPoint(new IPAddress(ipAddress), int.Parse(localEndPointList[1]));

        }
    }



 
}