using System;
using System.Collections.Generic;
using TradeSharp.Contract.Entity;

namespace TradeSharp.QuoteStreamLib
{
    public interface IQuoteSource
    {
        event Action<List<TickerQuoteData>> OnQuotes;

        void Start();

        void Stop();
    }
}
