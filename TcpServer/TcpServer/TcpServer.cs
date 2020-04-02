using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    public class TcpServer
    {
        private Socket[] _sockets { get; set; }
        private IPEndPoint[] _ipEndPoints { get; set; }
        private TcpListener _tcpService { get; set; }

        private ICollection<Task> Tasks = new List<Task>();

        public TcpServer(ICollection<IPEndPoint> ipEndpoints)
        {
            _ipEndPoints = ipEndpoints.ToArray();
            _sockets = new Socket[ipEndpoints.Count];
        }

        public void Start()
        {
            foreach(var ipEndpoint in _ipEndPoints)
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                socket.Bind(ipEndpoint);
                Tasks.Add(Task.Factory.StartNew(() => { PortListeningTask(socket); }));
            }

            Task.WaitAll(Tasks.ToArray());
            Console.WriteLine("Process exited");
        }

        private void PortListeningTask(Socket socket)
        {
            var data = new byte[1024];
            var sender = new IPEndPoint(IPAddress.Any, 0);
            var remoteEndPoint = (EndPoint)sender;

            try
            {
                socket.Listen(100);
                while(true)
                {
                    var senderSocket = socket.Accept();
                    var buffer = senderSocket.ReceiveFrom(data, ref remoteEndPoint);
                    var message = Encoding.ASCII.GetString(data, 0, buffer);

                    var responseMessage = "";
                    if (message.Length > 0)
                    {
                        responseMessage = $"Processed: =={message}==";
                        var reply = Encoding.ASCII.GetBytes(responseMessage);
                        senderSocket.Send(reply, 0, reply.Length, SocketFlags.None);
                    }

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
