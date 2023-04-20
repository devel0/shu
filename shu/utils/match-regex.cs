namespace shu
{

    public static partial class ShellUtilities
    {

        public static void RegisterMatchRegex(CmdlineParser parser)
        {
            var cmdRegex = parser.AddCommand("match-regex", "match regex groups", (parserRegex) =>
            {
                var _regex = parserRegex.AddMandatoryParameter("regex", "c# regex");
                var _fmt = parserRegex.AddMandatoryParameter("fmt", "format string ( use \\N to print Nth group in place )");

                parserRegex.OnCmdlineMatch(() =>
                {
                    var regexStr = (string)_regex;
                    var fmt = Regex.Unescape((string)_fmt);
                    using (var sr = new StreamReader(Console.OpenStandardInput()))
                    {
                        var s = sr.ReadToEnd();                        
                        // var s = "Battery 0: Unknown, 97%";
                        var rgx = new Regex(regexStr/*, RegexOptions.Singleline*/).Match(s);
                        if (rgx.Success)
                        {
                            for (int i = 1; i < rgx.Groups.Count; ++i)
                            {
                                fmt = fmt.Replace($"\\{i}", rgx.Groups[i].Value);
                            }
                            System.Console.WriteLine(fmt);
                            Environment.Exit(0);
                        }
                        Environment.Exit(1);
                    }
                });

            });

        }

    }

}
