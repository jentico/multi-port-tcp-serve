using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress serverIp;
            var serverIpStr = args.Length > 0 ? args[0] : "0.0.0.0";
            var ports = GetServicePortList();
            var ipEndPoints = new List<IPEndPoint>();

            if (!IPAddress.TryParse(serverIpStr, out serverIp))
            {
                throw new ArgumentException("Argument is not an ip address");
            }

            ipEndPoints.AddRange(ports.Select(p => new IPEndPoint(serverIp, p)));

            var tcpServer = new TcpServer(ipEndPoints);
            Console.WriteLine("Start listening on ports");
            tcpServer.Start();
        }

        public static ICollection<int> GetServicePortList()
        {
            return new[] { 11025, 11026, 11027, 11028, 11029, 11030, 11031, 11032, 11033, 11034,
                11035, 11036, 11037, 11038, 11039, 11040 };
        }
    }
}
