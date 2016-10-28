using System.Collections.Generic;
using TradeSharp.Contract.Entity;
using TradeSharp.QuoteStreamLib;

namespace TradeSharp.QuoteService.Feeder
{
    public class WebSocketQuoteFeeder : IQuoteFeeder
    {
        public event QuotesReceived OnQuotesReceived;

        private WebSocketQuoteSource quoteSource;

        private readonly string uri;

        public WebSocketQuoteFeeder(string uri)
        {
            this.uri = uri;
        }

        public void Start()
        {
            quoteSource = new WebSocketQuoteSource(uri);
            quoteSource.OnQuotes += list =>
            {
                var names = new List<string>();
                var quotes = new List<QuoteData>();
                foreach (var ticker in list)
                {
                    names.Add(ticker.Ticker);
                    quotes.Add(ticker);
                }
                if (OnQuotesReceived != null)
                    OnQuotesReceived(names, quotes);
            };
            quoteSource.Start();
        }

        public void Stop()
        {
            quoteSource.Stop();
            quoteSource = null;
        }        
    }
}
