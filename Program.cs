using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http;
using System.Security.Claims;

namespace peertopeer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            IPAddress localIP = IPAddress.Parse(externalIpString);

            IPAddress peerIP = IPAddress.Parse("ip from server");

            Console.WriteLine("Connecting to peer...");

            TcpClient client = new TcpClient();
            client.Connect(peerIP, 1235);

            Console.WriteLine("Connected to peer. Type a message to send.");

            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.Start();

            while (true)
            {
                string message = Console.ReadLine();

                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(name + ": " + message);
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        static void ReceiveMessages(TcpClient client)
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                NetworkStream stream = client.GetStream();
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
        }


       
    }
}
