using System;
using System.Collections.Generic;
using System.Linq;
using Entity;

namespace Candlechart.ChartMath
{
    public class CandleConsumptionFinder
    {
        public class Consumption
        {
            public int startIndex;

            public int consumerIndex;

            public decimal lower, upper;
        }

        class Bar
        {
            public decimal up, down;

            public int dir;

            public decimal Open
            {
                get { return dir < 0 ? up : down; }
                set
                {
                    if (dir < 0) up = value;
                    else down = value;
                }
            }

            public decimal Close
            {
                get { return dir < 0 ? down : up; }
                set
                {
                    if (dir < 0) down = value;
                    else up = value;
                }
            }

            public bool Consumes(Bar prey)
            {
                if (prey.dir != 0 && prey.dir == dir) return false;
                return prey.up <= up && prey.down >= down;
            }
        }

        public enum CandleConsumptionStyle
        {
            СтрогаяПроверка = 0,
            РазныеЦвета
        }

        private readonly List<CandleData> candles;

        private List<Bar> bars;

        private readonly int minCount;

        private readonly List<Consumption> consumptions;

        public CandleConsumptionFinder(List<CandleData> candles, int minCount)
        {
            this.candles = candles;
            this.minCount = minCount;
            consumptions = new List<Consumption>();
        }

        public List<Consumption> Search()
        {
            if (candles.Count <= minCount) return consumptions;
            BuildBars();


            for (var i = minCount; i < bars.Count; i++)
            {
                if (bars[i].dir == 0) continue;

                // строгая проверка
                var consumed = CheckStrict(i);

                if (consumed < minCount) continue;
                var startIndex = i - consumed;
                var subCandles = candles.Skip(startIndex).Take(consumed + 1).ToList();
                consumptions.Add(new Consumption
                {
                    startIndex = startIndex,
                    consumerIndex = i,
                    upper = (decimal)subCandles.Max(c => c.open > c.close ? c.open : c.close),
                    lower = (decimal)subCandles.Min(c => c.open < c.close ? c.open : c.close),
                });
            }

            return consumptions;
        }

        private int CheckStrict(int i)
        {
            var consumed = 0;
            for (var j = i - 1; j >= 0; j--)
            {
                if (!bars[i].Consumes(bars[j])) break;
                consumed++;
            }
            return consumed;
        }

        private void BuildBars()
        {
            bars = candles.Select(c => new Bar
            {
                up = (decimal)Math.Max(c.open, c.close),
                down = (decimal)Math.Min(c.open, c.close),
                dir = c.open < c.close ? 1 : c.open > c.close ? -1 : 0
            }).ToList();
            // выровнять бары - убрать гэпы
            for (var i = 1; i < bars.Count; i++)
            {
                if (bars[i].Open != bars[i - 1].Close)
                    bars[i].Open = bars[i - 1].Close;
            }
        }
    }
}
