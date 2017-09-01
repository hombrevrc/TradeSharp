using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSharpEntity
{
    [Table("POSITION_CLOSED")]
    public class PositionClosed
    {
        public int ID { get; set; }

        [MaxLength(12)]
        public string Symbol { get; set; }

        public int AccountID { get; set; }

        public int? PendingOrderID { get; set; }

        public int Volume { get; set; }

        [MaxLength(64)]
        public string Comment { get; set; }

        [MaxLength(64)]
        public string ExpertComment { get; set; }

        public int? Magic { get; set; }

        // 8,5
        public decimal PriceEnter { get; set; }

        public decimal PriceExit { get; set; }

        public DateTime TimeEnter { get; set; }

        public DateTime TimeExit { get; set; }

        public int Side { get; set; }

        // 8,5
        public decimal Stoploss { get; set; }

        // 8,5
        public decimal Takeprofit { get; set; }

        // 8,5
        public decimal PriceBest { get; set; }

        // 8,5
        public decimal PriceWorst { get; set; }

        public int ExitReason { get; set; }

        // 8,5
        public decimal Swap { get; set; }

        // 8,2
        public decimal ResultPoints { get; set; }

        // 16,2
        public decimal ResultBase { get; set; }

        // 16,2
        public decimal ResultDepo { get; set; }
    }
}
