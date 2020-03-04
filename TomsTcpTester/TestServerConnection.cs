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
                var read = stream.Read(header, 0, header.Length);
                if (read != 8)
                    throw new Exception("could not read entire header");

                var size = BitConverter.ToUInt32(header, 0);
                var seed = BitConverter.ToInt32(header, 4);

                var payload = new byte[size];
                read = 0;

                while (read != size)
                {
                    read += stream.Read(payload, read, payload.Length - read);
                }

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
            {
                var message = $"{ClientAddress} validation failed:\n";
                message += $"expected {string.Join(" ", expectedPayload.Select(x => x.ToString("x")))}\n\n";
                message += $"expected {string.Join(" ", payload.Select(x => x.ToString("x")))}\n\n";
                int i;
                for (i = payload.Length - 1; payload[i] == 0; i--) ;

                message += $"truncation occurred at byte {i}";

                throw new Exception(message);
            }

            Console.WriteLine($"{ClientAddress} received valid payload [{payload.Length}]");
        }
    }
}