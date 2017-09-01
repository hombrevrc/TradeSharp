using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TradeSharpEntity.Model;
using TradeSharpEntity.Util;

namespace TradeSharpEntity.Analysis
{
    public partial class TradeSignalAnalysis
    {
        private readonly int accountId;

        private List<TradeSignal> signals;

        private string quoteFilePath;

        private const int MaxOffset = 4;

        private int timeframe, timeframes, quoteOffset;

        private string symbol;

        public TradeSignalAnalysis(int accountId)
        {
            this.accountId = accountId;
        }

        public void BuildEratio(string symbol)
        {
            this.symbol = symbol;
            quoteFilePath = ExecutablePath.Combine($"{symbol}.csv");
            LoadTradeSignals();
            BuildEratio();
        }

        public void BuildEratio(string symbol, List<TradeSignal> signals)
        {
            this.symbol = symbol;
            quoteFilePath = ExecutablePath.Combine($"{symbol}.csv");
            this.signals = signals;
            BuildEratio();
        }

        public void FindSuitableOffset(string symbol)
        {
            this.symbol = symbol;
            quoteFilePath = ExecutablePath.Combine($"{symbol}.csv");
            LoadTradeSignals();
            FindTimeframeOffest();
        }

        private void LoadTradeSignals()
        {
            SharpRepo.InitDefault();
            signals = SharpRepo.GetSignals(accountId, symbol);
            signals.ForEach(s =>
            {
                var shift = s.Time.Second > 30;
                s.Time = new DateTime(s.Time.Year, s.Time.Month, s.Time.Day, s.Time.Hour, s.Time.Minute, 0);
                if (shift) s.Time = s.Time.AddMinutes(1);
            });
        }

        private void FindTimeframeOffest()
        {
            var sigByTime = signals.GroupBy(s => s.Time).ToDictionary(s => s.Key, s => s.First());

            var mistakeByOffset = new Dictionary<int, OffsetMistake>();
            for (var i = -MaxOffset; i <= MaxOffset; i++)
                mistakeByOffset.Add(i, new OffsetMistake {offset = i});
            using (var candleReader = new CandleStream(quoteFilePath))
            {
                foreach (var candle in candleReader)
                {
                    for (var i = -MaxOffset; i <= MaxOffset; i++)
                    {
                        var time = candle.time.AddHours(i);
                        if (!sigByTime.TryGetValue(time, out var sig)) continue;
                        var delta = Math.Abs(100 * ((decimal) candle.o - sig.Enter) / sig.Enter);
                        var mis = mistakeByOffset[i];
                        mis.mistakeSum += delta;
                        mis.count++;
                    }
                }
            }
            // выбрать смещение с наименьшей ошибкой
            var min = 0M;
            var offset = 0;
            foreach (var mis in mistakeByOffset.Values)
            {
                var av = mis.count == 0 ? 0 : mis.mistakeSum / mis.count;
                if (mis.count > 0 && av < min)
                {
                    offset = mis.offset;
                    min = av;
                }
                Console.WriteLine($"[Offset {mis.offset}]: avg. mistake is {av:f4}%, count {mis.count}");
            }
            Console.WriteLine($"Offset should be {offset}");
        }

        private void BuildEratio()
        {
            if (signals.Count == 0) return;

            ReadConfig();

            var signalStats = signals.OrderBy(s => s.Time).Select(s => new SignalStat(s, timeframe, timeframes)).ToList();
            var actualStats = new List<SignalStat>();
            var resultedStats = new List<SignalStat>();

            using (var candleReader = new CandleStream(quoteFilePath))
            {
                foreach (var candle in candleReader)
                {
                    candle.time = candle.time.AddHours(quoteOffset);
                    for (var i = 0; i < signalStats.Count; i++)
                    {
                        if (signalStats[i].time > candle.time) break;
                        actualStats.Add(signalStats[i]);
                        // выровнять цену входа
                        signalStats[i].enter = (candle.o + candle.c) / 2;
                        signalStats.RemoveAt(i--);
                    }

                    for (var i = 0; i < actualStats.Count; i++)
                    {
                        // обновить pros / cons
                        actualStats[i].UpdateProsCons(candle, candle.time);
                        if (actualStats[i].endTime > candle.time) break;
                        resultedStats.Add(actualStats[i]);
                        actualStats.RemoveAt(i--);
                    }
                }
            }
            resultedStats.AddRange(actualStats);

            // свернуть статистику в график
            ReduceERatio(resultedStats);
        }

        private void ReadConfig()
        {
            var fileName = ExecutablePath.Combine("settings.ini");
            var ini = new IniFile(fileName);
            timeframe = int.Parse(ini.ReadValue("Common", "Timeframe", "120"));
            timeframes = int.Parse(ini.ReadValue("Common", "Timeframes", "240"));
            quoteOffset = int.Parse(ini.ReadValue("Common", "Offset", "0"));
        }

        private void ReduceERatio(List<SignalStat> stat)
        {
            var eratio = new float[timeframes];
            for (var i = 0; i < eratio.Length; i++)
            {
                float sumPro = 0f, sumCons = 0f;
                foreach (var s in stat)
                {
                    sumPro += s.proCons[i].pro;
                    sumCons += s.proCons[i].con;
                }
                eratio[i] = sumCons == 0f ? 1 : sumPro / sumCons;
            }
            // сохранить в файл
            var fileName = ExecutablePath.Combine($"report_{accountId}_{symbol}.txt");
            using (var sw = new StreamWriter(fileName, false, Encoding.ASCII))
            {
                sw.WriteLine($"Account {accountId}, symbol {symbol}");
                sw.WriteLine($"{signals.Count} trades, M{timeframe}, {timeframes} timeframes");
                sw.WriteLine();
                foreach (var e in eratio)
                    sw.WriteLine(e.ToString("f4", CultureInfo.InvariantCulture));
            }
        }
    }
}
