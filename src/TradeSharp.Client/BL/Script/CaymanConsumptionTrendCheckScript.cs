using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Candlechart;
using Candlechart.ChartMath;
using Candlechart.Indicator;
using Entity;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    [DisplayName("Тест поглощения Каймана")]
    public class CaymanConsumptionTrendCheckScript : TerminalScript
    {
        class Trend
        {
            public int startIndex, endIndex;

            public decimal points;
        }

        private readonly float[] zzThresholds = { 2f, 1.5f };

        private readonly int[] minConsumeCandles = { 2, 3 };

        private CandleChartControl chart;

        private List<CandleData> chartCandles;

        private List<CandleData> caymanCandles;

        private readonly List<Trend> trends = new List<Trend>();

        private readonly List<int> consumeIndices = new List<int>();

        private int pointCost;

        public CaymanConsumptionTrendCheckScript()
        {
            ScriptTarget = TerminalScriptTarget.График;
            ScriptName = "Тест поглощения Каймана";
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            this.chart = chart;
            CalcStatistics();
            return string.Empty;
        }

        public override string ActivateScript(string ticker)
        {
            throw new Exception("Неверные параметры активации");
        }

        public override string ActivateScript(bool byTrigger)
        {
            throw new Exception("Неверные параметры активации");
        }

        private void CalcStatistics()
        {            
            pointCost = DalSpot.Instance.GetPrecision10(chart.Symbol);

            var append = false;
            foreach (var zzTh in zzThresholds)
            foreach (var minCandles in minConsumeCandles)
            {
                FindTrends(zzTh);
                FindConsumptions(minCandles);
                if (chartCandles.Count < 10 || caymanCandles == null || caymanCandles.Count < 10) continue;
                if (consumeIndices.Count == 0 || trends.Count == 0)
                    continue;
                CalcPercent(zzTh, minCandles, append);
                append = true;
            }
        }

        private void FindTrends(float zzThreshold)
        {
            trends.Clear();
            chartCandles = chart.chart.StockSeries.Data.Candles;
            if (chartCandles.Count < 10) return;
            var pivots = ZigZag.GetPivots(chartCandles, zzThreshold, ZigZagSource.HighLow);

            for (var i = 1; i < pivots.Count; i++)
            {
                var deltaAbs = Math.Abs(pivots[i].b - pivots[i - 1].b);
                trends.Add(new Trend
                {
                    startIndex = pivots[i - 1].a,
                    endIndex = pivots[i].a,
                    points = (decimal) deltaAbs * pointCost
                });
            }
        }

        private void FindConsumptions(int minConsumes)
        {
            var indi = chart.indicators.FirstOrDefault(i => i is IndicatorExternSeries) as IndicatorExternSeries;
            if (indi == null) return;

            caymanCandles = indi.candleSeries.Data.Candles;
            if (caymanCandles.Count == 0) return;

            var finder = new CandleConsumptionFinder(caymanCandles, minConsumes);
            var cons = finder.Search();
            consumeIndices.Clear();
            consumeIndices.AddRange(cons.Select(c => c.consumerIndex));
        }

        private void CalcPercent(float zzThreshold, int minConsumeCandles, bool append)
        {
            // тренды
            var avgTrendCandles = trends.Average(t => t.endIndex - t.startIndex);
            var longestTrend = trends.Max(t => t.endIndex - t.startIndex);
            var avgTrendPoints = trends.Average(t => t.points);
            var maxTrendPoints = trends.Max(t => t.points);
            var minTrendPoints = trends.Min(t => t.points);

            // попадания
            decimal avgDistCandles, avgDistPercent;
            CalcDistances(consumeIndices, out avgDistCandles, out avgDistPercent);

            // для сравнения - попадания равномерных отметок
            var intervalLen = (double) (chartCandles.Count - consumeIndices[0]) /(consumeIndices.Count + 1);
            var evenIndex = (double) consumeIndices[0];
            var evenIndicies = new List<int>();
            while (evenIndex < chartCandles.Count)
            {                
                evenIndicies.Add((int) Math.Round(evenIndex));
                evenIndex += intervalLen;
            }

            decimal avgEvenDistCandles, avgEvenDistPercent;
            CalcDistances(evenIndicies, out avgEvenDistCandles, out avgEvenDistPercent);

            using (var sw = new StreamWriter($"{ExecutablePath.ExecPath}\\{chart.Symbol}.txt", append))
            {
                sw.WriteLine($"------------------------------------------------------------------------");
                sw.WriteLine($"Статистика поглощений Каймана {chart.Symbol}");
                sw.WriteLine($"Поглощение {minConsumeCandles} свеч, порог ЗигЗага {zzThreshold:F1}%");
                sw.WriteLine();
                sw.WriteLine($"{trends.Count} трендов от {minTrendPoints:F0} до {maxTrendPoints:F0} пунктов");
                sw.WriteLine($"Средний тренд: {avgTrendPoints:F0} пунктов, {avgTrendCandles:F1} свеч (дней)");
                sw.WriteLine($"Самый продолжительный тренд: {longestTrend} дней");
                sw.WriteLine();
                sw.WriteLine($"Процент \"попаданий\" по поглощениям Каймана:");
                sw.WriteLine($"В среднем на {avgDistCandles:F1} свеч до завершения тренда ({avgDistPercent:F1}%)");
                sw.WriteLine($"Процент \"попаданий\", взятых с равным шагом:");
                sw.WriteLine($"В среднем на {avgEvenDistCandles:F1} свеч до завершения тренда ({avgEvenDistPercent:F1}%)");
            }
        }

        private void CalcDistances(List<int> indicies,
            out decimal avgDistCandles, out decimal avgDistPercent)
        {
            var distances = new List<int>();
            var distPercents = new List<decimal>();
            foreach (var i in indicies)
            {
                var trend = trends.FirstOrDefault(t => t.startIndex < i && i <= t.endIndex);
                if (trend == null) continue;
                var distance = trend.endIndex - i;
                var percent = distance * 100M / (trend.endIndex - trend.startIndex);
                distances.Add(distance);
                distPercents.Add(percent);
            }
            avgDistCandles = distances.Count == 0 ? 0 : distances.Average(d => (decimal)d);
            avgDistPercent = distPercents.Count == 0 ? 0 : distPercents.Average(d => d);
        }
    }
}
