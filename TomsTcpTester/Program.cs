using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace TomsTcpTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ClientOptions, ServerOptions>(args);
            parserResult
                .WithParsed(Run)
                .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "TomsTCPTester"; //change header
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }

        public static void Run(object options)
        {
            switch (options)
            {
                case ServerOptions so:
                    var s = new TestServer(so);
                    s.Run();
                    break;

                case ClientOptions co:
                    var c = new TestClient(co);
                    c.Run();
                    break;
            }
        }
    }
}