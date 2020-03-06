using System;
using System.Linq;
using System.Net.Sockets;

namespace TomsTcpTester
{
    public class TestServerConnection
    {
        private readonly TcpClient _client;
        private readonly ServerOptions _options;

        public TestServerConnection(TcpClient client, ServerOptions options)
        {
            _client = client;
            _options = options;
            Console.WriteLine($"Connection established with {ClientAddress}");
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
                var message = $"{DateTime.Now:G}\n{ClientAddress} validation failed:\n";

                var count = 0;
                for (var i = 0; i < payload.Length; i++)
                {
                    if (payload[i] != expectedPayload[i])
                    {
                        message += $"[{i}] expected {expectedPayload[i]:x2} received {payload[i]:x2}\n";
                        message += $"expected: {Convert.ToString(expectedPayload[i], 2).PadLeft(8, '0')}\n";
                        message += $"received: {Convert.ToString(payload[i], 2).PadLeft(8, '0')}\n";
                        count++;
                    }
                }

                message += $"payload length = {payload.Length}\n";
                message += $"mismatch count = {count}\n\n";

                Console.WriteLine(message);
            }

            if (_options.Verbose)
                Console.WriteLine($"{ClientAddress} received valid payload [{payload.Length}]");
        }
    }
}