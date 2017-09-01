using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TradeSharpEntity.Model
{
    public class CandleStream : IEnumerable<Candle>, IDisposable
    {
        private readonly StreamReader reader;

        public CandleStream(string fileName)
        {
            reader = new StreamReader(fileName, Encoding.ASCII);            
        }

        public IEnumerator<Candle> GetEnumerator()
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                var candle = Candle.ParseString(line);
                if (candle == null) continue;
                yield return candle;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            reader?.Dispose();
        }
    }
}
