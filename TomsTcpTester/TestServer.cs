using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TomsTcpTester
{
    public class TestServer
    {
        private readonly TcpListener _server;

        public TestServer(string ip, int port)
        {
            _server = new TcpListener(IPAddress.Parse(ip), port);
        }

        public void Run()
        {
            _server.Start();

            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                var client = _server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                var c = new TestServerConnection(client);
                var t = new Thread(c.Run);
                t.Start();
            }
        }
    }
}