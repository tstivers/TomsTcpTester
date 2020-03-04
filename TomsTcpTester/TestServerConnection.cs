using System;
using System.Linq;
using System.Net.Sockets;

namespace TomsTcpTester
{
    public class TestServerConnection
    {
        private readonly TcpClient _client;

        public TestServerConnection(TcpClient client)
        {
            _client = client;
        }

        private string ClientAddress
        {
            get { return _client.Client.RemoteEndPoint.ToString(); }
        }

        public void Run()
        {
            var stream = _client.GetStream();

            while (true)
            {
                // read packet from client
                var header = new byte[8];
                stream.Read(header, 0, header.Length);

                var size = BitConverter.ToUInt32(header, 0);
                var seed = BitConverter.ToInt32(header, 4);

                var payload = new byte[size];
                stream.Read(payload, 0, payload.Length);

                // validate payload
                ValidatePayload(seed, payload);
            }
        }

        private void ValidatePayload(int seed, byte[] payload)
        {
            var r = new Random(seed);
            var expectedPayload = new byte[payload.Length];
            r.NextBytes(expectedPayload);

            if (!expectedPayload.SequenceEqual(payload))
                throw new Exception($"{ClientAddress} validation failed:\nexpected {string.Join(" ", expectedPayload.Select(x => x.ToString("x")))}\nreceived {string.Join(" ", payload.Select(x => x.ToString("x")))}");

            Console.WriteLine($"{ClientAddress} received valid payload [{payload.Length}]");
        }
    }
}