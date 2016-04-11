using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Candlechart;
using Candlechart.ChartMath;
using Candlechart.Series;
using Entity;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    [DisplayName("Сигнал от узкого диапазона")]
    public class NarrowRangeMarkerScript : TerminalScript
    {
        [DisplayName("Свечей в шаблоне")]
        [Category("Основные")]
        [Description("Количество свечей во флетовом фрейме")]
        public int FrameLength { get; set; } = 4;

        [DisplayName("Период СС")]
        [Category("Основные")]
        [Description("Количество фреймов / свеч для оценки")]
        public int MaPeriod { get; set; } = 50;

        [DisplayName("Период Моментума")]
        [Category("Основные")]
        [Description("Период Моментума для оценки направления входа")]
        public int MomentumPeriod { get; set; } = 14;

        [LocalizedDisplayName("TitleNarrowPercent")]
        [LocalizedCategory("TitleMain")]
        [Description("% среднего диапазона")]
        public int NarrowPercent { get; set; } = 50;

        private CandleChartControl chart;

        private float pointCost;

        private readonly List<float> pointsSum = new List<float>();

        public NarrowRangeMarkerScript()
        {
            ScriptTarget = TerminalScriptTarget.График;
            ScriptName = "Сигнал от узкого диапазона";
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            this.chart = chart;
            pointsSum.Clear();
            pointCost = DalSpot.Instance.GetPointsValue(chart.Symbol, 1f);
            PlaceMarkers();
            // подбить результат
            var avgPoints = pointsSum.Count == 0 ? 0 : pointsSum.Average();
            var sumPoints = pointsSum.Sum();
            return $"{sumPoints:F1} пунктов всего, в среднем {avgPoints:F1}";
        }

        public override string ActivateScript(string ticker)
        {
            throw new Exception("Неверные параметры активации");
        }

        public override string ActivateScript(bool byTrigger)
        {
            throw new Exception("Неверные параметры активации");
        }

        private void PlaceMarkers()
        {
            var candles = chart.chart.StockSeries.Data.Candles;
            if (candles.Count <= FrameLength + MaPeriod) return;

            var lastRanges = new RestrictedQueue<float>(MaPeriod);

            var curFrame = candles.Take(FrameLength).ToList();
            lastRanges.Add(curFrame.Max(c => c.high));

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

                if (lastRanges.Length < lastRanges.MaxQueueLength)
                    continue;
                var avgRange = lastRanges.Average();

                var rangePercent = avgRange == 0 ? 0 : range * 100 / avgRange;
                var isNarrow = rangePercent <= NarrowPercent;

                if (prevFrameIsNarrow) // && !isNarrow)
                {
                    if (i <= MomentumPeriod) continue;
                    var prevClose = candles[i - MomentumPeriod - 1].close;
                    if (prevClose <= 0) continue;
                    var momentum = candles[i - 1].close*100 / prevClose;
                    var side = momentum > 100 ? -1 : 1;

                    AddMark(side, i, candles, $"M: {momentum:F1}");
                }
                prevFrameIsNarrow = isNarrow;
            }            
        }

        private void AddMark(int side, int index, List<CandleData> candles, string text)
        {
            var candle = candles[index];
            var priceOpen = candle.open;
            var priceClose = candle.close;

            var profitPoints = side * (priceClose - priceOpen) * pointCost;
            pointsSum.Add(profitPoints);
            var openMarker = new DealMarker(chart.chart, chart.seriesMarker.data, DealMarker.DealMarkerType.Вход,
                side > 0 ? DealType.Buy : DealType.Sell, index, priceOpen, candle.timeOpen)
            {
                Comment = text,
                ColorText = profitPoints > 0 ? Color.DarkGreen : Color.Maroon                
            };
            chart.seriesMarker.data.Add(openMarker);
            var closeMarker = new DealMarker(chart.chart, chart.seriesMarker.data,
                DealMarker.DealMarkerType.Выход,
                side > 0 ? DealType.Buy : DealType.Sell, index + 1, priceClose, candle.timeClose)
            {
                Comment = text,
                exitPair = openMarker.id
            };
            chart.seriesMarker.data.Add(closeMarker);
        }
    }
}
