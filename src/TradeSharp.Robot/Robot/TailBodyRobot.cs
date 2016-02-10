using System;
using System.Collections.Generic;
using System.ComponentModel;
using Entity;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.Robot.Robot
{
    [DisplayName("Хвост - тело")]
    public class TailBodyRobot : BaseRobot
    {
        #region Настройки
        [PropertyXMLTag("Robot.TailPoints")]
        [DisplayName("Хвост, пунктов")]
        [Category("Торговые")]
        [Description("Для входа в рынок хвост должен быть не больше N пунктов")]
        public int TailPoints { get; set; } = 25;

        [PropertyXMLTag("Robot.TradeHour")]
        [DisplayName("Час входа в рынок")]
        [Category("Торговые")]
        [Description("Вход в рынок (проверка условий) в указанный час")]
        public int TradeHour { get; set; } = 8;
        #endregion

        #region Переменные

        private string ticker;

        private CandlePacker packer;

        private double pointCost;

        private DateTime lastQuoteTime;

        #endregion

        public override BaseRobot MakeCopy()
        {
            var bot = new TailBodyRobot
            {
                TailPoints = TailPoints,
                TradeHour = TradeHour
            };
            CopyBaseSettings(bot);
            return bot;
        }

        public override void Initialize(BacktestServerProxy.RobotContext grobotContext, 
            Contract.Util.BL.CurrentProtectedContext protectedContextx)
        {
            base.Initialize(grobotContext, protectedContextx);
            if (Graphics.Count == 0)
            {
                Logger.DebugFormat("TailBodyRobot: настройки графиков не заданы");
                return;
            }
            if (Graphics.Count > 1)
            {
                Logger.DebugFormat("TailBodyRobot: настройки графиков должны описывать один тикер / один ТФ");
                return;
            }
            packer = new CandlePacker(Graphics[0].b);
            ticker = Graphics[0].a;
            pointCost = 1.0 / DalSpot.Instance.GetPrecision10(ticker);
        }

        public override Dictionary<string, DateTime> GetRequiredSymbolStartupQuotes(DateTime startTrade)
        {
            if (Graphics.Count == 0) return null;
            var historyIndexStart = Graphics[0].b.GetDistanceTime(0, -1, startTrade);
            return new Dictionary<string, DateTime> { { Graphics[0].a, historyIndexStart } };
        }

        public override List<string> OnQuotesReceived(string[] names, CandleDataBidAsk[] quotes, bool isHistoryStartOff)
        {
            var events = new List<string>();

            #region получить candle из quote
            if (string.IsNullOrEmpty(ticker)) return events;
            var tickerIndex = -1;
            for (var i = 0; i < names.Length; i++)
                if (names[i] == ticker)
                {
                    tickerIndex = i;
                    break;
                }
            if (tickerIndex < 0) return events;
            var quote = quotes[tickerIndex];
            var newCandle = packer.UpdateCandle(quote);
            #endregion

            if (newCandle != null)
            {
                CloseOrders(); // закрыть открытые сделки
                return events;
            }

            // если пора проверить хвост свечи...
            var hour = quote.timeClose.Hour;
            if (hour == TradeHour && lastQuoteTime.Hour != quote.timeClose.Hour)
                CheckTrade();
            lastQuoteTime = quote.timeClose;

            return events;
        }

        private void CheckTrade()
        {
            var candle = packer.CurrentCandle;
            if (candle == null) return;
            var tail = candle.close > candle.open ? candle.open - candle.low : candle.high - candle.open;
            tail = tail / (float) pointCost;
            if (tail >= TailPoints) return;

            // войти в рынок в направлении свечи
            OpenDeal(candle.close > candle.open ? 1 : -1);
        }

        private void CloseOrders()
        {
            List<MarketOrder> orders;
            GetMarketOrders(out orders, true);
            if (orders == null || orders.Count == 0) return;
            foreach (var order in orders)
                CloseMarketOrder(order.ID);
        }

        private void OpenDeal(int dealSide)
        {
            var dealVolumeDepo = CalculateVolume(ticker);
            if (dealVolumeDepo == 0) return;

            // открыть сделку
            robotContext.SendNewOrderRequest(
                protectedContext.MakeProtectedContext(),
                RequestUniqueId.Next(),
                new MarketOrder
                {
                    AccountID = robotContext.AccountInfo.ID,
                    Magic = Magic,
                    Symbol = ticker,
                    Volume = dealVolumeDepo,
                    Side = dealSide,
                    ExpertComment = "TailBodyRobot"
                },
            OrderType.Market, 0, 0);
        }
    }
    // ReSharper restore LocalizableElement
}
