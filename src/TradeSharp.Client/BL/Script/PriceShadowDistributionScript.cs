using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Candlechart;
using Candlechart.ChartMath;
using Entity;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    [DisplayName("Вероятность движения / тень")]
    public class PriceShadowDistributionScript : TerminalScript
    {
        private CandleChartControl chart;

        private List<CandleData> minuteCandles, dailyCandles;

        private Dictionary<int, List<decimal>> tailLengthByHour;

        private int pointCost;

        private DateTime startTime;

        private readonly int[] tailHours = {1, 2, 4, 8, 12};

        private readonly int[] pointsScales = { 10, 20, 30, 40, 50, 60, 70 };

        private decimal avgBody, avgHL;

        public PriceShadowDistributionScript()
        {
            ScriptTarget = TerminalScriptTarget.График;
            ScriptName = "Вероятность движения / тень";
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            this.chart = chart;
            startTime = chart.chart.StockSeries.GetCandleOpenTimeByIndex((int)worldCoords.X);
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
            PackCandles();
            if (dailyCandles.Count < 10) return;

            var fileName = ExecutablePath.ExecPath + $"\\price_shadow_distr_{chart.Symbol}.txt";
            using (var sw = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                sw.WriteLine($"{chart.Symbol} с {minuteCandles[0].timeOpen:yyyy-MM-dd HH:mm}");
                avgBody = (decimal) dailyCandles.Average(c => Math.Abs(c.close - c.open)) * pointCost;
                avgHL = (decimal) dailyCandles.Average(c => c.high - c.low) * pointCost;
                sw.WriteLine($"Среднее O-C: {avgBody:F1} пунктов, среднее H-L: {avgHL:F1} пунктов");
                
                // оцениваем свечу на момент закрытия
                var tailLengths =
                    dailyCandles.Select(
                        c => (decimal) (c.open < c.close ? (c.open - c.low) : (c.high - c.open)) * pointCost).ToList();
                CalculateCommon(sw, tailLengths);

                // оцениваем свечи по хвостам на 1-й, 2-й, 4-й 8-й и 12-й час после открытия
                foreach (var tailLenList in tailLengthByHour.OrderBy(l => l.Key))
                {
                    sw.WriteLine("----------------------------------------------------------------------");
                    sw.WriteLine($"Оценка по хвосту на {tailLenList.Key}-й час после открытия");
                    CalculateCommon(sw, tailLenList.Value);
                }
            }
        }

        private void PackCandles()
        {
            // длина хвостика, измеренная на 1-й, второй, ... час
            tailLengthByHour = tailHours.ToDictionary(h => h, h => new List<decimal>());

            var allCandles = AtomCandleStorage.Instance.GetAllMinuteCandles(chart.Symbol);
            minuteCandles = allCandles.SkipWhile(c => c.timeOpen < startTime).ToList();
            var packer = new CandlePacker(new BarSettings {Intervals = new List<int> {1440}});
            dailyCandles = new List<CandleData>();
            pointCost = DalSpot.Instance.GetPrecision10(chart.Symbol);

            var highOnHour = tailHours.ToDictionary(h => h, h => 0M);
            var lowOnHour = tailHours.ToDictionary(h => h, h => 0M);

            foreach (var c in minuteCandles)
            {
                var candle = packer.UpdateCandle(c);
                var hour = c.timeClose.Hour;
                
                if (candle != null)
                {
                    dailyCandles.Add(candle);
                    // заполнить "дырки" в часах
                    for (var i = tailHours.Length - 2; i <= 0; i--)
                    {
                        if (highOnHour[tailHours[i]] == 0)
                            highOnHour[tailHours[i]] = highOnHour[tailHours[i + 1]];
                        if (lowOnHour[tailHours[i]] == 0)
                            lowOnHour[tailHours[i]] = lowOnHour[tailHours[i + 1]];
                    }
                    // заполнить массив "хвостиков"
                    foreach (var h in tailHours)
                    {
                        var tailList = tailLengthByHour[h];
                        if (candle.open < candle.close)
                            tailList.Add(((decimal) candle.open - lowOnHour[h]) * pointCost);
                        else
                            tailList.Add((highOnHour[h] - (decimal) candle.open) * pointCost);
                    }
                    // обнулить хвостики
                    foreach (var h in tailHours)
                    {
                        highOnHour[h] = 0;
                        lowOnHour[h] = 0;
                    }
                }

                foreach (var tailHour in tailHours)
                {
                    if (hour >= tailHour) continue;
                    if ((decimal) c.high > highOnHour[tailHour] || highOnHour[tailHour] == 0)
                        highOnHour[tailHour] = (decimal) c.high;
                    if ((decimal) c.low < lowOnHour[tailHour] || lowOnHour[tailHour] == 0)
                        lowOnHour[tailHour] = (decimal) c.low;
                }
            }
        }

        private void CalculateCommon(StreamWriter sw, List<decimal> tailLengths)
        {
            // распределения длин свечей в зависимости от длины хвостика           
            var lenByScale = pointsScales.ToDictionary(s => s, s => new List<decimal>());
            for (var i = 0; i < dailyCandles.Count; i++)
            {
                var c = dailyCandles[i];
                // длина тени от хвостика до открытия
                var oShad = tailLengths[i];
                var scale = pointsScales.FirstOrDefault(s => oShad <= s);
                if (scale == 0) scale = pointsScales.Last();

                // длина свечи
                var bodyLen = Math.Abs(c.open - c.close) * pointCost;
                lenByScale[scale].Add((decimal)bodyLen);
            }

            // график средней длины от длины хвостика
            sw.WriteLine();
            sw.WriteLine("Средняя длина свечи от длины хвоста (пункты):");
            foreach (var len in lenByScale.OrderBy(l => l.Key))
            {
                if (len.Value.Count == 0) continue;
                var avgLen = len.Value.Average();
                sw.WriteLine($"{len.Key}\t{avgLen.ToStringUniform(1)}");
            }

            // вероятность того, что движение будет больше 100 - 150 - 200 - 250 пунктов
            var moveScales = new [] { 100, 150, 200, 250 };
            sw.WriteLine();
            sw.WriteLine("Вероятность длины свечи N пунктов от длины хвоста (пункты).");
            sw.WriteLine("N = [" + string.Join(", ", moveScales) + "]:");
            PrintProbForMovementTable(sw, lenByScale, moveScales);

            // вероятность того, что движение составит больше 150 - 200 - 250 - 300% среднего движения
            var probScales = new [] { 150, 200, 250, 300 };
            moveScales = probScales.Select(p => (int) Math.Round( p * (decimal) avgBody / 100M)).ToArray();
            sw.WriteLine();
            sw.WriteLine("Вероятность длины свечи N% от средней за период, от длины хвоста (пункты).");
            sw.WriteLine("%: [" + string.Join(", ", probScales) + "]:");
            sw.WriteLine("Длины: [" + string.Join(", ", moveScales) + "]:");
            PrintProbForMovementTable(sw, lenByScale, moveScales);
        }

        private static void PrintProbForMovementTable(StreamWriter sw, Dictionary<int, List<decimal>> lenByScale, int[] moveScales)
        {
            foreach (var len in lenByScale.OrderBy(l => l.Key))
            {
                if (len.Value.Count == 0) continue;
                var probList = new List<decimal>();
                foreach (var moveScale in moveScales)
                {
                    var prob = len.Value.Count(v => v >= moveScale)*100M/len.Value.Count;
                    probList.Add(prob);
                }
                var probsStr = string.Join("\t", probList.Select(p => p.ToStringUniform(1)));
                sw.WriteLine($"{len.Key}\t{probsStr}");
            }
        }
    }
}
