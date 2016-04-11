using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using Candlechart.Chart;
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
            rangeIndi.FrameLength = FrameLength;
            rangeIndi.MaPeriod = MaPeriod;
            rangeIndi.PointsRange = PointsRange;
            rangeIndi.LineStyle = LineStyle;
            rangeIndi.LineColor = LineColor;
            rangeIndi.ChainBound = ChainBound;
        }

        [Browsable(false)]
        public override string Name => "Сужение диапазона";

        [LocalizedDisplayName("TitleNarrowPercent")]
        [LocalizedCategory("TitleMain")]
        [Description("% среднего диапазона")]
        public int NarrowPercent { get; set; } = 50;

        public enum ChainLenBound { Строго = 0, НеМеньше }

        [LocalizedDisplayName("TitleNarrowChainBound")]
        [LocalizedCategory("TitleMain")]
        [Description("Ограничить длину флетового диапазона")]
        public ChainLenBound ChainBound { get; set; }

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
        public int FrameLength { get; set; } = 4;

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

        private float avgBreakCandle, minBreakCandle, maxBreakCandle;

        private int breaksCount;

        private LineSeries seriesUp = new LineSeries("RangeUp") { Transparent = true };
        private LineSeries seriesDn = new LineSeries("RangeDn") { Transparent = true };

        public IndicatorNarrowRange()
        {
            LineColor = lineColors[curColorIndex++];
            if (curColorIndex >= lineColors.Length) curColorIndex = 0;
        }

        public void BuildSeries(ChartControl chart)
        {
            tooltipSeries.data.Clear();
            averageRangeList.Clear();
            seriesUp.Data.Clear();
            seriesDn.Data.Clear();

            var candles = chart.StockSeries.Data.Candles;
            var minimumCandles = PointsRange > 0 ? 5 : MaPeriod + 5;
            if (candles == null || candles.Count < minimumCandles) return;

            var lastRanges = new RestrictedQueue<float>(MaPeriod);
            var lastHL = new RestrictedQueue<float>(MaPeriod);
            float targetSize = PointsRange > 0 ? DalSpot.Instance.GetAbsValue(chart.Symbol, (float) PointsRange) : 0;

            var curFrame = candles.Take(FrameLength).ToList();
            lastRanges.Add(curFrame.Max(c => c.high) - curFrame.Min(c => c.low));

            seriesUp.Data.Add(candles[0].high);
            seriesDn.Data.Add(candles[0].low);
            for (var i = 0; i < FrameLength; i++)
            {
                averageRangeList.Add(0);
                seriesUp.Data.Add(candles[i].high);
                seriesDn.Data.Add(candles[i].low);
            }
            var breakCandleLengthPercents = new List<float>();
            var prevFrameIsNarrow = false;

            for (var i = FrameLength; i < candles.Count; i++)
            {
                var candle = candles[i];
                curFrame.RemoveAt(0);
                curFrame.Add(candle);

                var high = curFrame[0].high;
                var low = curFrame[0].low;
                foreach (var c in curFrame.Skip(1))
                {
                    if (c.high > high) high = c.high;
                    if (c.low < low) low = c.low;
                }
                var range = high - low;
                lastRanges.Add(range);
                lastHL.Add(candle.high - candle.low);

                if (lastRanges.Length < lastRanges.MaxQueueLength && PointsRange == 0)
                {
                    averageRangeList.Add(0);
                    seriesUp.Data.Add(candles[i].high);
                    seriesDn.Data.Add(candles[i].low);
                    continue;
                }
                var avgRange = lastRanges.Average();
                averageRangeList.Add(avgRange);
                seriesUp.Data.Add(candles[i].close + range);
                seriesDn.Data.Add(candles[i].close - range);

                var avgHl = lastHL.Average();
                var rangePercent = avgRange == 0 ? 0 : range * 100 / avgRange;                
                var isNarrow = PointsRange > 0 ? range < targetSize
                    : rangePercent <= NarrowPercent;

                if (prevFrameIsNarrow) // && !isNarrow)
                {
                    // отметить, сколько процентов данная свеча составила от обычной волатильности
                    var candlePercent = avgHl == 0 ? 0 : (candle.high - candle.low)*100/avgHl;
                    if (candlePercent > 0)
                        breakCandleLengthPercents.Add(candlePercent);
                    AddMark($"{candlePercent:F0}%", $"{candlePercent:F0}%", i, candle.open, false);
                }
                prevFrameIsNarrow = isNarrow;                
            }

            // вывести в лог среднее
            if (breakCandleLengthPercents.Count > 0)
            {
                avgBreakCandle = breakCandleLengthPercents.Average();
                minBreakCandle = breakCandleLengthPercents.Min();
                maxBreakCandle = breakCandleLengthPercents.Max();
                breaksCount = breakCandleLengthPercents.Count;
                Logger.Info($"{breakCandleLengthPercents.Count}, от {minBreakCandle:F1} до {maxBreakCandle:F1}, среднее: {avgBreakCandle:F1}");
            }
        }

        //public void BuildSeries2(ChartControl chart)
        //{
        //    tooltipSeries.data.Clear();
        //    averageRangeList.Clear();

        //    var candles = chart.StockSeries.Data.Candles;
        //    var minimumCandles = PointsRange > 0 ? 5 : MaPeriod + 5;
        //    if (candles == null || candles.Count < minimumCandles) return;

        //    var lastRanges = new RestrictedQueue<float>(MaPeriod);
        //    float targetSize = PointsRange > 0 ? DalSpot.Instance.GetAbsValue(chart.Symbol, (float)PointsRange) : 0;

        //    var series = 0;
        //    for (var i = 0; i < candles.Count; i++)
        //    {
        //        var candle = candles[i];
        //        var range = candle.high - candle.low;
        //        lastRanges.Add(range);
        //        if (lastRanges.Length < lastRanges.MaxQueueLength && PointsRange == 0)
        //        {
        //            averageRangeList.Add(0);
        //            continue;
        //        }
        //        var avgRange = lastRanges.Average();
        //        averageRangeList.Add(avgRange);

        //        var candlePercent = range * 100 / avgRange;
        //        var isNarrow = PointsRange > 0 ? range < targetSize
        //            : candlePercent <= NarrowPercent;

        //        if (!isNarrow && series > 0)
        //        {
        //            if (series >= CandlesToSignal)
        //            {
        //                // отметить, сколько процентов данная свеча составила от обычной волатильности
        //                AddMark($"{candlePercent:F0}%", $"{candlePercent:F0}%", i, candle.open, false);
        //            }
        //            series = 0;
        //            continue;
        //        }

        //        if (!isNarrow) continue;

        //        series++;
        //        if (series >= CandlesToSignal)
        //        {
        //            AddMark($"{series}", $"{series}", i, candle.close, true);
        //        }
        //    }
        //}

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

            SeriesResult = new List<Series.Series> { tooltipSeries, seriesUp, seriesDn };
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
            var candleIndex = (int) Math.Round(index);
            var candles = owner.StockSeries.Data.Candles;
            if (candleIndex < 0 || candleIndex >= candles.Count) return string.Empty;

            var avgRange = candleIndex < averageRangeList.Count ? averageRangeList[candleIndex] : 0;
            avgRange = DalSpot.Instance.GetPointsValue(owner.Symbol, avgRange);

            if (candleIndex < FrameLength)
            {
                var candle = candles[candleIndex];
                var points = DalSpot.Instance.GetPointsValue(owner.Symbol, candle.high - candle.low);
                return $"H-L: {points:F1} p, range: {avgRange:F1} p";
            }

            var frame = candles.Skip(candleIndex - FrameLength + 1).Take(FrameLength).ToList();
            var min = frame.Min(c => c.low);
            var max = frame.Max(c => c.high);
            var curRangePoints = DalSpot.Instance.GetPointsValue(owner.Symbol, max - min);
            return $"Range: {curRangePoints:F1} p, avg. range: {avgRange:F1} p" + Environment.NewLine +
                $"Свечи [{breaksCount}]: {avgBreakCandle:F1} сред., от {minBreakCandle:F1} до {maxBreakCandle:F1}";
        }

        public List<IChartInteractiveObject> GetObjectsUnderCursor(int screenX, int screenY, int tolerance)
        {
            return new List<IChartInteractiveObject>();
        }

        public override string GenerateNameBySettings()
        {
            string colorStr;
            GetKnownColor(LineColor.ToArgb(), out colorStr);
            return string.Format("Узкий диапазон [{0}]", colorStr);
        }
    }
    // ReSharper restore InconsistentNaming
}
