using System;
using System.IO;
using System.Text.RegularExpressions;
using SearchAThing;

namespace shu
{
    class Program
    {
        static void Main(string[] args)
        {
            CmdlineParser.Create("shell utils", (parser) =>
            {
                parser.AddShortLong("h", "help", "show usage", null, (item) => item.MatchParser.PrintUsage());

                ShellUtilities.RegisterReplaceToken(parser);
                ShellUtilities.RegisterMatchRegex(parser);
                ShellUtilities.RegisterAutoCPULimiter(parser);
                ShellUtilities.RegisterGraphArea(parser);
                ShellUtilities.RegisterLogic2FreqGraph(parser);
                
                parser.OnCmdlineMatch(() =>
                {
                });

                parser.Run(args);
            }, useColors: true, unescapeArguments: true);
        }
    }
}
