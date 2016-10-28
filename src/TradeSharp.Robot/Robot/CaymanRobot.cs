using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using Entity;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.Robot.Robot
{
    // ReSharper disable LocalizableElement
    [DisplayName("Торговля по Кайману")]
    class CaymanRobot : BaseRobot
    {
        #region Настройки
        [PropertyXMLTag("Robot.CaymanFilePath")]
        [LocalizedDisplayName("TitleFile")]
        [LocalizedCategory("TitleMain")]
        [Description("Путь к файлу данных")]
        [Editor("Candlechart.Indicator.FileBrowseUITypeEditor, System.Drawing.Design.UITypeEditor",
            typeof(UITypeEditor))]
        public string CaymanFilePath { get; set; }


        // ReSharper disable ConvertToAutoProperty
        private int stopLossPoints;
        [PropertyXMLTag("Robot.StopLossPoints")]
        [DisplayName("Стоплосс, пп")]
        [Category("Торговые")]
        [Description("Стоплосс, пунктов. 0 - не задан")]
        public int StopLossPoints
        {
            get { return stopLossPoints; }
            set { stopLossPoints = value; }
        }

        private int takeProfitPoints;
        [PropertyXMLTag("Robot.TakeProfitPoints")]
        [DisplayName("Тейкпрофит, пп")]
        [Category("Торговые")]
        [Description("Тейкпрофит, пунктов. 0 - не задан")]
        public int TakeProfitPoints
        {
            get { return takeProfitPoints; }
            set { takeProfitPoints = value; }
        }

        [PropertyXMLTag("Robot.HighLevel")]
        [DisplayName("Верхняя граница Каймана")]
        [Category("Торговые")]
        [Description("Верхняя граница коридора Каймана")]
        public int HighLevel { get; set; } = 50;

        [PropertyXMLTag("Robot.LowLevel")]
        [DisplayName("Нижняя граница Каймана")]
        [Category("Торговые")]
        [Description("Нижняя граница коридора Каймана")]
        public int LowLevel { get; set; } = 50;

        public enum CloseSignalType { ПоУровню = 0, По50Процентам }

        [PropertyXMLTag("Robot.CloseSignal")]
        [DisplayName("Сигнал для выхода")]
        [Category("Торговые")]
        [Description("По обратному сигналу (выход из коридора) / по пересечению 50%")]
        public CloseSignalType CloseSignal { get; set; }

        // ReSharper restore ConvertToAutoProperty
        #endregion

        #region Переменные

        private string ticker;
        private CandlePacker packer;
        private Dictionary<DateTime, CandleData> caymanCandles;

        /// <summary>
        /// знаки Каймана
        /// </summary>
        private int lastCaymanSign;
        #endregion

        public override BaseRobot MakeCopy()
        {
            var bot = new CaymanRobot
            {
                CaymanFilePath = CaymanFilePath,
                FixedVolume = FixedVolume,
                StopLossPoints = StopLossPoints,
                TakeProfitPoints = TakeProfitPoints,
                Leverage = Leverage,
                RoundType = RoundType,
                NewsChannels = NewsChannels,
                RoundMinVolume = RoundMinVolume,
                RoundVolumeStep = RoundVolumeStep,
                HighLevel = HighLevel,
                LowLevel = LowLevel,
                CloseSignal = CloseSignal
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
                Logger.DebugFormat("CaymanRobot: настройки графиков не заданы");
                return;
            }
            if (Graphics.Count > 1)
            {
                Logger.DebugFormat("CaymanRobot: настройки графиков должны описывать один тикер / один ТФ");
                return;
            }
            packer = new CandlePacker(Graphics[0].b);
            ticker = Graphics[0].a;            
            ReadCaymanHistory();  
        }

        public override Dictionary<string, DateTime> GetRequiredSymbolStartupQuotes(DateTime startTrade)
        {
            if (Graphics.Count == 0) return null;
            return new Dictionary<string, DateTime> { { Graphics[0].a, startTrade } };
        }

        public override List<string> OnQuotesReceived(string[] names, CandleDataBidAsk[] quotes, bool isHistoryStartOff)
        {
            var events = new List<string>();
            var candle = UpdateCurrentCandle(names, quotes);
            if (candle == null) return events;

            // проверить Каймана
            CandleData cayCandle;
            if (!caymanCandles.TryGetValue(candle.timeOpen, out cayCandle))
                return events;

            // знак Каймана
            var enterSign = GetEnterSign(cayCandle.close);

            // последняя сделка
            bool stillOpened;
            CloseExisting(cayCandle.close, enterSign, out stillOpened);

            // войти в рынок?
            if (stillOpened || enterSign == 0) return events;
            OpenDeal(candle.close, enterSign);

            return events;
        }

        private int GetEnterSign(float caymanClose)
        {
            var sign = caymanClose >= HighLevel ? 1 : caymanClose <= LowLevel ? -1 : 0;
            if (sign == lastCaymanSign) return 0;
            lastCaymanSign = sign;
            return -sign; // > High ? SELL : < LOW ? BUY
        }

        private void CloseExisting(float caymanClose, int enterSign, out bool stillOpened)
        {
            stillOpened = false;
            List<MarketOrder> orders;
            GetMarketOrders(out orders);
            var lastOrder = orders.LastOrDefault();
            if (lastOrder == null) return;

            var caymanSign = enterSign;
            if (CloseSignal == CloseSignalType.По50Процентам)
                caymanSign = caymanClose < 50 ? 1 : -1;

            // закрыть ордер
            if (lastOrder != null && caymanSign != lastOrder.Side && caymanSign != 0)
            {
                CloseMarketOrder(lastOrder.ID);
                lastOrder = null;
            }
            stillOpened = lastOrder != null;
        }

        private CandleData UpdateCurrentCandle(string[] names, CandleDataBidAsk[] quotes)
        {
            if (string.IsNullOrEmpty(ticker)) return null;
            var tickerIndex = -1;
            for (var i = 0; i < names.Length; i++)
                if (names[i] == ticker)
                {
                    tickerIndex = i;
                    break;
                }
            if (tickerIndex < 0) return null;
            var quote = quotes[tickerIndex];

            var candle = packer.UpdateCandle(quote);
            return candle;
        }

        private void OpenDeal(float price, int dealSide)
        {
            var dealVolumeDepo = CalculateVolume(ticker, Leverage, FixedVolume);
            if (dealVolumeDepo == 0) return;

            var sl = stopLossPoints == 0 ? (float?)null 
                : price - dealSide * DalSpot.Instance.GetAbsValue(ticker, (float)stopLossPoints);
            var tp = takeProfitPoints == 0 ? (float?)null 
                : price + dealSide * DalSpot.Instance.GetAbsValue(ticker, (float)takeProfitPoints);

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
                    StopLoss = sl,
                    TakeProfit = tp,
                    ExpertComment = "CaymanRobot"
                },
            OrderType.Market, 0, 0);
        }

        private void ReadCaymanHistory()
        {
            if (string.IsNullOrEmpty(CaymanFilePath))
            {
                Logger.DebugFormat("Робот по Кайману - путь к файлу Каймана не задан");
                return;
            }
            if (!File.Exists(CaymanFilePath))
            {
                Logger.DebugFormat("Робот по Кайману - файл ({0}) не найден", CaymanFilePath);
                return;
            }

            var timeframe = Graphics[0].b.Intervals[0];

            var candles = CsvReader.ReadCandles(CaymanFilePath, timeframe);
            caymanCandles = candles.ToDictionary(c => c.timeOpen, c => c);            
        }
    }
    // ReSharper restore LocalizableElement
}
