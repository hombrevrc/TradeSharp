using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSharpEntity
{
    [Table("POSITION")]
    public class Position
    {
        public int ID { get; set; }

        public int State { get; set; }

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

        public DateTime TimeEnter { get; set; }

        public int Side { get; set; }

        // 8,5
        public decimal Stoploss { get; set; }

        // 8,5
        public decimal Takeprofit { get; set; }

        // 8,5
        public decimal PriceBest { get; set; }

        // 8,5
        public decimal PriceWorst { get; set; }

        // 8,5
        public decimal? TrailLevel1 { get; set; }

        public decimal? TrailTarget1 { get; set; }

        public decimal? TrailLevel2 { get; set; }

        public decimal? TrailTarget2 { get; set; }

        public decimal? TrailLevel3 { get; set; }

        public decimal? TrailTarget3 { get; set; }

        public decimal? TrailLevel4 { get; set; }

        public decimal? TrailTarget4 { get; set; }
    }
}