using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    public partial class IndicatorOrders
    {
        public void UpdatePositionSummary(float totalProfit, float totalProfitInPercent, List<MarketOrder> openOrders)
        {
            var currPrice = owner.StockPane.StockSeries.Data[owner.StockPane.StockSeries.Data.Count - 1].close;

            var currAbs = seriesCommentLastBar.data.FirstOrDefault(c => c.Name == "CurrProfitAbs");
            if (currAbs != null)
            {
                currAbs.Text = totalProfit.ToStringUniformMoneyFormat();
                currAbs.PivotPrice = currPrice;
                currAbs.ColorText = totalProfit > 0 ? Color.Green : Color.Red;
            }

            var currPercent = seriesCommentLastBar.data.FirstOrDefault(c => c.Name == "CurrProfitPercent");
            if (currPercent != null)
            {
                currPercent.Text = totalProfitInPercent.ToString("F2") + " %";
                currPercent.PivotPrice = currPrice;
                currPercent.ColorText = totalProfitInPercent > 0 ? Color.Green : Color.Red;
            }

            foreach (var openOrder in openOrders)
            {
                var pos = openPositions.FirstOrDefault(c => c.ID == openOrder.ID);
                if (pos != null)
                    pos.ResultDepo = openOrder.ResultDepo;
            }
        }
    }
}