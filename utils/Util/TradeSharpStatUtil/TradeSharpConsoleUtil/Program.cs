using System;
using TradeSharpEntity;
using TradeSharpEntity.Analysis;
using TradeSharpEntity.Util;

namespace TradeSharpConsoleUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input account Id (949 for default)");
            var s = Console.ReadLine();
            var accountId = string.IsNullOrEmpty(s) ? 949 : int.Parse(s);
            Console.WriteLine("Now input symbol (EURUSD for default)");
            var symbol = Console.ReadLine();
            if (string.IsNullOrEmpty(symbol))
                symbol = "EURUSD";

            var analz = new TradeSignalAnalysis(accountId);
            Console.WriteLine("Check timezone offset [t] or build E-Ratio [e] (e for default).");
            Console.WriteLine("Type \"load <filename>\" (this folder) to load trades from file.");
            Console.WriteLine("Or type \"save <filename>\" (this folder) to save trades into file.");
            s = Console.ReadLine() ?? "";

            // offset
            if (s == "t" || s == "T" || s == "е" || s == "Е")            
                FindOffset(analz, symbol);            
            else
            {
                if (s.StartsWith("save "))
                    SaveSignals(s, accountId, symbol);                
                else
                    BuildEratio(s, analz, symbol);
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        private static void FindOffset(TradeSignalAnalysis analz, string symbol)
        {
            try
            {
                analz.FindSuitableOffset(symbol);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        private static void BuildEratio(string s, TradeSignalAnalysis analz, string symbol)
        {
            try
            {
                if (s.StartsWith("load "))
                {
                    var fname = ExecutablePath.Combine(s.Substring("load ".Length));
                    var signals = SharpRepo.LoadSignalsFromFile(fname);
                    analz.BuildEratio(symbol, signals);
                }
                else                
                    analz.BuildEratio(symbol);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        private static void SaveSignals(string s, int accountId, string symbol)
        {
            try
            {
                var fname = ExecutablePath.Combine(s.Substring("save ".Length));
                SharpRepo.InitDefault();
                var signals = SharpRepo.GetSignals(accountId, symbol);
                SharpRepo.SaveSignalsToFile(fname, signals);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
}
