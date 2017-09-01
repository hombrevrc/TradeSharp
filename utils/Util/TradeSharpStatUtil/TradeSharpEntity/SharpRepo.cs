using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TradeSharpEntity.Model;
using TradeSharpEntity.Util;

namespace TradeSharpEntity
{
    public static class SharpRepo
    {
        private static string connectionString;

        public static void Init(string connString)
        {
            connectionString = connString;
        }

        public static void InitDefault()
        {
            var fileName = ExecutablePath.Combine("settings.ini");
            var ini = new IniFile(fileName);
            connectionString = ini.ReadValue("DB", "SharpConnection", "");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"Connection string was not found, file \"{fileName}\"");
        }

        public static List<TradeSignal> GetSignals(int accountId, string optionalSymbol)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var ctx = new SharpContext(conn, true))
            {
                var opSignal = (string.IsNullOrEmpty(optionalSymbol)
                    ? ctx.Position.Where(p => p.AccountID == accountId)
                    : ctx.Position.Where(p => p.AccountID == accountId && p.Symbol == optionalSymbol)).Select(p => new TradeSignal
                    {
                        Symbol = p.Symbol,
                        Enter = p.PriceEnter,
                        Side = p.Side,
                        Time = p.TimeEnter
                    }).ToList();

                var closeSignal = (string.IsNullOrEmpty(optionalSymbol)
                    ? ctx.PositionClosed.Where(p => p.AccountID == accountId)
                    : ctx.PositionClosed.Where(p => p.AccountID == accountId && p.Symbol == optionalSymbol)).Select(p => new TradeSignal
                {
                    Symbol = p.Symbol,
                    Enter = p.PriceEnter,
                    Side = p.Side,
                    Time = p.TimeEnter
                }).ToList();

                return opSignal.Union(closeSignal).ToList();
            }
        }

        public static void SaveSignalsToFile(string path, List<TradeSignal> signals)
        {
            using (var sw = new StreamWriter(path, false, Encoding.ASCII))
            {
                foreach (var signal in signals)
                    sw.WriteLine($"{signal.Time:yyyy-MM-dd HH:mm:ss};{signal.Side};{signal.Enter:f5};{signal.Symbol}");
            }
        }

        public static List<TradeSignal> LoadSignalsFromFile(string path)
        {
            var signals = new List<TradeSignal>();
            var st = new [] {';'};
            using (var sr = new StreamReader(path, Encoding.ASCII))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    try
                    {
                        var parts = line.Split(st);
                        if (parts.Length != 4) continue;
                        signals.Add(new TradeSignal
                        {
                            Enter = decimal.Parse(parts[2]),
                            Side = int.Parse(parts[1]),
                            Symbol = parts[3],
                            Time = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        });
                    }
                    catch
                    {
                    }
                }
            }
            return signals;
        }
    }
}
