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
                parser.AddShort("h", "show usage", null, (item) => item.MatchParser.PrintUsage());

                ShellUtilities.RegisterReplaceToken(parser);
                
                parser.OnCmdlineMatch(() =>
                {
                });

                parser.Run(args);
            }, useColors: true);
        }
    }
}
