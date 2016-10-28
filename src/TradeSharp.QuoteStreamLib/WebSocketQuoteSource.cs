using System;
using System.Collections.Generic;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;
using WebSocket4Net;
using WebSocketDistribution.Model;

namespace TradeSharp.QuoteStreamLib
{
    public class WebSocketQuoteSource : IQuoteSource
    {
        public event Action<List<TickerQuoteData>> OnQuotes;

        private readonly WebSocketClient client;

        private readonly FloodSafeLogger logNoFlood = new FloodSafeLogger(1000 * 60);

        private static readonly string[] separators = { Environment.NewLine, ";" };

        private static readonly char[] quoteSeparators = { ' ' };

        public WebSocketQuoteSource(string uri)
        {
            Logger.Info($"Connecting to {uri}");
            client = new WebSocketClient();
            client.Setup(uri, "basic", WebSocketVersion.Rfc6455,
                OnMessage, (@event, s) =>
                {
                    if (@event == ConnectionEvent.Connected)
                        logNoFlood.LogMessageCheckFlood(LogEntryType.Info, (int)@event, 1000 * 60 * 60 * 2,
                            $"WebSocket quote stream: connected to {uri}");
                    if (@event == ConnectionEvent.Disconnected)
                        logNoFlood.LogMessageCheckFlood(LogEntryType.Info, (int)@event, 1000 * 60 * 60 * 2,
                            $"WebSocket quote stream: disconnected");
                    if (@event == ConnectionEvent.Faulted)
                        logNoFlood.LogMessageCheckFlood(LogEntryType.Info, (int)@event, 1000 * 60 * 60,
                            $"WebSocket quote stream: faulted to connect ({s})");
                });
        }

        public void Start()
        {
            try
            {
                client.Start();
                Logger.Info("WebSocket quote source is started");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error starting WebSocketQuoteSource: {ex}");
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                client.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in stopping WebSocketQuoteSource: {ex}");
            }
        }

        private void OnMessage(string s)
        {
            if (string.IsNullOrEmpty(s) || OnQuotes == null) return;
            var lines = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var quotes = new List<TickerQuoteData>();

            foreach (var line in lines)
            {
                var quote = ParseLine(line);
                if (quote != null) quotes.Add(quote);
            }

            if (lines.Length > 0 && quotes.Count == 0)
                logNoFlood.LogMessageCheckFlood(LogEntryType.Error, 2, 1000 * 60 * 60, "No quotes are got from lines: " + lines[0]);

            if (quotes.Count > 0) OnQuotes(quotes);
        }

        private TickerQuoteData ParseLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;
            var parts = line.Split(quoteSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return null;
            var bid = parts[1].ToFloatUniformSafe();
            if (bid == null) return null;
            var ask = parts[2].ToFloatUniformSafe();
            if (ask == null) return null;

            return new TickerQuoteData
            {
                Ticker = parts[0],
                bid = bid.Value,
                ask = ask.Value,
                Time = DateTime.Now
            };
        }
    }
}
