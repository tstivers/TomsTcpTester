using System;
using System.Net.Sockets;
using System.Threading;

namespace TomsTcpTester
{
    public class TestClient
    {
        private readonly TcpClient _client;
        private readonly ClientOptions _options;

        public TestClient(ClientOptions options)
        {
            _options = options;
            _client = new TcpClient();

            if (_options.DisableNagle)
                _client.NoDelay = true; // disable nagle

            _client.Connect(_options.Address, _options.Port);
            Console.WriteLine($"Connected to {_client.Client.RemoteEndPoint}");
        }

        public void Run()
        {
            var r = new Random();
            var stream = _client.GetStream();
            while (_client.Connected)
            {
                var size = r.Next(_options.MinPacketSize, _options.MaxPacketSize);
                var seed = r.Next();

                var header = new byte[8];
                Array.Copy(BitConverter.GetBytes(size), 0, header, 0, 4);
                Array.Copy(BitConverter.GetBytes(seed), 0, header, 4, 4);
                stream.Write(header);

                var pr = new Random(seed);
                var payload = new byte[size];
                pr.NextBytes(payload);
                stream.Write(payload);
                Console.WriteLine($"Sent payload {{size = {size}}}");
                Thread.Sleep(_options.Delay);
            }

            Console.WriteLine("Server disconnected");
        }
    }
}