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
                
                parser.OnCmdlineMatch(() =>
                {
                });

                parser.Run(args);
                // parser.Run(new[] { "match-regex", @"[,\\s]*(\\d+)%", @"battery percent is [\\\\1]"});
            }, useColors: true, unescapeArguments: false);
        }
    }
}
