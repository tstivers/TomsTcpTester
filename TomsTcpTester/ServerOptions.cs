using CommandLine;

namespace TomsTcpTester
{
    [Verb("server", HelpText = "Start a server tester process")]
    public class ServerOptions
    {
        [Value(0, Required = true, HelpText = "the ip to listen on, '*' for all interfaces")]
        public string Address { get; set; }

        [Value(1, Required = true, HelpText = "the port to listen on")]
        public int Port { get; set; }

        [Option('v', "verbose", Default = false)]
        public bool Verbose { get; set; }
    }
}