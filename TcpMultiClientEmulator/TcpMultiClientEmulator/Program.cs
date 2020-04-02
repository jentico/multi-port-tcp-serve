using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpMultiClientEmulator
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress serverIp;
            var serverIpStr = args.Length > 0 ? args[0] : "127.0.0.1";
            var tasks = new List<Task>();

            var ports = new[] { 11025, 11026, 11027, 11028, 11029, 11030, 11031, 11032, 11033, 11034,
                11035, 11036, 11037, 11038, 11039, 11040 };

            if (!IPAddress.TryParse(serverIpStr, out serverIp))
            {
                throw new ArgumentException("Argument is not an ip address");
            }
            
            var emulator = new Program();
            var rnd = new Random();
            var terminalNum = 100;

            foreach(var port in ports)
            {
                // For each server tcp port run 5 loops
                for (var i = 0; i < 5; i++)
                {
                    var processNum = $"{terminalNum + i}: {port}";
                    // Sleep time between messages for a client terminal message frequesncy
                    var sleepTime = rnd.Next(2, 20) * 1000;
                    tasks.Add(Task.Factory.StartNew(() => { emulator.StartTcpTransactionLoop(serverIpStr, port, sleepTime, processNum); }));
                }
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Process finished");
        }

        public void StartTcpTransactionLoop(String host, int port, int sleepTime, string processNum)
        {
            var rnd = new Random();
            int messageCount = 0;
            
            while(messageCount < 30)
            {
                try
                {
                    var tcpClient = new TcpClient(host, port);
                    NetworkStream stream = tcpClient.GetStream();
                    var data = Encoding.ASCII.GetBytes(
                        $"Terminal #{processNum}: Message: {rnd.Next(100000).ToString("N0")}");
                    stream.Write(data, 0, data.Length);

                    data = new byte[1024];
                    var responseMessage = "";
                    int streamSize = stream.Read(data, 0, data.Length);
                    responseMessage = Encoding.ASCII.GetString(data, 0, streamSize);
                    Console.WriteLine($"Received: {responseMessage}");
                    
                    messageCount++;
                    stream.Close();
                    tcpClient.Close();

                    Thread.Sleep(sleepTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception {0}", ex);
                }
            }
        }
    }
}
