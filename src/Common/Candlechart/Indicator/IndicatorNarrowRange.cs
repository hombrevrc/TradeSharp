using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using Candlechart.Chart;
using Candlechart.ChartMath;
using Candlechart.Controls;
using Candlechart.Core;
using Candlechart.Series;
using Entity;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    // ReSharper disable InconsistentNaming
    [DisplayName("Сужение диапазона")]
    [LocalizedCategory("TitleTrending")]
    [TypeConverter(typeof(PropertySorter))]
    public class IndicatorNarrowRange : BaseChartIndicator, IChartIndicator
    {
        #region Основные настройки

        public override BaseChartIndicator Copy()
        {
            var macd = new IndicatorNarrowRange();
            Copy(macd);
            return macd;
        }

        public override void Copy(BaseChartIndicator indi)
        {
            var rangeIndi = (IndicatorNarrowRange) indi;
            CopyBaseSettings(rangeIndi);
            rangeIndi.NarrowPercent = NarrowPercent;
            rangeIndi.CandlesToSignal = CandlesToSignal;
            rangeIndi.MaPeriod = MaPeriod;
            rangeIndi.PointsRange = PointsRange;
            rangeIndi.LineStyle = LineStyle;
            rangeIndi.LineColor = LineColor;
        }

        [Browsable(false)]
        public override string Name => "Сужение диапазона";

        [LocalizedDisplayName("TitleNarrowPercent")]
        [LocalizedCategory("TitleMain")]
        [Description("% среднего диапазона")]
        public int NarrowPercent { get; set; } = 65;

        [LocalizedDisplayName("TitlePoints")]
        [LocalizedCategory("TitleMain")]
        [Description("Диапазон в пунктах (вместо % от среднего)")]
        public int PointsRange { get; set; }

        [LocalizedDisplayName("TitleMAPeriod")]
        [LocalizedCategory("TitleMain")]
        [Description("Период средней")]
        public int MaPeriod { get; set; } = 50;

        [LocalizedDisplayName("TitleCandleToSignal")]
        [LocalizedCategory("TitleMain")]
        [Description("Отсчитывать от N свечей с узким диапазоном")]
        public int CandlesToSignal { get; set; } = 3;

        [LocalizedDisplayName("TitleCreateDrawingPanel")]
        [LocalizedCategory("TitleMain")]
        [Description("Создавать свою панель отрисовки")]
        public override bool CreateOwnPanel { get; set; }

        #endregion

        #region Визуальные настройки

        private readonly static Color[] lineColors = new[] { Color.Red, Color.Green, Color.Blue, Color.DarkViolet, Color.DarkOrange };

        private static int curColorIndex;

        [LocalizedDisplayName("TitleLineColor")]
        [LocalizedCategory("TitleVisuals")]
        [Description("Цвет линии")]
        public Color LineColor { get; set; }

        [LocalizedDisplayName("TitleLineType")]
        [LocalizedCategory("TitleVisuals")]
        [Description("TitleLineType")]
        [Editor(typeof(TrendLineStyleUIEditor), typeof(UITypeEditor))]
        public TrendLine.TrendLineStyle LineStyle { get; set; }

        #endregion

        private SeriesAsteriks tooltipSeries;

        private List<float> averageRangeList = new List<float>();

        public IndicatorNarrowRange()
        {
            LineColor = lineColors[curColorIndex++];
            if (curColorIndex >= lineColors.Length) curColorIndex = 0;
        }

        public void BuildSeries(ChartControl chart)
        {
            tooltipSeries.data.Clear();
            averageRangeList.Clear();

            var candles = chart.StockSeries.Data.Candles;
            var minimumCandles = PointsRange > 0 ? 5 : MaPeriod + 5;
            if (candles == null || candles.Count < minimumCandles) return;

            var lastRanges = new RestrictedQueue<float>(MaPeriod);
            float targetSize = PointsRange > 0 ? DalSpot.Instance.GetAbsValue(chart.Symbol, (float) PointsRange) : 0;

            var series = 0;
            for (var i = 0; i < candles.Count; i++)
            {
                var candle = candles[i];
                var range = candle.high - candle.low;
                lastRanges.Add(range);
                if (lastRanges.Length < lastRanges.MaxQueueLength && PointsRange == 0)
                {
                    averageRangeList.Add(0);
                    continue;
                }
                var avgRange = lastRanges.Average();
                averageRangeList.Add(avgRange);

                var candlePercent = range * 100 / avgRange;
                var isNarrow = PointsRange > 0 ? range < targetSize
                    : candlePercent <= NarrowPercent;

                if (!isNarrow && series > 0)
                {
                    if (series >= CandlesToSignal)
                    {
                        // отметить, сколько процентов данная свеча составила от обычной волатильности
                        AddMark($"{candlePercent:F0}%", $"{candlePercent:F0}%", i, candle.open, false);
                    }
                    series = 0;
                    continue;
                }

                if (!isNarrow) continue;

                series++;
                if (series >= CandlesToSignal)
                {
                    AddMark($"{series}", $"{series}", i, candle.close, true);
                }
            }
        }

        private void AddMark(string name, string text, int index, float price, 
            bool inversed)
        {
            var line = new AsteriskTooltip(name, text)
            {
                ColorLine = Color.White,
                ColorText = inversed ? Color.White : Color.Black,
                ColorFill = inversed ? Color.Black : Color.White,
                CandleIndex = index,
                Price = price,
                Sign = name,
                Shape = inversed ? AsteriskTooltip.ShapeType.Круг : AsteriskTooltip.ShapeType.Квадрат,
                TransparencyText = 255
            };
            tooltipSeries.data.Add(line);
        }

        public void Add(ChartControl chart, Pane ownerPane)
        {
            owner = chart;
            // инициализируем индикатор
            tooltipSeries = new SeriesAsteriks(Name);

            SeriesResult = new List<Series.Series> { tooltipSeries };
            EntitleIndicator();
        }

        public void Remove()
        {
        }

        public void AcceptSettings()
        {
            foreach (var marker in tooltipSeries.data)
            {
                //line.LineColor = LineColor;
                //line.LineStyle = LineStyle;
            }
        }

        public void OnCandleUpdated(CandleData updatedCandle, List<CandleData> newCandles)
        {
            if (newCandles.Count > 0)
                BuildSeries(owner);
        }

        public string GetHint(int x, int y, double index, double price, int tolerance)
        {
            //var pane = owner.StockSeries.Owner;
            //var ptClient = pane.ChartToClient(new Point(x, y));
            //var pt = Conversion.ScreenToWorld(ptClient, pane.WorldRect, pane.CanvasRect);
            var candleIndex = (int) Math.Round(index); //pt.X);
            var candles = owner.StockSeries.Data.Candles;
            if (candleIndex < 0 || candleIndex >= candles.Count) return string.Empty;
            var candle = candles[candleIndex];
            var points = DalSpot.Instance.GetPointsValue(owner.Symbol, candle.high - candle.low);

            var avgRange = candleIndex < averageRangeList.Count ? averageRangeList[candleIndex] : 0;
            avgRange = DalSpot.Instance.GetPointsValue(owner.Symbol, avgRange);

            return $"H-L: {points:F1} p, range: {avgRange:F1} p";
        }

        public List<IChartInteractiveObject> GetObjectsUnderCursor(int screenX, int screenY, int tolerance)
        {
            return new List<IChartInteractiveObject>();
        }

        public override string GenerateNameBySettings()
        {
            string colorStr;
            GetKnownColor(LineColor.ToArgb(), out colorStr);
            return string.Format("Narrow Range[{0}]", colorStr);
        }
    }
    // ReSharper restore InconsistentNaming
}
