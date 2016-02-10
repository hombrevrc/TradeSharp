using System;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.FakeUser.BL
{
    public class LiveQuotes
    {
        private static readonly Lazy<LiveQuotes> instance = new Lazy<LiveQuotes>(() => new LiveQuotes());

        public static LiveQuotes Instance => instance.Value;

        private readonly ThreadSafeStorage<string, QuoteData> lastQuotes = new ThreadSafeStorage<string, QuoteData>();

        private LiveQuotes()
        {
        }

        public void UpdateQuotes(string[] names, QuoteData[] values)
        {
            lastQuotes.UpdateValues(names, values);
        }

        public QuoteData FindQuote(string name)
        {
            return lastQuotes.ReceiveValue(name);
        }
    }
}
