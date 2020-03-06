using CommandLine;

namespace TomsTcpTester
{
    [Verb("client", HelpText = "Start a client tester process")]
    public class ClientOptions
    {
        [Value(0, Required = true, HelpText = "the destination server name/ip to connect to")]
        public string Address { get; set; }

        [Value(1, Required = true, HelpText = "the destination port to connect to")]
        public int Port { get; set; }

        [Option("minPacketSize", Default = 1)]
        public int MinPacketSize { get; set; }

        [Option("maxPacketSize", Default = 64 * 1024)]
        public int MaxPacketSize { get; set; }

        [Option("disableNagle", Default = false)]
        public bool DisableNagle { get; set; }

        [Option('v', "verbose", Default = false)]
        public bool Verbose { get; set; }

        [Option('d', "delay", HelpText = "Delay between packets", Default = 100)]
        public int Delay { get; set; }
    }
}