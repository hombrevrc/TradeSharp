using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TradeSharp.Util;

namespace Entity
{
    public static class CsvReader
    {
        private static readonly char[] stSpace = {' ', '\t'};

        private static readonly char[] stComma = {','};

        public enum LineFormat { SpaceSeparated = 0, CommaSeparated }

        public static List<CandleData> ReadCandles(string filePath, int timeframe)
        {
            if (!File.Exists(filePath)) return new List<CandleData>();

            LineFormat? format = null;
            var candles = new List<CandleData>();
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        var candle = ParseCandle(line, timeframe, ref format);
                        if (candle != null)
                            candles.Add(candle);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in CsvReader({filePath}): {ex}");
            }
            return candles;
        }

        private static CandleData ParseCandle(string line, int timeframe, ref LineFormat? format)
        {
            if (format.HasValue)
                return ParseCandleFormatted(line, timeframe, format.Value);
            var formatCandidate = LineFormat.CommaSeparated;
            var candle = ParseCandleFormatted(line, timeframe, formatCandidate);
            if (candle != null)
            {
                format = formatCandidate;
                return candle;
            }

            formatCandidate = LineFormat.SpaceSeparated;
            candle = ParseCandleFormatted(line, timeframe, formatCandidate);
            if (candle != null)
            {
                format = formatCandidate;
                return candle;
            }

            return null;
        }

        private static CandleData ParseCandleFormatted(string line, int timeframe, LineFormat format)
        {
            if (format == LineFormat.CommaSeparated)
                return ParseCandleCommaFormatted(line, timeframe);
            return ParseCandleSpaceFormatted(line, timeframe);
        }

        private static CandleData ParseCandleCommaFormatted(string line, int timeframe)
        {
            // 2007.11.14,00:00,50.000,50.000,50.000,50.000,0
            var parts = line.Split(stComma, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 6) return null;
            var dateStr = parts[0] + " " + parts[1];
            DateTime date;
            if (!DateTime.TryParseExact(dateStr, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out date))
                return null;
            var o = parts[2].ToFloatUniformSafe();
            if (!o.HasValue) return null;
            var h = parts[3].ToFloatUniformSafe();
            if (!h.HasValue) return null;
            var l = parts[4].ToFloatUniformSafe();
            if (!l.HasValue) return null;
            var c = parts[5].ToFloatUniformSafe();
            if (!c.HasValue) return null;

            return new CandleData(o.Value, h.Value, l.Value, c.Value, date, date.AddMinutes(timeframe));
        }

        private static CandleData ParseCandleSpaceFormatted(string line, int timeframe)
        {
            //KS_EURUSD Bid
            //TIME	OPEN	HIGH	LOW	CLOSE	VOLUME	
            //29/07/15 11:00:00	41,18	41,18	40,37	40,52	12,00
            var parts = line.Split(stSpace, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 6) return null;
            var dateStr = parts[0] + " " + parts[1];

            DateTime date;
            if (!DateTime.TryParseExact(dateStr, "dd/MM/yy HH:mm:ss", CultureProvider.Common, DateTimeStyles.None, out date))
                return null;
            var open = parts[2].Replace(',', '.').ToFloatUniformSafe();
            if (open == null) return null;
            var high = parts[3].Replace(',', '.').ToFloatUniformSafe();
            if (high == null) return null;
            var low = parts[4].Replace(',', '.').ToFloatUniformSafe();
            if (low == null) return null;
            var close = parts[5].Replace(',', '.').ToFloatUniformSafe();
            if (close == null) return null;

            return new CandleData(open.Value, high.Value, low.Value, close.Value, date, date.AddMinutes(timeframe));
        }
    }
}
