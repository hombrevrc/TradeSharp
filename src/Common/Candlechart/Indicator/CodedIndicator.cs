using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using Candlechart.Chart;
using Candlechart.Core;
using Candlechart.Interface;
using Candlechart.Series;
using Entity;
using Microsoft.CSharp;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    [LocalizedDisplayName("TitleCodedIndicator")]
    [LocalizedCategory("TitleCodedIndicator")]
    [TypeConverter(typeof(PropertySorter))]
    public class CodedIndicator : BaseChartIndicator, IChartIndicator
    {
        [Browsable(false)]
        public override string Name => Localizer.GetString("TitleCodedIndicator");

        private string code;

        [LocalizedDisplayName("TitleCodeIndicatorCode")]
        [LocalizedCategory("TitleMain")]
        [LocalizedDescription("TitleCodeIndicatorCode")]
        [Editor(typeof (FormulaUIEditor), typeof (UITypeEditor))]
        public string Code
        {
            get { return code; }
            set
            {
                code = value;
                if (!string.IsNullOrEmpty(code)) return;
                code = "public override AsteriskTooltip CheckCandleMark(List<CandleData> candles, " +
                       "List<List<double>> series, int index) \n" +
                       "{\n    return null;\n}\n\n" +

                       "public override double CalcIndcatorValue(List<CandleData> candles, " +
                       "List<List<double>> series, int index) \n" +
                       "{\n    return 0;\n}";
            }
        }

        [LocalizedDisplayName("TitleSourceSeries")]
        [Description("Серии источников-данных для индикатора")]
        [LocalizedCategory("TitleMain")]
        [Editor("Candlechart.Indicator.CheckedListBoxSeriesUITypeEditor, System.Drawing.Design.UITypeEditor",
            typeof(UITypeEditor))]
        public override string SeriesSourcesDisplay { get; set; }

        [LocalizedDisplayName("TitleColor")]
        [LocalizedDescription("MessageCurveColorDescription")]
        [LocalizedCategory("TitleVisuals")]
        [PropertyOrder(50)]
        public Color ColorLine { get; set; } = Color.DeepPink;

        private string compiledCode;

        private CodeIndicatorBase indiBuilder;

        private SeriesAsteriks seriesMarks;

        private LineSeries seriesLine;

        public CodedIndicator()
        {
            CreateOwnPanel = false;
        }

        public override BaseChartIndicator Copy()
        {
            var div = new CodedIndicator();
            Copy(div);
            return div;
        }

        public override void Copy(BaseChartIndicator indi)
        {
            var div = (CodedIndicator)indi;
            CopyBaseSettings(div);
            div.Code = Code;
            div.ColorLine = ColorLine;
        }
        
        public void BuildSeries(ChartControl chart)
        {
            owner = chart;
            EnsureProcessingFunction();
            if (indiBuilder == null) return;

            // подготовить входные данные индикатору - свечи и серии
            var candles = chart.StockSeries.Data.Candles;
            if (candles.Count == 0) return;

            var srcSeries = BuildSrcSeries(candles);

            // очистить серии
            seriesMarks.data.Clear();
            seriesLine.Data.Clear();

            // добавить маркеры
            for (var i = 0; i < candles.Count; i++)
            {
                try
                {
                    var marker = indiBuilder.CheckCandleMark(candles, srcSeries, i);
                    if (marker == null) continue;
                    marker.CandleIndex = i;
                    marker.Owner = seriesMarks;
                    seriesMarks.data.Add(marker);
                }
                catch
                {
                    continue;
                }
            }

            // график
            for (var i = 0; i < candles.Count; i++)
            {
                try
                {
                    var v = indiBuilder.CalcIndcatorValue(candles, srcSeries, i);
                    seriesLine.Data.Add(v);
                }
                catch
                {
                    continue;
                }
            }
        }

        private List<List<double>> BuildSrcSeries(List<CandleData> candles)
        {
            var srcSeries = new List<List<double>>(SeriesSources.Count);
            foreach (var sr in SeriesSources)
            {
                if (sr is CandlestickSeries)
                {
                    srcSeries.Add(((CandlestickSeries) sr).Data.Candles.Select(c => (double) c.close).ToList());
                    continue;
                }
                if (sr is LineSeries)
                {
                    srcSeries.Add(((LineSeries) sr).Data.DataArray.Select(c => c).ToList());
                    continue;
                }
                if (sr is IPriceQuerySeries)
                {
                    var lst = new List<double>();
                    for (var i = 0; i < candles.Count; i++)
                        lst.Add(((IPriceQuerySeries) sr).GetPrice(i) ?? 0);
                    srcSeries.Add(lst);
                    continue;
                }
                srcSeries.Add(candles.Select(c => 0.0).ToList());
            }
            return srcSeries;
        }

        public void Add(ChartControl chart, Pane ownerPane)
        {
            owner = chart;
            seriesMarks = new SeriesAsteriks(Localizer.GetString("TitleMarks"));
            seriesLine = new LineSeries("Line Function")
            {
                LineColor = Color.DeepPink,
                Transparent = true,
                ShiftX = 1
            };
            // инициализируем индикатор
            EntitleIndicator();
            SeriesResult = new List<Series.Series> { seriesLine, seriesMarks };
        }

        public void Remove()
        {
            seriesMarks?.data.Clear();
            seriesLine?.Data.Clear();
        }

        public void AcceptSettings()
        {
            seriesLine.LineColor = ColorLine;
        }

        public void OnCandleUpdated(CandleData updatedCandle, List<CandleData> newCandles)
        {
            if (newCandles.Count == 0) return;
            BuildSeries(owner);
        }

        public string GetHint(int x, int y, double index, double price, int tolerance)
        {
            throw new NotImplementedException();
        }

        public List<IChartInteractiveObject> GetObjectsUnderCursor(int screenX, int screenY, int tolerance)
        {
            return new List<IChartInteractiveObject>();
        }

        private void EnsureProcessingFunction()
        {
            if (Code == compiledCode) return;
            var compiledAsm = Compile();
            if (compiledAsm == null) return;

            indiBuilder = null;
            foreach (var tp in compiledAsm.GetTypes())
            {
                if (!tp.IsSubclassOf(typeof (CodeIndicatorBase))) continue;
                indiBuilder = (CodeIndicatorBase) tp.GetConstructor(new Type[0]).Invoke(new object[0]);
                break;
            }
        }

        private Assembly Compile()
        {
            if (string.IsNullOrEmpty(Code)) return null;
            try
            {
                var libCode = DecorateCode();
                var codeProvider = new CSharpCodeProvider();
                var parameters = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false };

                parameters.ReferencedAssemblies.Add("system.dll");
                parameters.ReferencedAssemblies.Add("mscorlib.dll");
                parameters.ReferencedAssemblies.Add("system.core.dll");

                AddAssemblyRef(parameters, "TradeSharp.Contract");
                AddAssemblyRef(parameters, "TradeSharp.Util");
                AddAssemblyRef(parameters, "Entity");
                AddAssemblyRef(parameters, "Candlechart");

                parameters.ReferencedAssemblies.Add(GetType().Assembly.Location);
                var results = codeProvider.CompileAssemblyFromSource(parameters, libCode);
                var compiledAsm = !results.Errors.HasErrors ? results.CompiledAssembly : null;
                if (results.Errors.HasErrors)
                {
                    foreach (CompilerError er in results.Errors)
                    {
                        var msg = $"[{er.Line - 14},{er.Column}]: {er.ErrorText}";
                        owner.Owner.DoLogMessageInStatusPane(msg);
                    }
                }
                return compiledAsm;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error compiling code: {ex}");
                return null;
            }
            finally
            {
                compiledCode = Code;
            }
        }

        private void AddAssemblyRef(CompilerParameters parameters, string nameSpace)
        {
            var nameTradeSharpContract =
                GetType().Assembly.GetReferencedAssemblies().FirstOrDefault(x => x.Name == nameSpace);
            if (nameTradeSharpContract != null)
                parameters.ReferencedAssemblies.Add(Assembly.ReflectionOnlyLoad(nameTradeSharpContract.FullName).Location);
        }

        private string DecorateCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Candlechart.Interface;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Candlechart.Chart;");
            sb.AppendLine("using Candlechart.Core;");
            sb.AppendLine("using Candlechart.Indicator;");
            sb.AppendLine("using Entity;");
            sb.AppendLine("using TradeSharp.Util;");
            sb.AppendLine("using Candlechart.Series;");

            sb.AppendLine("namespace TradeSharp.Contract.CodeIndi");
            sb.AppendLine("{");
            sb.AppendLine(" public class CodeIndiProcessor : CodeIndicatorBase");
            sb.AppendLine(" {");
            sb.AppendLine();
            sb.Append(Code);
            sb.AppendLine();
            sb.AppendLine(" }"); // end of CodeIndiProcessor
            sb.AppendLine("}"); // end of namespace
            return sb.ToString();
        }
    }
}
