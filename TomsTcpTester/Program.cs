namespace TomsTcpTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            switch (args[0])
            {
                case "server":
                    var s = new TestServer(args[1], int.Parse(args[2]));
                    s.Run();
                    break;

                case "client":
                    var c = new TestClient(args[1], int.Parse(args[2]));
                    c.Run();
                    break;
            }
        }
    }
}