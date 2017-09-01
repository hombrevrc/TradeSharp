using System.Collections.Generic;
using System.Linq;
using Entity;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.Robot.Robot
{
    public partial class FiboLevelRobot
    {
        /// <summary>
        /// содержит дополнительные атрибуты ордера,
        /// определяемые через поле ExpertComment
        /// (цены A - B, номер в серии)
        /// </summary>
        class FiboRobotPosition
        {
            public MarketOrder order;

            public BarSettings timeframe;

            public decimal PriceA { get; set; }

            public decimal PriceB { get; set; }

            /// <summary>
            /// 1 для "первичного" входа, 2 для "вторичного" и т.д.
            /// </summary>
            public int Sequence { get; set; }

            public static void MakeOrderComments(MarketOrder order, BarSettings timeframe,
                decimal priceA, decimal priceB, int sequence = 1)
            {
                order.ExpertComment = RobotNamePreffix;
                order.Comment = string.Join(";",
                    BarSettingsStorage.Instance.GetBarSettingsFriendlyName(timeframe),
                    priceA.ToStringUniformPriceFormat(),
                    priceB.ToStringUniformPriceFormat(),
                    sequence);
            }

            public static List<FiboRobotPosition> GetRobotPositions(string ticker,
                IEnumerable<MarketOrder> orders, BarSettings timeframe,
                decimal? priceAFilter, decimal? priceBFilter)
            {
                var maxDeltaPriceAbs = DalSpot.Instance.GetAbsValue(ticker, 1.5M);
                var deals = orders.Select(MakeFiboPosition)
                    .Where(o => o != null)
                    .Where(d =>
                        d.timeframe == timeframe &&
                        (!priceAFilter.HasValue || d.PriceA.RoughCompares(priceAFilter.Value, maxDeltaPriceAbs)) &&
                        (!priceBFilter.HasValue || d.PriceB.RoughCompares(priceBFilter.Value, maxDeltaPriceAbs)))
                    .ToList();
                return deals;
            }

            private static FiboRobotPosition MakeFiboPosition(MarketOrder order)
            {
                if (order.ExpertComment != RobotNamePreffix) return null;
                var barSetsPreffixLength = order.Comment.IndexOf(';');
                if (barSetsPreffixLength <= 0) return null;
                var barSetsPreffix = order.Comment.Substring(0, barSetsPreffixLength);
                var barSettings = BarSettingsStorage.Instance.GetBarSettingsByName(barSetsPreffix);
                if (barSettings == null)
                    return null;

                var commentStr = order.Comment.Substring(barSetsPreffixLength + 1);

                var commentParts = commentStr.ToDecimalArrayUniform();
                if (commentParts.Length < 2 || commentParts.Length > 3) return null;

                return new FiboRobotPosition
                {
                    timeframe = barSettings,
                    order = order,
                    PriceA = commentParts[0],
                    PriceB = commentParts[1],
                    Sequence = commentParts.Length > 2 ? (int)commentParts[2] : 1
                };
            }
        }
    }
}
