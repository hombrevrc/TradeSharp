using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    public partial class IndicatorOrders
    {
        public void UpdatePositionSummary(float profit, float profitInPercent)
        {
            var currPrice = owner.StockPane.StockSeries.Data[owner.StockPane.StockSeries.Data.Count - 1].close;

            var currAbs = seriesCommentLastBar.data.FirstOrDefault(c => c.Name == "CurrProfitAbs");
            if (currAbs != null)
            {
                currAbs.Text = profit.ToStringUniformMoneyFormat();
                currAbs.PivotPrice = currPrice;
                currAbs.ColorText = profit > 0 ? Color.Green : Color.Red;
            }

            var currPercent = seriesCommentLastBar.data.FirstOrDefault(c => c.Name == "CurrProfitPercent");
            if (currPercent != null)
            {
                currPercent.Text = profitInPercent.ToString("F2") + " %";
                currPercent.PivotPrice = currPrice;
                currPercent.ColorText = profitInPercent > 0 ? Color.Green : Color.Red;
            }
        }
    }
}