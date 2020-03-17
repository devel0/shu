using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using SearchAThing;
using SearchAThing.Util;
using static SearchAThing.Util.Toolkit;
using System.Linq;
using static System.FormattableString;

namespace shu
{

    public static partial class ShellUtilities
    {

        public static void RegisterAutoCPULimiter(CmdlineParser parser)
        {
            var DEFAULT_mincpu_find = 50d;
            var DEFAULT_maxcpu_set = 20d;

            var cmdRegex = parser.AddCommand("cpu-autolimiter", "autolimit cpu process usage", (parserCpuLimiter) =>
            {
                var _mincpu_find = parserCpuLimiter.AddShort("mincpu-find",
                    Invariant($"minimum cpu load to match process [default={DEFAULT_mincpu_find}]"), "val");

                var _maxcpu_set = parserCpuLimiter.AddShort("maxcpu-set",
                    Invariant($"maximum cpu load to set on matched processes [default={DEFAULT_maxcpu_set}]"), "val");

                parserCpuLimiter.OnCmdlineMatch(() =>
                {
                    var mincpu_find = _mincpu_find.ArgValues.Count == 0 ? DEFAULT_mincpu_find : ((string)_mincpu_find).InvDoubleParse();
                    var maxcpu_set = _maxcpu_set.ArgValues.Count == 0 ? DEFAULT_maxcpu_set : ((string)_maxcpu_set).InvDoubleParse();

                    var task = Exec("cpustat", new[] { "1", "1" }, CancellationToken.None, false);
                    task.Wait();
                    var pid_cpu_lines = task.Result.output.Lines().Skip(1)
                        .Select(line =>
                        {
                            var ss = line.Split(" ").Select(w => w.Trim()).Where(r => r.Length > 0).ToArray();
                            var pid = ss[3];
                            var cpu = ss[0].InvDoubleParse();
                            var cmdline = ss[7];

                            return new
                            {
                                pid = pid,
                                cpu = cpu,
                                cmdline = cmdline
                            };
                        })
                        .Where(r => r.cpu >= mincpu_find)
                        .ToList();

                    foreach (var x in pid_cpu_lines)
                    {
                        Console.WriteLine($"process pid={x.pid} with cpu={x.cpu}% will limited to cpu={maxcpu_set}% [{x.cmdline}]");

                        Exec("cpulimit", new[] { "--background", "--pid", $"{x.pid}", "--limit", $"{maxcpu_set}" }, CancellationToken.None);
                    }
                });

            });

        }

    }

}
