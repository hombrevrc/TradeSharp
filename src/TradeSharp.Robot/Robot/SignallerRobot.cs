using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Entity;
using TradeSharp.Contract.Util.BL;

namespace TradeSharp.Robot.Robot
{
    /// <summary>
    /// Робот, отправляющий сигналы по новым ордерам
    /// </summary>
    // ReSharper disable LocalizableElement
    [DisplayName("Скользящие средние")]
    public class SignallerRobot : BaseRobot
    {
        [PropertyXMLTag("Robot.SenderTitle")]
        [DisplayName("Sender's title (name)")]
        [Category("Основные")]
        [Description("Sender's title (name or email address)")]
        public string SenderTitle { get; set; } = "";

        [PropertyXMLTag("Robot.CandlesForScreenshot")]
        [DisplayName("Candles in screenshot")]
        [Category("Основные")]
        [Description("Candles in screenshot")]
        public int CandlesForScreenshot { get; set; } = 35;

        #region Переменные

        private DateTime nowTime;
        #endregion

        /// <summary>
        /// создать полную копию робота,
        /// оперативные настройки (переменные-члены) копировать необязательно
        /// </summary>
        public override BaseRobot MakeCopy()
        {
            var bot = new SignallerRobot
            {
                SenderTitle = SenderTitle,
                CandlesForScreenshot = CandlesForScreenshot
            };
            CopyBaseSettings(bot);
            return bot;
        }

        public SignallerRobot()
        {
            lastMessages = new List<string>();
        }

        public override void Initialize(BacktestServerProxy.RobotContext grobotContext, CurrentProtectedContext protectedContextx)
        {
            base.Initialize(grobotContext, protectedContextx);
        }

        /// <summary>
        /// Здесь я узнаю все котировки по открытым графикам, запрашиваю по ним время старта, отстоящее на 
        /// CandlesForScreenshot таймфреймов назад от текущего (startTrade)
        /// </summary>
        public override Dictionary<string, DateTime> GetRequiredSymbolStartupQuotes(DateTime startTrade)
        {
            var allTickers = robotContext.GetAllUsedTickers();
            allTickers.AddRange(Graphics);
            allTickers = allTickers.Distinct().ToList();

            var historyStart = allTickers.ToDictionary(t => t,
                t => t.b.GetDistanceTime(CandlesForScreenshot, -1, startTrade));
            var tickers = historyStart.Select(t => t.Key.a).Distinct().ToList();
            return tickers.ToDictionary(t => t, t => historyStart.Where(h => h.Key.a == t).Min(h => h.Value));
        }

        /// <summary>
        /// пока, за неимением лучшего, новые ордера проверяем в этом методе
        /// например, сравниваем старый список Id ордеров (private Hashset<string> currentOrders = new ...) с новым
        /// 
        /// но лучше где-нибудь подцепить к базовому роботу (BaseRobot) либо к RobotContext 
        /// обработчик события - новый ордер
        /// </summary>
        public override List<string> OnQuotesReceived(string[] names,
            CandleDataBidAsk[] quotes, bool isHistoryStartOff)
        {
            var events = new List<string>(0);
            if (isHistoryStartOff) return events;

            nowTime = quotes.Max(q => q.timeOpen);

            // ..

            return events;
        }


        /// <summary>
        /// три задачи:
        /// - получить свечки
        /// - нарисовать свечки на канве - например, написать простой класс для отрисовки свечек... это несложно
        /// - сохранить свечки во временный файл, чтобы как-то зааттачить этот файл в телегу
        /// </summary>
        private void MakeCandlesScreenshot(string symbol, BarSettings timeframe)
        {
            var candles = GetCandles(symbol, timeframe);

            // нарисовать и сохранить
            
        }

        private List<CandleData> GetCandles(string symbol, BarSettings timeframe)
        {
            var startTime = timeframe.GetDistanceTime(CandlesForScreenshot, -1, nowTime);
            var stream = robotContext.GetMinuteCandlesPacked(symbol, startTime, nowTime);
            var packer = new CandlePacker(timeframe);
            var candles = new List<CandleData>(CandlesForScreenshot + 1);
            foreach (var quote in stream.GetCandles())
            {
                var candle = packer.UpdateCandle(quote.close, quote.timeOpen.AddMinutes(1));
                if (candle != null)
                    candles.Add(candle);
            }
            return candles;
        }
    }
    // ReSharper restore LocalizableElement
}
