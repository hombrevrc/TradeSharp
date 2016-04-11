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
        private int skipCandles = 1;
        [PropertyXMLTag("Robot.TakeRange")]
        [DisplayName("Коэффициент TakeRange, %")]
        [Category("Торговые")]
        [Description("Входить на первой - второй - ... свече")]
        public int SkipCandles
        {
            get { return skipCandles; }
            set
            {
                if (value > 0 && value <= 100)
                    skipCandles = value;
            }
        }

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

        private int highLevel = 50;
        [PropertyXMLTag("Robot.HighLevel")]
        [DisplayName("Верхняя граница Каймана")]
        [Category("Торговые")]
        [Description("Верхняя граница коридора Каймана")]
        public int HighLevel { get; set; } = 50;

        private int lowLevel = 50;
        [PropertyXMLTag("Robot.LowLevel")]
        [DisplayName("Нижняя граница Каймана")]
        [Category("Торговые")]
        [Description("Нижняя граница коридора Каймана")]
        public int LowLevel { get; set; } = 50;
        // ReSharper restore ConvertToAutoProperty
        #endregion

        #region Переменные

        private string ticker;
        private CandlePacker packer;
        private Dictionary<DateTime, CandleData> caymanCandles;

        /// <summary>
        /// знаки Каймана
        /// </summary>
        private RestrictedQueue<int> caymanLastSigns;
        #endregion

        public override BaseRobot MakeCopy()
        {
            var bot = new CaymanRobot
            {
                CaymanFilePath = CaymanFilePath,
                SkipCandles = SkipCandles,
                FixedVolume = FixedVolume,
                StopLossPoints = StopLossPoints,
                TakeProfitPoints = TakeProfitPoints,
                Leverage = Leverage,
                RoundType = RoundType,
                NewsChannels = NewsChannels,
                RoundMinVolume = RoundMinVolume,
                RoundVolumeStep = RoundVolumeStep,
                HighLevel = HighLevel,
                LowLevel = LowLevel
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
            caymanLastSigns = new RestrictedQueue<int>(SkipCandles);
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
            var sign = cayCandle.close >= 50 ? -1 : 1;
            caymanLastSigns.Add(sign);
            if (caymanLastSigns.Length < caymanLastSigns.MaxQueueLength) return events;

            // последняя сделка
            List<MarketOrder> orders;
            GetMarketOrders(out orders);
            var lastOrder = orders.LastOrDefault();

            // закрыть ордер
            if (lastOrder != null && sign != lastOrder.Side)
            {
                CloseMarketOrder(lastOrder.ID);
                lastOrder = null;
            }

            // войти в рынок?
            if (lastOrder != null) return events;
            if (caymanLastSigns.Any(s => s != sign)) return events;
            OpenDeal(candle.close, sign);

            return events;
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

            caymanCandles = new Dictionary<DateTime, CandleData>();
            var timeframe = Graphics[0].b.Intervals[0];

            using (var sr = new StreamReader(CaymanFilePath))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    var parts = line.Split(new[] { ' ', (char)9 }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 7) continue;
                    var dateStr = parts[0] + " " + parts[1];

                    //KS_EURUSD Bid
                    //TIME	OPEN	HIGH	LOW	CLOSE	VOLUME	
                    //29/07/15 11:00:00	41,18	41,18	40,37	40,52	12,00
                    DateTime date;
                    if (!DateTime.TryParseExact(dateStr, "dd/MM/yy HH:mm:ss", CultureProvider.Common, DateTimeStyles.None, out date))
                        continue;
                    var open = parts[2].Replace(',', '.').ToFloatUniformSafe();
                    var high = parts[3].Replace(',', '.').ToFloatUniformSafe();
                    var low = parts[4].Replace(',', '.').ToFloatUniformSafe();
                    var close = parts[5].Replace(',', '.').ToFloatUniformSafe();

                    if (!open.HasValue || !close.HasValue || !high.HasValue || !low.HasValue)
                        continue;
                    caymanCandles.Add(date, new CandleData(open.Value, high.Value, low.Value, close.Value,
                        date, date.AddMinutes(timeframe)));
                }
            }
        }
    }
    // ReSharper restore LocalizableElement
}
