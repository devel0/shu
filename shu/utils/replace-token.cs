using System;
using System.IO;
using System.Text.RegularExpressions;
using SearchAThing;

namespace shu
{

    public static partial class ShellUtilities
    {

        public static void RegisterReplaceToken(CmdlineParser parser)
        {
            var cmdReplaceToken = parser.AddCommand("replace-token", "replace token from given standard input (not optimized for huge files)", (parserReplaceToken) =>
            {
                var flagCSharpRegex = parserReplaceToken.AddShort("csregex", "token will treated as csharp regex");
                var _token = parserReplaceToken.AddMandatoryParameter("token", "token to search for");
                var replacement = parserReplaceToken.AddMandatoryParameter("replacement", "text to replace where token was found");

                parserReplaceToken.OnCmdlineMatch(() =>
                {
                    var token = (string)_token;
                    using (var sr = new StreamReader(Console.OpenStandardInput()))
                    {
                        var s = sr.ReadToEnd();
                        if (flagCSharpRegex)
                        {                            
                            var rgx = new Regex(token);
                            System.Console.Write(rgx.Replace(s, (string)replacement));
                        }
                        else
                            System.Console.Write(s.Replace(token, (string)replacement));
                    }
                    System.Console.WriteLine($"using regex csharp token [{token}]");
                });

            });

        }

    }

}
