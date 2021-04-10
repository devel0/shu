using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using SearchAThing;
using System.Linq;
using static System.FormattableString;
using static System.Math;

namespace shu
{

    public static partial class ShellUtilities
    {

        public class XYPt { public double x; public double y; }

        public static void RegisterGraphArea(CmdlineParser parser)
        {
            var cmdReplaceToken = parser.AddCommand("graph-area", "compute area under graph XY", (parserGraphArea) =>
            {
                var _decimalDelimiter = parserGraphArea.AddShort("d", "decimal separator (default=.)", "VAL");
                var _fieldDelimiter = parserGraphArea.AddShort("f", "field separator (default=,)", "VAL");
                var _genDxf = parserGraphArea.AddShort("x", "generate dxf lwpolyline output");
                var _filename = parserGraphArea.AddMandatoryParameter("filename", "simple XY column file");

                parserGraphArea.OnCmdlineMatch(() =>
                {
                    var genDxf = (bool)_genDxf;

                    var pathfilename = (string)_filename;
                    if (!File.Exists(pathfilename))
                    {
                        System.Console.WriteLine($"can't find file [{pathfilename}]");
                        return;
                    }

                    string dxfPathfilename = null;
                    netDxf.DxfDocument dxf = null;
                    if (genDxf)
                    {
                        dxfPathfilename = Path.Combine(
                            Path.GetDirectoryName(pathfilename), Path.GetFileNameWithoutExtension(pathfilename) + ".dxf");
                        dxf = new netDxf.DxfDocument();
                    }

                    var decDelim = (string)_decimalDelimiter;
                    var fieldDelim = (string)_fieldDelimiter;

                    if (decDelim == null) decDelim = ".";
                    if (fieldDelim == null) fieldDelim = ",";

                    var ni = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                    ni.NumberDecimalSeparator = decDelim;

                    var area = 0d;

                    var pts = new List<XYPt>();

                    using (var sr = new StreamReader(pathfilename))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine().Trim();
                            if (line.Length != 0)
                            {
                                var ss = line.Split(fieldDelim);
                                var x = double.Parse(ss[0], ni);
                                var y = double.Parse(ss[1], ni);
                                pts.Add(new XYPt() { x = x, y = y });
                            }
                        }
                    }

                    pts = pts.OrderBy(w => w.x).ToList();

                    var MINY = pts.Min(w => w.y);

                    var prevx = 0d;
                    var prevy = 0d;
                    int cnt = 0;

                    netDxf.Entities.LwPolyline lw = null;
                    if (dxf != null) lw = new netDxf.Entities.LwPolyline();

                    foreach (var pt in pts)
                    {
                        if (dxf != null) lw.Vertexes.Add(new netDxf.Entities.LwPolylineVertex(pt.x, pt.y));
                        if (cnt > 0)
                        {
                            var maxy = Max(prevy, pt.y);
                            var miny = Min(prevy, pt.y);
                            var b = (pt.x - prevx);
                            area += b * (miny - MINY) + b * (maxy - miny) / 2;
                        }

                        prevx = pt.x;
                        prevy = pt.y;

                        ++cnt;
                    }

                    System.Console.WriteLine(Invariant($"area: {area}"));

                    if (dxf != null)
                    {
                        dxf.AddEntity(lw);
                        dxf.Save(dxfPathfilename, true);
                        System.Console.WriteLine($"[{dxfPathfilename}] written.");
                    }
                });

            });

        }

    }

}
