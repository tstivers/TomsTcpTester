using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TomsTcpTester
{
    public class TestServer
    {
        private readonly TcpListener _server;
        private readonly ServerOptions _options;

        public TestServer(ServerOptions options)
        {
            _options = options;
            _server = new TcpListener(_options.Address == "*" ? IPAddress.Any : IPAddress.Parse(_options.Address), _options.Port);
        }

        public void Run()
        {
            _server.Start();
            Console.WriteLine("Waiting for connections...");
            while (true)
            {
                var client = _server.AcceptTcpClient();
                var c = new TestServerConnection(client, _options);
                var t = new Thread(c.Run);
                t.Start();
            }
        }
    }
}