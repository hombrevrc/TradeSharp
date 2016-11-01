using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Candlechart.Chart;
using Candlechart.Core;
using Candlechart.Series;
using Candlechart.Theme;
using Entity;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    [DisplayName("CCI")]
    [LocalizedCategory("TitleOscillator")]
    [TypeConverter(typeof(PropertySorter))]
    public class IndicatorCCI : BaseChartIndicator, IChartIndicator
    {
        #region members

        public override BaseChartIndicator Copy()
        {
            var cci = new IndicatorCCI();
            Copy(cci);
            return cci;
        }

        public override void Copy(BaseChartIndicator indi)
        {
            var cci = (IndicatorCCI) indi;
            CopyBaseSettings(cci);
            cci.Period = Period;
            cci.ColorLine = ColorLine;
            cci.ColorBorder = ColorBorder;
            cci.series = series;
            cci.seriesBounds = seriesBounds;            
        }

        [Browsable(false)]
        public override string Name => "CCI";

        [LocalizedDisplayName("TitlePeriod")]
        [LocalizedDescription("MessageCCIPeriodDescription")]
        [LocalizedCategory("TitleMain")]
        [PropertyOrder(1)]
        public int Period { get; set; } = 14;

        [LocalizedDisplayName("TitleColor")]
        [LocalizedDescription("MessageCurveColorDescription")]
        [LocalizedCategory("TitleVisuals")]
        [PropertyOrder(50)]
        public Color ColorLine { get; set; } = Color.Teal;

        [LocalizedDisplayName("TitleBorder")]
        [LocalizedDescription("MessageBorderColorDescription")]
        [LocalizedCategory("TitleVisuals")]
        [PropertyOrder(51)]
        public Color ColorBorder { get; set; } = Color.DimGray;

        private LineSeries series;

        private PartSeries seriesBounds;

        #endregion

        public void BuildSeries(ChartControl chart)
        {
            series.Data.Clear();

            if (SeriesSources.Count == 0 || !(SeriesSources[0] is IPriceQuerySeries)) return;
            var sources = (IPriceQuerySeries)SeriesSources[0];
            var candles = SeriesSources[0] is StockSeries ? ((StockSeries)SeriesSources[0]).Data.Candles : null;

            var count = SeriesSources[0].DataCount;
            if (count == 0 || Period == 0 || count <= (2 * Period)) return;

            // границы - -100 ... 100
            seriesBounds.parts.Add(new PartSeries.Polyline(new PartSeriesPoint(1, -100M),
                                           new PartSeriesPoint(count, -100M)));
            seriesBounds.parts.Add(new PartSeries.Polyline(
                                           new PartSeriesPoint(1, 100M),
                                           new PartSeriesPoint(count, 100M)));
            seriesBounds.parts.Add(new PartSeries.Polyline(
                                           new PartSeriesPoint(1, 0),
                                           new PartSeriesPoint(count, 0)));

            // пустое начало графика
            for (var i = 0; i < Period + Period; i++)
                series.Data.Add(0);

            // заполнить массив медианных цен (TP)
            var tpArray = new float[count];
            for (var i = 0; i < count; i++)
            {
                tpArray[i] = candles == null 
                    ? (sources.GetPrice(i) ?? 0)
                    : (candles[i].high + candles[i].low + candles[i].close) / 3f;
            }                

            // заполнить массив МА
            var maArray = new float[count];
            for (var i = 0; i < Period; i++)
                maArray[i] = tpArray[i];
            for (var i = Period; i < count; i++)
            {
                // получить MA
                var ma = 0f;
                for (var j = 0; j < Period; j++)
                    ma += tpArray[i - j];
                ma /= Period;
                maArray[i] = ma;
            }

            // посчитать индикатор
            for (var i = Period + Period; i < count; i++)
            {
                var minDev = 0f;
                for (var j = 0; j < Period; j++)
                    minDev += Math.Abs(tpArray[i - j] - maArray[i - j]);
                minDev /= Period;
                float cci;
                if (minDev != 0)
                    cci = (tpArray[i] - maArray[i]) / (0.015f * minDev);
                else
                    cci = 0;
                series.Data.Add(cci);
            }
        }

        public void Add(ChartControl chart, Pane ownerPane)
        {
            owner = chart;
            series = new LineSeries("Commodity Channel Index")
            {
                LineColor = Color.Teal,
                ShiftX = 1
            };
            seriesBounds = new PartSeries("CCI Bounds") { LineStyle = LineStyle.Dot };
            SeriesResult = new List<Series.Series> {series, seriesBounds };
            EntitleIndicator();
        }

        public void Remove()
        {
        }

        public void AcceptSettings()
        {
            series.LineColor = ColorLine;
            series.Transparent = true;
            seriesBounds.LineColor = ColorBorder;
            if (DrawPane != null && DrawPane != owner.StockPane) 
                DrawPane.Title = $"{UniqueName} [{Period}]";
        }

        public void OnCandleUpdated(CandleData updatedCandle, List<CandleData> newCandles)
        {
            if (newCandles != null)
                if (newCandles.Count > 0) BuildSeries(owner);
        }

        public string GetHint(int x, int y, double index, double price, int tolerance)
        {
            return string.Empty;
        }

        public List<IChartInteractiveObject> GetObjectsUnderCursor(int screenX, int screenY, int tolerance)
        {
            return new List<IChartInteractiveObject>();
        }

        public override string GenerateNameBySettings()
        {
            string colorStr;
            GetKnownColor(ColorLine.ToArgb(), out colorStr);
            return string.Format("CCI[{0}]{1}",
                                 Period, string.IsNullOrEmpty(colorStr) ? "" : string.Format("({0})", colorStr));
        }
    }
}
