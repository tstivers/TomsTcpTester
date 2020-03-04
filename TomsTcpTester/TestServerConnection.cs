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
                var message = $"\n\n\n{DateTime.Now:G}{ClientAddress} validation failed:\n";
                //message += $"expected {string.Join(" ", expectedPayload.Select(x => x.ToString("x2")))}\n\n";
                //message += $"received {string.Join(" ", payload.Select(x => x.ToString("x2")))}\n\n";
                int i;
                int firstOffset = 0;
                var count = 0;

                for (i = 0; i < payload.Length; i++)
                {
                    if (payload[i] != expectedPayload[i])
                    {
                        message += $"[{i}] expected {expectedPayload[i]:x2} received {payload[i]:x2}\n";
                        count++;
                        if (firstOffset == 0)
                            firstOffset = i;
                    }
                }

                message += $"\npayload length = {payload.Length}\n";
                message += $"mismatch count = {count}\n";

                Console.WriteLine(message);
                //throw new Exception(message);
            }

            //Console.WriteLine($"{ClientAddress} received valid payload [{payload.Length}]");
        }
    }
}