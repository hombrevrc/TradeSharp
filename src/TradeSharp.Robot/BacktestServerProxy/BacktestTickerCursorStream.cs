using System;
using System.IO;
using Entity;

namespace TradeSharp.Robot.BacktestServerProxy
{
    /// <summary>
    /// в текущей реализации поток котировок "сидит" в ОП:
    /// хранится список котировок, по которому "указатель" (индекс)
    /// смещается вперед по мере теста
    /// </summary>
    public class BacktestTickerCursorStream : IDisposable
    {
        public string Ticker { get; private set; }

        public CandleData currentCandle, nextCandle;

        private DateTime startTime;
        
        private StreamReader stream;

        private DateTime? fileDate;

        private int precision = 0;

        private string fileName;

        public BacktestTickerCursorStream(string quoteFolder, string ticker)
        {
            Ticker = ticker;
            precision = DalSpot.Instance.GetPrecision10(Ticker);
            fileName = quoteFolder.TrimEnd('\\') + "\\" + Ticker + ".quote";
            if (string.IsNullOrEmpty(fileName))
                return;
            if (!File.Exists(fileName))
                return;

            stream = new StreamReader(fileName);
            Reset();
        }

        public void Close()
        {
            if (stream == null)
                return;
            stream.Close();
        }

        public bool MoveToTime(DateTime time, out CandleData candle)
        {
            candle = null;
            if (stream == null) return false;
            if (currentCandle == null) return false;
            if (time < currentCandle.timeClose)
            {
                stream.BaseStream.Position = 0;
                stream.DiscardBufferedData();  
                Reset();
                if (currentCandle.timeClose > time)
                    return false;
            }

            while (currentCandle != null)
            {
                if (nextCandle != null && nextCandle.timeClose > time)
                {
                    candle = currentCandle;
                    return true;
                }
                currentCandle = nextCandle;
                nextCandle = ReadCandle(nextCandle);
            }
            return false;
        }

        public CandleData GetCurQuote()
        {
            return currentCandle == null ? null : new CandleData(currentCandle);
        }

        public DateTime? GetNextTime()
        {
            return nextCandle == null ? (DateTime?)null : nextCandle.timeClose;
        }

        public void MoveNext()
        {
            if (stream == null || nextCandle == null)
                return;
            currentCandle = nextCandle;
            nextCandle = ReadCandle(nextCandle);
        }

        public void Dispose()
        {
            Close();
        }

        private void Reset()
        {
            fileDate = null;
            currentCandle = null;
            nextCandle = null;
            currentCandle = ReadCandle(null);
            
            if (currentCandle == null)
            {
                stream.Close();
                stream = null;
                currentCandle = null;
                fileName = null;
                return;
            }

            startTime = currentCandle.timeClose;
            nextCandle = ReadCandle(currentCandle);
        }

        private CandleData ReadCandle(CandleData candle)
        {
            while (true)
            {
                if (stream.EndOfStream)
                    return null;

                var fileLine = stream.ReadLine();
                if (string.IsNullOrEmpty(fileLine))
                    continue;
                candle = CandleData.ParseLine(fileLine, ref fileDate, precision, ref candle);
                if (candle != null)
                    break;
            }
            return candle;
        }
    }    
}
