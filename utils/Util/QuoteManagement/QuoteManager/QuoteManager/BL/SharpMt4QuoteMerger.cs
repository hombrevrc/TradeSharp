using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Entity;
using TradeSharp.Util;

namespace QuoteManager.BL
{
    class SharpMt4QuoteMerger
    {
        private readonly string pathQuote;

        private readonly string pathCsv;

        private readonly DateTime endTime;

        private readonly List<CandleData> candlesMt4 = new List<CandleData>();

        private readonly List<CandleData> candlesSharp = new List<CandleData>();

        private readonly int timeframe, digits;

        public readonly List<string> messages = new EverSortedList<string>();

        public SharpMt4QuoteMerger(string pathQuote, string pathCsv, DateTime endTime,
            int timeframe, int digits)
        {
            this.pathQuote = pathQuote;
            this.pathCsv = pathCsv;
            this.endTime = endTime;
            this.timeframe = timeframe;
            this.digits = digits;
        }

        public void Merge()
        {
            ReadCsv();
            if (candlesMt4.Count == 0) return;
            ReadSharpQuotes();
            MergeCandles();
        }

        private void MergeCandles()
        {
            candlesMt4.AddRange(candlesSharp);
            CandleData.SaveInFile(pathQuote + ".merged", digits, candlesMt4);
        }

        private void ReadCsv()
        {
            using (var sr = new StreamReader(pathCsv))
            {
                // 2004.01.01,00:00,1.25920,1.25970,1.25920,1.25970,3
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    var parts = line.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 6) continue;
                    var dateStr = parts[0] + " " + parts[1];

                    DateTime time;
                    if (!DateTime.TryParseExact(dateStr, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out time))
                        continue;

                    if (time >= endTime) break;

                    var open = parts[2].ToFloatUniformSafe();
                    var high = parts[3].ToFloatUniformSafe();
                    var low = parts[4].ToFloatUniformSafe();
                    var close = parts[5].ToFloatUniformSafe();

                    if (open == null || high == null || low == null || close == null) continue;

                    candlesMt4.Add(new CandleData(open.Value, high.Value, low.Value, close.Value, time, time.AddMinutes(timeframe)));
                }
            }
            messages.Add($"Прочитано {candlesMt4.Count} котировок _{timeframe}");
        }

        private void ReadSharpQuotes()
        {
            var absToPoints = (int) Math.Pow(10, digits);

            using (var sr = new StreamReader(pathQuote))
            {
                // 18012011
                // 0000 1.32790 7FFF8027
                DateTime? date = null;
                CandleData prevCandle = null, candle;

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    candle = CandleData.ParseLine(line, ref date, absToPoints, ref prevCandle);
                    if (candle != null)
                        prevCandle = candle;

                    if (candle == null) continue;
                    if (candle.timeOpen <= endTime) continue;

                    candlesSharp.Add(candle);                    
                }
                messages.Add($"Прочитано {candlesSharp.Count} котировок T#");
            }
        }
    }
}
