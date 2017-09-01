using System;

namespace TradeSharpEntity.Model
{
    public class TradeSignal
    {
        public int Side { get; set; }

        public DateTime Time { get; set; }

        public decimal Enter { get; set; }

        public string Symbol { get; set; }
    }
}
