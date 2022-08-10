using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Management;

namespace Test_Broadcast
{
    class Program
    {
        //  static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        static readonly UdpClient socket = new UdpClient(4446);
        static System.Net.IPHostEntry ipEntry;
        static System.Net.IPAddress[] ipAddr;
        static void Main(string[] args)
        {
            Task.Factory.StartNew(Listen_Dgram);


            //string host = System.Net.Dns.GetHostName();
            //ipEntry = System.Net.Dns.GetHostEntry(host);
            ////Send_Dgram(IPAddress.Parse("255.255.255.255"), Convert.ToString(Dns.GetHostByName(Dns.GetHostName()).AddressList.FirstOrDefault()));
            //ipAddr = ipEntry.AddressList;
            //for (int i = 0; i < ipAddr.Length; i++)
            //{
            //    if (ipAddr[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //    {

            //        //Send_Dgram(ipAddr[i], Convert.ToString(ipAddr[i]));
            //        int dot = Convert.ToString(ipAddr[i]).LastIndexOf('.');
            //        Console.WriteLine(Convert.ToString(ipAddr[i]).Substring(0, dot+1) + "255");
            //        Console.WriteLine(ipAddr[i]);
            //        Console.WriteLine();
            //    }


            //}

            Task.Delay(200);
            Task.Factory.StartNew(Send_Dgram);

            Console.ReadLine();
        }

        static void Send_Dgram()
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip;
            string message;
            bool sent_broadcast_default = false;
            string host = System.Net.Dns.GetHostName();
            ipEntry = System.Net.Dns.GetHostEntry(host);
            ipAddr = ipEntry.AddressList;

            for (int i = 0; i < ipAddr.Length; i++)
            {
                if (ipAddr[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {

                    if (Convert.ToString(ipAddr[i]) == "255.255.255.255") message = Convert.ToString(Dns.GetHostByName(Dns.GetHostName()).AddressList.FirstOrDefault());
                    else message = Convert.ToString(ipAddr[i]);

                    int dot = Convert.ToString(ipAddr[i]).LastIndexOf('.');
                    ip = new IPEndPoint(IPAddress.Parse((Convert.ToString(ipAddr[i]).Substring(0, dot+1) + "255")), 4446);
                    byte[] buf = new byte[message.Length];
                    buf = Encoding.ASCII.GetBytes(Convert.ToString(message));
                    client.Send(buf, buf.Length, ip);
                }
                else if (sent_broadcast_default == false)
                {
                    sent_broadcast_default = true;
                    ipAddr[i] = IPAddress.Parse("255.255.255.255");
                    i--;
                }
            }
        }
        
        static void Listen_Dgram()
        {
            byte[] buf = new byte[Convert.ToString(DateTime.Now).Length];
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, 4446);
                buf = socket.Receive(ref ip);
                Console.WriteLine(Encoding.ASCII.GetString(buf));
            }
            catch
            {

            }
            Listen_Dgram();
        }

    }
}
