using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using SearchAThing;
using System.Linq;
using static System.FormattableString;
using static System.Math;
using Avalonia;
using Avalonia.Controls;
using OxyPlot;
using OxyPlot.Avalonia;
using SearchAThing.Gui;

namespace shu
{

    public static partial class ShellUtilities
    {

        public class PlotData
        {
            public PlotData(double x, double y) { this.x = x; this.y = y; }
            public double x { get; set; }
            public double y { get; set; }
        }

        public class CsvItem
        {
            public double t { get; set; }
            public bool v { get; set; }

            public double periodSec { get; set; }

            public CsvItem prev { get; set; }
        }

        public class MainWindow : Win
        {
            PlotView pv;

            internal static string csv_pathfilename;
            internal static string chansFilter;

            public MainWindow() : base(new[]
            {
                "resm:OxyPlot.Avalonia.Themes.Default.xaml?assembly=OxyPlot.Avalonia"
            })
            {
                Title = csv_pathfilename;
                string hdr = "";
                using (var sr = new StreamReader(csv_pathfilename))
                {
                    hdr = sr.ReadLine();
                }
                var hdr_ss = hdr.Split(',');
                var cols = new List<int>();
                foreach (var chanFilter in chansFilter.Split(','))
                {
                    int idx = -1;
                    for (int i = 0; i < hdr_ss.Length; ++i)
                    {
                        if (hdr_ss[i].Trim().StripBegin('"').StripEnd('"') == chanFilter.Trim().StripBegin('"').StripEnd('"'))
                        {
                            idx = i;
                            break;
                        }
                    }
                    if (idx != -1) cols.Add(idx);
                }

                if (cols.Count == 0)
                {
                    System.Console.WriteLine($"no matching cols");
                    Environment.Exit(1);
                }

                var grRoot = new Grid() { Margin = new Thickness(10) }; this.Content = grRoot;

                pv = new PlotView();
                pv.Model = new PlotModel();
                grRoot.Children.Add(pv);

                var series = new List<OxyPlot.Series.LineSeries>();
                foreach (var col in cols)
                {
                    var csv_data = new List<CsvItem>();
                    using (var sr = new StreamReader(csv_pathfilename))
                    {
                        sr.ReadLine();
                        while (!sr.EndOfStream)
                        {
                            var str = sr.ReadLine();
                            var ss = str.Split(',');
                            csv_data.Add(new CsvItem { t = ss[0].InvDoubleParse(), v = ss[col].InvDoubleParse() != 0 });
                        }
                    }
                    CsvItem periodBegin = null;
                    foreach (var x in csv_data.WithPrev())
                    {
                        if (x.item.v)
                        {
                            if (periodBegin == null)
                            {
                                periodBegin = x.item;
                            }
                            if (x.prev != null && !x.prev.v)
                            {
                                x.item.prev = periodBegin;
                                periodBegin.periodSec = x.item.t - periodBegin.t;
                                periodBegin = x.item;
                            }
                        }
                    }

                    var speed_items = csv_data
                            .Where(r => r.periodSec > 0)
                            .Select(x => new PlotData(
                                x.t + x.periodSec / 2,
                                1.0 / x.periodSec
                                ))
                            .OrderBy(w => w.x);

                    var f1 = new OxyPlot.Series.LineSeries()
                    {
                        Title = hdr_ss[col] + $" ({speed_items.Count()}pts)",
                        DataFieldX = "x",
                        DataFieldY = "y",

                        ItemsSource = speed_items
                    };
                    pv.Model.Series.Add(f1);
                }

                pv.ResetAllAxes();
                pv.InvalidatePlot();
            }

            protected override void OnMeasureInvalidated()
            {
                base.OnMeasureInvalidated();

                pv.InvalidatePlot();
            }
        }


        public static void RegisterLogic2FreqGraph(CmdlineParser parser)
        {
            var cmdReplaceToken = parser.AddCommand("logic2-freq-graph", "display pulse freq graph", (logic2FreqGraph) =>
            {
                var _filename = logic2FreqGraph.AddMandatoryParameter("filename", "Logic2 exported csv file");
                var _columnsFilter = logic2FreqGraph.AddMandatoryParameter("columnsFilter", "filter for csv columns (eg. \"Channel 0, Channel 2\")");

                logic2FreqGraph.OnCmdlineMatch(() =>
                {
                    var pathfilename = (string)_filename;
                    if (!File.Exists(pathfilename))
                    {
                        System.Console.WriteLine($"can't find file [{pathfilename}]");
                        return;
                    }

                    var columnsFilter = (string)_columnsFilter;

                    MainWindow.csv_pathfilename = pathfilename;
                    MainWindow.chansFilter = columnsFilter;                    

                    GuiToolkit.CreateGui<MainWindow>();

                });

            });

        }

    }

}
