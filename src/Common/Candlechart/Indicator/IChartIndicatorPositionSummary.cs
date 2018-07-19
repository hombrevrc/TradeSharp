using System.Collections.Generic;
using TradeSharp.Contract.Entity;

namespace Candlechart.Indicator
{
    public interface IChartIndicatorPositionSummary
    {
        void UpdatePositionSummary(float totalProfit, float totalProfitInPercent, List<MarketOrder> openOrders);
    }
}