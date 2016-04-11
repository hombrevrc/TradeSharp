using System;
using System.Collections.Generic;
using Entity;

namespace Candlechart.ChartMath
{
    public enum ZigZagSource { HighLow = 0, OpenClose = 1}

    public static class ZigZag
    {
        public static List<ZigZagPivot> GetPivots(IList<CandleData> candles,
            float thresholdPercent, ZigZagSource srcType)
        {
            return GetPivots(candles, thresholdPercent, candles.Count - 1, srcType);
        }

        public static List<ZigZagPivot> GetPivots(IList<CandleData> candles,
            float thresholdPercent, int endIndex, ZigZagSource srcType)
        {
            return GetPivots(candles, thresholdPercent, thresholdPercent,
                endIndex, srcType);
        }

        public static List<ZigZagPivot> GetPivots(IList<CandleData> candles,
            float thresholdPercent,
            float correctionPercent,
            int endIndex, ZigZagSource srcType)
        {
            var pivots = new List<ZigZagPivot>();
            if (candles.Count == 0) return pivots;
            var lastPivot = new ZigZagPivot(0, candles[0].open);
            int lastSign = 0;

            // занести вершины
            int i = 1;
            for (; i <= endIndex; i++)
            {
                var candle = candles[i];
                var high = srcType == ZigZagSource.HighLow ? candle.high : Math.Max(candle.open, candle.close);
                var low = srcType == ZigZagSource.HighLow ? candle.low : Math.Min(candle.open, candle.close);

                var deltaPlus = high - lastPivot.price;
                var deltaMinus = lastPivot.price - low;
                deltaPlus = deltaPlus > 0 ? 100 * deltaPlus / lastPivot.price : 0;
                deltaMinus = deltaMinus > 0 ? 100 * deltaMinus / lastPivot.price : 0;

                if (deltaPlus > correctionPercent &&
                    ((deltaPlus > deltaMinus && lastSign == 0) || lastSign < 0))
                {
                    pivots.Add(lastPivot);
                    lastPivot = new ZigZagPivot(i, high);
                    lastSign = 1;
                    continue;
                }
                if (deltaMinus > correctionPercent &&
                    ((deltaPlus <= deltaMinus && lastSign == 0) || lastSign > 0))
                {
                    pivots.Add(lastPivot);
                    lastPivot = new ZigZagPivot(i, low);
                    lastSign = -1;
                    continue;
                }
                if (lastSign > 0 && high > lastPivot.price)
                {
                    lastPivot.price = high;
                    lastPivot.index = i;
                    continue;
                }
                if (lastSign < 0 && low < lastPivot.price)
                {
                    lastPivot.price = low;
                    lastPivot.index = i;
                    continue;
                }
            }
            // последняя вершина
            if (lastSign != 0)
                pivots.Add(lastPivot);

            DetermineCorrectionOrTrend(pivots, thresholdPercent, correctionPercent);

            return pivots;
        }

        private static void DetermineCorrectionOrTrend(List<ZigZagPivot> pivots, float trendTh, float correctTh)
        {
            if (pivots.Count < 2 || trendTh <= correctTh) return;

            for (var i = 1; i < pivots.Count; i++)
            {
                var start = pivots[i - 1].price;
                if (start == 0) continue;

                var deltaPrice = 100 * Math.Abs(pivots[i].price - start) / start;
                if (deltaPrice < trendTh)
                    pivots[i - 1].nextArc = ZigZagPivot.ZigZagArc.Correction;
            }
        }
    }

    public class ZigZagPivot
    {
        public enum ZigZagArc { Trend = 0, Correction }

        public int index;

        public float price;

        public ZigZagArc nextArc;

        public ZigZagPivot()
        {            
        }

        public ZigZagPivot(int index, float price)
        {
            this.index = index;
            this.price = price;
        }
    }
}
