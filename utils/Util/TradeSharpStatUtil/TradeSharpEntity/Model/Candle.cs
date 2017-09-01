using System;
using System.Globalization;

namespace TradeSharpEntity.Model
{
    public class Candle
    {
        public DateTime time;

        public float o, h, l, c;

        private static readonly char[] separators = {','};

        public override string ToString()
        {
            return $"{time:yyyy.MM.dd HH:mm};{o:f5};{h:f5};{l:f5};{c:f5}";
        }

        public static Candle ParseString(string s)
        {
            // 2015.01.02 00:00,1.21043,1.21043,1.21030,1.21030,7
            var parts = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5) return null;

            var candle = new Candle();
            if (!DateTime.TryParseExact(parts[0], "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var time)) return null;
            candle.time = time;

            if (!float.TryParse(parts[1], out var f)) return null;
            candle.o = f;

            if (!float.TryParse(parts[2], out f)) return null;
            candle.h = f;

            if (!float.TryParse(parts[3], out f)) return null;
            candle.l = f;

            if (!float.TryParse(parts[4], out f)) return null;
            candle.c = f;

            return candle;
        }
    }
}
