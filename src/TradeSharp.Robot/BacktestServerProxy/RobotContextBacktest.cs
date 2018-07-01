using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Candlechart.Indicator;
using Entity;
using TradeSharp.Contract.Entity;
using TradeSharp.TradeLib;
using TradeSharp.Util;
using TradeSharp.Util.Serialization;

namespace TradeSharp.Robot.BacktestServerProxy
{
    // Функции для загрузки истории котировок и раздачи их роботам
    public partial class RobotContextBacktest : RobotContext
    {
        private UpdateTickersCacheForRobotsExDel updateTickersCacheForRobots;

        public bool LogRobots { get; set; }

        public const float DefaultSpreadPoints = 1.5f;

        public override Account AccountInfo { get; set; }

        private readonly Func<List<Cortege2<string, BarSettings>>> getUsedTickers;

        #region Основные настройки
        private DateTime timeFrom = DateTime.MinValue;
        [DisplayName("Дата от:")]
        [Description("Дата начала тестирования")]
        [Category("Основные")]
        public DateTime TimeFrom
        {
            get { return timeFrom; }
            set { timeFrom = value; }
        }

        private DateTime timeTo = DateTime.Now;
        [DisplayName("Дата до:")]
        [Description("Дата окончания тестирования")]
        [Category("Основные")]
        public DateTime TimeTo
        {
            get { return timeTo; }
            set { timeTo = value; }
        }

        private bool updateTickerCache = true;
        [DisplayName("Обновлять котировки")]
        [Description("Обновлять котировки из БД при старте теста")]
        [Category("Основные")]
        public bool UpdateTickerCache { get { return updateTickerCache; } set { updateTickerCache = value; } }
        #endregion

        #region Информация о тесте
        public DateTime? lastTestStart, lastTestEnd;

        public DateTime? startModelTime, endModelTime;
        #endregion

        #region Переменные состояния

        private int nextOrderId = 1;
        
        /// <summary>
        /// список для открытых позиций
        /// </summary>
        private List<MarketOrder> positions = new List<MarketOrder>();

        public List<MarketOrder> Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        private List<MarketOrder> posHistory = new List<MarketOrder>();

        public List<MarketOrder> PosHistory
        {
            get { return posHistory; }
            set { posHistory = value; }
        }

        public List<MarketOrder> AllOrders
        {
            get
            {
                var posList = PosHistory;
                posList.AddRange(Positions);
                return posList.OrderBy(o => o.ID).ToList();
            }
        }

        /// <summary>
        /// список для отложенных ордеров
        /// </summary>
        private readonly List<PendingOrder> orders = new List<PendingOrder>();
        
        /// <summary>
        /// история - закрытые позиции
        /// </summary>
        private readonly List<PendingOrder> ordHistory = new List<PendingOrder>();

        /// <summary>
        /// время наступления каждого стопаута
        /// </summary>
        public readonly List<DateTime> stopoutEventTimes = new List<DateTime>();

        /// <summary>
        /// предыдущая котировка по каждому тикеру
        /// нужна для проверки условия срабатывания ордеров
        /// </summary>
        private IStorage<string, QuoteData> previousQuotes;

        private List<Cortege2<string, CandleData>> finalCandles;

        private decimal dailyHwm, hwm, maxDailyDrawdown, maxDrawdown, maxAbsDrawdown;

        private DateTime highWaterMarkTime, drawdownRelTime, drawDownStartTime;

        private DateTime? prevModelTime;

        private DateTime curModelTime;

        public decimal MaxDailyDrawDown => maxDailyDrawdown;

        public decimal MaxDrawDown => maxDrawdown;

        public decimal MaxAbsDrawDown => maxAbsDrawdown;

        public DateTime DrawDownStartTime => drawDownStartTime;

        public DateTime DrawdownRelTime => drawdownRelTime;
        #endregion

        #region Предустановленные переменные

        public AccountGroup groupDefault = new AccountGroup
                                                {
                                                    Code = "Default",
                                                    BrokerLeverage = 50,
                                                    IsReal = false,
                                                    MarginCallPercentLevel = 50,
                                                    StopoutPercentLevel = 33,
                                                    Name = "Тестовая группа счетов"
                                                };

        #endregion

        #region Параметры теста
        private BacktestTickerCursor testCursor;

        private decimal initialTestBalance;

        private bool historyStartoffPassed;

        private DateTime? firstDateOfTest;
        #endregion

        public RobotContextBacktest(UpdateTickersCacheForRobotsExDel updateTickersCacheForRobots, Func<List<Cortege2<string, BarSettings>>> getUsedTickers)
        {
            this.updateTickersCacheForRobots = updateTickersCacheForRobots;
            this.getUsedTickers = getUsedTickers;
        }

        public bool InitiateTest()
        {
            nextOrderId = 1;
            lastTestStart = DateTime.Now;
            firstDateOfTest = null;
            startModelTime = null;
            endModelTime = null;
            hwm = 0;
            maxAbsDrawdown = 0;
            maxDailyDrawdown = 0;
            maxDrawdown = 0;
            // начальная инициализация
            robotLogEntries.Clear();
            previousQuotes = new UnsafeStorage<string, QuoteData>();
            quotesStorage = new UnsafeStorage<string, QuoteData>();
            InitTradeLib();
            
            // запомнить баланс торгового счета и вернуть его в исходное состояние
            // по завершению теста
            initialTestBalance = AccountInfo.Balance;
            historyStartoffPassed = false;

            OnRobotMessage -= LogRobotMessages;
            if (LogRobots)
                OnRobotMessage += LogRobotMessages;
            
            // получить от гроботов используемые тикеры и старт истории
            // роботы сами указывают используемые тикеры, плюс также берутся тикеры из параметра Graphics
            var usedTickers = GetUserTickersAndStartTime();
            if (usedTickers.Count == 0)
            {
                Logger.DebugFormat("Бэк-тест портфеля из {0} роботов: не указаны используемые и торгуемые котировки", 
                    listRobots.Count);
                return false;
            }

            if (UpdateTickerCache)
                updateTickersCacheForRobots(usedTickers,  3);

            // открыть файловые потоки
            testCursor = new BacktestTickerCursor();            
            // !! path !!
            var path = ExecutablePath.ExecPath + "\\quotes";
            try
            {
                var cursorStart = usedTickers.Min(t => t.Value);
                if (!testCursor.SetupCursor(path, usedTickers.Keys.ToList(), cursorStart))
                {
                    testCursor.Close();
                    testCursor = null;
                    Logger.DebugFormat(
                        "Бэк-тест портфеля из {0} роботов: не удалось установить курсор, путь \"{1}\"",
                        listRobots.Count, path);
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (testCursor != null) testCursor.Close();
                testCursor = null;
                Logger.Error("Ошибка в SetupTest", ex);
                return false;
            }
            return true;
        }
        
        public void FinalizeTest()
        {
            if (testCursor != null)
            {
                finalCandles = testCursor.GetCurrentQuotes();
                testCursor.Close();
            }
            testCursor = null;
            AccountInfo.Balance = initialTestBalance;
            lastTestEnd = DateTime.Now;
            CalcProfitForOpenOrder();
        }

        /// <summary>
        /// переместиться на ближ. следующий момент времени в архиве из N котировок
        /// скормить котировки роботам
        /// роботы разродятся ордерами (возможно)
        /// 
        /// перед вызовами MakeStep() необходимо вызвать InitiateTest
        /// </summary>        
        public bool MakeStep(out DateTime modelTime, out DateTime firstRealDateOfTest)
        {
            modelTime = TimeFrom;
            firstRealDateOfTest = TimeFrom;
            var candles = testCursor.GetCurrentQuotes();
            if (candles.Count == 0) return true;

            modelTime = candles[0].b.timeOpen;
            curModelTime = modelTime;
            startModelTime = startModelTime ?? modelTime;
            endModelTime = modelTime;

            if (!firstDateOfTest.HasValue)
                firstDateOfTest = modelTime;
            firstRealDateOfTest = firstDateOfTest.Value;
            
            if (!historyStartoffPassed && modelTime >= TimeFrom)
                historyStartoffPassed = true;            

            var names = candles.Select(q => q.a).ToArray();
            var candlesBidAsk = candles.Select(q =>
                {
                    var spread = DalSpot.Instance.GetAbsValue(q.a, DefaultSpreadPoints);
                    return new CandleDataBidAsk(q.b, spread);
                }
                ).ToArray();
            var quotes = candlesBidAsk.Select((c, i) =>
                                              new QuoteData(c.close, c.closeAsk, c.timeClose)).ToArray();
            // обновить хранилище котировок
            quotesStorage.UpdateValues(names, quotes);

            // проверить стопаут
            var isSo = tradeManager.CheckStopOut(AccountInfo.ID);
            if (isSo)
            {// закрыть все сделки по стопауту
                stopoutEventTimes.Add(modelTime);
                var idToCloseList = positions.Select(p => p.ID).ToList();
                foreach (var posId in idToCloseList)
                    SendCloseRequest(null, AccountInfo.ID, posId, PositionExitReason.Stopout);
            }

            // проверить отложенные и открытые ордера, маржу и плечо)
            for (var i = 0; i < names.Length; i++)
                CheckDeals(names[i], quotes[i]);

            // дать роботам котировки в работу
            OnQuotesReceived(names, candlesBidAsk, !historyStartoffPassed);
            
            // обновить кривые средств и экспозиции
            if (historyStartoffPassed)
            {
                UpdateDailyEquityExposure(modelTime.Date);
                // дневное "проседание"
                CalcMaxDailyDrawDown(modelTime);
            }
                    
            previousQuotes.UpdateValues(names, quotes);
            if (!testCursor.MoveNext()) return true;
            if (timeTo < modelTime.Date) return true;
            return false;
        }

        private void CalcMaxDailyDrawDown(DateTime modelTime)
        {
            //if (highWaterMarkTime == default(DateTime)) highWaterMarkTime = modelTime;
            //if (drawdownRelTime == default(DateTime)) drawdownRelTime = modelTime;
            
            if (historyStartoffPassed && prevModelTime.HasValue)
            {
                if (prevModelTime.Value.Date != modelTime.Date)
                    dailyHwm = AccountInfo.Equity;
                else
                {
                    if (AccountInfo.Equity > dailyHwm) dailyHwm = AccountInfo.Equity;
                    else
                    {
                        var drawDown = dailyHwm == 0
                            ? 0
                            : AccountInfo.Equity <= 0
                                ? -100
                                : -100 * (dailyHwm - AccountInfo.Equity) / dailyHwm;
                        if (drawDown < maxDailyDrawdown)
                            maxDailyDrawdown = drawDown;
                    }
                }
            }

            if (AccountInfo.Equity > hwm)
            {
                hwm = AccountInfo.Equity;
                highWaterMarkTime = modelTime;
            }
            else
            {
                var drawDown = hwm == 0
                            ? 0
                            : AccountInfo.Equity <= 0
                                ? -100
                                : -100 * (hwm - AccountInfo.Equity) / hwm;
                var absDrawdown = AccountInfo.Equity - hwm;
                if (absDrawdown < maxAbsDrawdown) maxAbsDrawdown = absDrawdown;
                if (drawDown < maxDrawdown)
                {
                    maxDrawdown = drawDown;
                    drawdownRelTime = modelTime;
                    drawDownStartTime = highWaterMarkTime;
                }
            }

            prevModelTime = modelTime;
        }

        public Dictionary<string, DateTime> GetUserTickersAndStartTime()
        {
            var allTickers = new Dictionary<string, DateTime>();
            var tradeTickers = new List<string>();

            foreach (var robot in listRobots)
            {                
                var tickers = robot.GetRequiredSymbolStartupQuotes(timeFrom);
                if (robot.Graphics != null && robot.Graphics.Count > 0)
                    tradeTickers.AddRange(robot.Graphics.Select(g => g.a).ToList());
                if (tickers == null) continue;
                foreach (var ticker in tickers)
                {
                    if (allTickers.ContainsKey(ticker.Key))
                    {
                        if (ticker.Value < allTickers[ticker.Key])
                            allTickers[ticker.Key] = ticker.Value;
                    }
                    else allTickers.Add(ticker.Key, ticker.Value);
                }
            }

            tradeTickers = tradeTickers.Distinct().ToList();

            // получить список тикеров, необходимых для перевода
            // базовой (плечо) и контрвалюты (прибыль) в валюту депо)
            foreach (var ticker in tradeTickers)
            {
                bool inverse, areEqual;
                var tickerName = DalSpot.Instance.FindSymbol(ticker, false, AccountInfo.Currency, out inverse,
                                                                out areEqual);
                if (!string.IsNullOrEmpty(tickerName) && !allTickers.ContainsKey(tickerName))
                    allTickers.Add(tickerName, timeFrom);
                tickerName = DalSpot.Instance.FindSymbol(ticker, true, AccountInfo.Currency, out inverse,
                                                                out areEqual);
                if (!string.IsNullOrEmpty(tickerName) && !allTickers.ContainsKey(tickerName))
                    allTickers.Add(tickerName, timeFrom);
            }
            return allTickers;
        }

        public override RequestStatus SendNewOrderRequest(ProtectedOperationContext secCtx,
            int requestUniqueId,
            MarketOrder order,
            OrderType orderType,
            decimal requestedPrice,
            decimal slippagePoints)
        {
            // проверить, разрешен ли вход
            decimal equity;
            if (!tradeManager.IsEnterEnabled(AccountInfo.ID, order.Symbol, order.Side, order.Volume, out equity))
            {
                return RequestStatus.MarginOrLeverageExceeded;
            }

            order.PriceEnter = requestedPrice > 0
                                   ? (float) requestedPrice
                                   : (order.Side == 1
                                          ? quotesStorage.ReceiveValue(order.Symbol).ask
                                          : quotesStorage.ReceiveValue(order.Symbol).bid);
            order.TimeEnter = curModelTime; //quotesStorage.ReceiveValue(order.Symbol).time;
            order.State = PositionState.Opened;
            order.ID = nextOrderId++;
            positions.Add(order);
            return RequestStatus.OK;
        }

        public override RequestStatus SendNewPendingOrderRequest(
            ProtectedOperationContext ctx, int requestUniqueId, PendingOrder order)
        {
            orders.Add(order);
            return RequestStatus.OK;
        }

        public override RequestStatus SendCloseRequest(ProtectedOperationContext ctx,
            int accountId, int orderId, PositionExitReason reason)
        {
            var pos = positions.FirstOrDefault(o => o.ID == orderId);
            if (pos == null) return RequestStatus.NotFound;
            var quote = quotesStorage.ReceiveValue(pos.Symbol);
            if (quote == null) return RequestStatus.NoPrice;
            ClosePositionAndCalculateProfit(pos, quote, reason);
            return RequestStatus.OK;
        }

        public override RequestStatus SendCloseByTickerRequest(ProtectedOperationContext ctx, int accountId,
                                                               string ticker, PositionExitReason reason)
        {
            var posToClose = positions.Where(p => p.Symbol == ticker).ToList();
            if (posToClose.Count == 0) return RequestStatus.OK;

            var quote = quotesStorage.ReceiveValue(ticker);
            if (quote == null) return RequestStatus.NoPrice;
            foreach (var pos in posToClose)
                ClosePositionAndCalculateProfit(pos, quote, reason);
            return RequestStatus.OK;
        }

        public override RequestStatus SendDeletePendingOrderRequest(ProtectedOperationContext ctx, PendingOrder order,
            PendingOrderStatus status, int? positionId, string closeReason)
        {
            order.Status = status;
            order.CloseReason = closeReason;
            ordHistory.Add(order);
            orders.Remove(order);
            return RequestStatus.OK;
        }

        public override RequestStatus SendEditMarketRequest(ProtectedOperationContext secCtx, MarketOrder pos)
        {
            foreach (var position in
                positions.Where(position => position.ID == pos.ID && position.AccountID == pos.AccountID))
            {
                // ордер нашли
                position.Magic = pos.Magic;
                position.TakeProfit = pos.TakeProfit;
                position.StopLoss = pos.StopLoss;
                position.Comment = pos.Comment;
                position.ExpertComment = pos.ExpertComment;
                position.Swap = pos.Swap;
                return RequestStatus.OK;
            }
            return RequestStatus.NotFound;
        }

        public override RequestStatus SendEditPendingRequest(ProtectedOperationContext secCtx, PendingOrder ord)
        {
            foreach (var order in orders.Where(order => order.ID == ord.ID && AccountInfo.ID == order.AccountID))
            {
                order.Magic = ord.Magic;
                order.PriceFrom = ord.PriceFrom;
                order.PriceTo = ord.PriceTo;
                order.TimeFrom = ord.TimeFrom;
                order.TimeTo = ord.TimeTo;
                order.Comment = ord.Comment;
                order.ExpertComment = ord.ExpertComment;
                order.StopLoss = ord.StopLoss;
                order.TakeProfit = ord.TakeProfit;
                order.Volume = ord.Volume;
                order.PriceSide = ord.PriceSide;
                order.Side = ord.Side;
                order.Symbol = ord.Symbol;
                return RequestStatus.OK;
            }
            return RequestStatus.NotFound;
        }

        /// <summary>
        /// очистка списков торговых ордеров и всей истории транзакций для отдельного счета
        /// </summary>
        /// <param name="accountId"></param>
        public void ClearDealsForAccount(int accountId)
        {
            foreach (var position in positions.Where(position => position.AccountID == accountId))
                positions.Remove(position);

            foreach (var order in orders.Where(order => order.AccountID == accountId))
                orders.Remove(order);

            foreach (var order in posHistory.Where(order => order.AccountID == accountId))
                posHistory.Remove(order);

            foreach (var order in ordHistory.Where(order => order.AccountID == accountId))
                ordHistory.Remove(order);
        }
        /// <summary>
        /// очистка всех списков торговой истории для бэктестинга
        /// </summary>
        public void ClearAllTradeHistory()
        {
            positions.Clear();
            orders.Clear();
            posHistory.Clear();
            ordHistory.Clear();
            dailyEquityExposure.Clear();
            stopoutEventTimes.Clear();
        }

        public override RequestStatus GetMarketOrders(int accountId, out List<MarketOrder> ordlist)
        {
            if (accountId == AccountInfo.ID)
            {
                ordlist = positions;
                return RequestStatus.OK;
            }
            ordlist = null;
            return RequestStatus.NotFound;
        }

        public override RequestStatus GetHistoryOrdersCompressed(int? accountId, DateTime? startDate, out byte[] buffer)
        {
            buffer = null;
            List<MarketOrder> ordlist;
            var status = GetHistoryOrders(accountId, startDate, out ordlist);
            if (ordlist == null || ordlist.Count == 0)
                return status;

            using (var writer = new SerializationWriter())
            {
                writer.Write(orders);
                writer.Flush();
                buffer = writer.ToArray();
            }

            return status;
        }

        public override RequestStatus GetHistoryOrders(int? accountId, DateTime? startDate, out List<MarketOrder> ordlist)
        {
            ordlist = new List<MarketOrder>();
            if (AccountInfo.ID == (accountId ?? AccountInfo.ID))
            {
                ordlist.AddRange(posHistory.Where(order => order.TimeEnter >= (startDate ?? order.TimeEnter)));
                return RequestStatus.OK;
            }
            ordlist = null;
            return RequestStatus.NotFound;
        }

        public override RequestStatus GetHistoryOrdersByCloseDate(int? accountId, DateTime? startDate, out List<MarketOrder> ordlist)
        {
            ordlist = new List<MarketOrder>();
            if (AccountInfo.ID == (accountId ?? AccountInfo.ID))
            {
                ordlist.AddRange(posHistory.Where(order => order.TimeExit >= startDate || (!startDate.HasValue)));
                return RequestStatus.OK;
            }
            ordlist = null;
            return RequestStatus.NotFound;
        }

        public override RequestStatus GetOrdersByFilter(int accountId, bool getClosedOrders, OrderFilterAndSortOrder filter, out List<MarketOrder> orders)
        {
            var listPos = getClosedOrders ? posHistory : positions;
            orders = filter.ApplyFilter(listPos.AsQueryable()).ToList();
            return RequestStatus.OK;
        }

        public override RequestStatus GetPendingOrders(int accountId, out List<PendingOrder> ordlist)
        {
            if (accountId == AccountInfo.ID)
            {
                ordlist = orders;
                return RequestStatus.OK;
            }
            ordlist = null;
            return RequestStatus.NotFound;
        }

        private void CheckDeals(string symbol, QuoteData quote)
        {
            CheckPendingOrders(symbol, quote);
            CheckMarketOrders(symbol, quote);                      
        }

        private void CheckMarketOrders(string symbol, QuoteData quote)
        {
            var ordersToClose = tradeManager.CheckMarketOrders(AccountInfo.ID);
            foreach (var pos in ordersToClose.Where(pos => symbol == pos.Symbol))
            {
                ClosePositionAndCalculateProfit(pos, quote, pos.ExitReason);
            }

            // обсчитываем оставшиеся незакрытые позиции
            foreach (var pos in Positions.Where(pos => pos.Symbol == symbol))
            {
                // позиция не закрылась, пересчитываем прибыль
                if (pos.PriceBest == null)
                    pos.PriceBest = quote.bid;
                if (pos.PriceWorst == null)
                    pos.PriceWorst = quote.bid;
                if (pos.Side == 1)
                {
                    if (pos.PriceBest < quote.bid)
                        pos.PriceBest = quote.bid;

                    if (pos.PriceWorst > quote.bid)
                        pos.PriceWorst = quote.bid;
                }
                else
                {
                    if (pos.PriceBest > quote.bid)
                        pos.PriceBest = quote.bid;

                    if (pos.PriceWorst < quote.bid)
                        pos.PriceWorst = quote.bid;
                }
            }  
        }

        private void CheckPendingOrders(string symbol, QuoteData quote)
        {
            var execOrders = GetFireingPendgingOrders(symbol, quote);
            foreach (var ord in execOrders)
            {
                // ордер сработал, открываем позицию и удаляем его
                if (SendNewOrderRequest(null,
                                        0,
                                        new MarketOrder
                                            {
                                                AccountID = AccountInfo.ID,
                                                Magic = ord.Magic,
                                                Symbol = ord.Symbol,
                                                Volume = ord.Volume,
                                                Side = ord.Side,
                                                StopLoss = ord.StopLoss,
                                                TakeProfit = ord.TakeProfit,
                                                Comment = ord.Comment,
                                                ExpertComment = ord.ExpertComment
                                            }, OrderType.Market,
                                        (decimal) ord.PriceFrom, 0) != RequestStatus.OK) continue;

                positions[positions.Count - 1].PendingOrderID = ord.ID;
                orders.Remove(ord);                
            }
        }

        private List<PendingOrder> GetFireingPendgingOrders(string symbol, QuoteData quote)
        {
            var execList = new List<PendingOrder>();
            for (var i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                // ордер устарел?
                if (order.TimeTo.HasValue && quote.time > order.TimeTo.Value)
                {
                    orders.RemoveAt(i);
                    i--;
                    continue;
                }
                if (order.Symbol != symbol) continue;
                // время еще не подошло?
                if (order.TimeFrom.HasValue && order.TimeFrom.Value > quote.time) continue;
                // цена должна пробить заявку снизу-вверх (1) или сверху вниз (-1)
                var priceDirection = (order.Side == 1 && order.PriceSide == PendingOrderType.Stop) ||
                                     (order.Side == -1 && order.PriceSide == PendingOrderType.Limit)
                                         ? 1 : -1;
                var checkedPrice = order.Side > 0 ? quote.ask : quote.bid;
                var priceOk = priceDirection > 0
                                  ? (checkedPrice >= order.PriceFrom && (!order.PriceTo.HasValue ||
                                                                         order.PriceTo.Value <= order.PriceTo))
                                  : (checkedPrice <= order.PriceFrom && (!order.PriceTo.HasValue ||
                                                                         order.PriceTo.Value >= order.PriceTo));
                if (!priceOk) continue;
                // цена должна перешагнуть рубеж
                if (!order.TimeFrom.HasValue)
                {
                    var prevQuote = previousQuotes.ReceiveValue(symbol);
                    if (prevQuote == null) continue;
                    checkedPrice = order.Side > 0 ? prevQuote.ask : prevQuote.bid;
                    priceOk = priceDirection > 0
                                  ? checkedPrice < order.PriceFrom : checkedPrice > order.PriceFrom;
                }
                if (priceOk) execList.Add(order);
            }
            return execList;
        }

        private void ClosePositionAndCalculateProfit(MarketOrder pos, QuoteData quote, PositionExitReason reason)
        {
            pos.PriceExit = pos.Side < 0 ? quote.ask : quote.bid;
            
            // убираем проскальзывания на истории 
            if (pos.StopLoss != null && pos.StopLoss != 0)
            {
                if ((pos.StopLoss > pos.PriceExit && pos.Side == 1) || (pos.StopLoss < pos.PriceExit && pos.Side == -1))
                    pos.PriceExit = pos.StopLoss;
            }
            else
                if (pos.TakeProfit != null && pos.TakeProfit != 0)
                {
                    if ((pos.TakeProfit < pos.PriceExit && pos.Side == 1) || (pos.TakeProfit > pos.PriceExit && pos.Side == -1))
                        pos.PriceExit = pos.TakeProfit;
                }
            pos.TimeExit = curModelTime; //quote.time;
            pos.State = PositionState.Closed;
            var deltaAbs = pos.Side * (pos.PriceExit.Value - pos.PriceEnter);
            pos.ResultPoints = DalSpot.Instance.GetPointsValue(pos.Symbol, deltaAbs);
            pos.ResultDepo = (float)profitCalculator.CalculatePositionProfit(pos, AccountInfo.Currency, quotesStorage.ReceiveAllData());
            pos.ResultDepo = pos.ResultDepo;
            pos.ExitReason = reason;
            // скорректировать баланс
            AccountInfo.Balance += (decimal)pos.ResultDepo;
            // обновить историю и убрать позицию из списка
            posHistory.Add(pos);
            Positions.Remove(pos);
        }

        public override AccountGroup GetAccountGroup(int accountId)
        {
            var group = base.GetAccountGroup(accountId);
            return group ?? groupDefault;
        }

        public override Account GetAccountInfo(bool needEquity)
        {
            return AccountInfo;
        }

        public override List<Cortege2<string, BarSettings>> GetAllUsedTickers()
        {
            throw new NotImplementedException();
        }

        private void InitTradeLib()
        {
            counterId = 0;
            tradeManager = new TradeManager(this, this,
                quotesStorage, GetAccountGroup);
            profitCalculator = ProfitCalculator.Instance;
        }

        private void CalcProfitForOpenOrder()
        {
            if (finalCandles == null || finalCandles.Count == 0) return;          

            var quotes = finalCandles.ToDictionary(c => c.a, c =>
            {
                var symbol = c.a;
                var bid = c.b.close;
                var pointCost = DalSpot.Instance.GetAbsValue(symbol, 1f);
                var ask = bid + pointCost*DefaultSpreadPoints;
                return new QuoteData(bid, ask, new DateTime());
            });

            foreach (var order in Positions)
            {
                QuoteData quote;
                if (!quotes.TryGetValue(order.Symbol, out quote))
                    continue;
                order.PriceExit = order.Side > 0 ? quote.bid : quote.ask;
                order.ResultPoints = DalSpot.Instance.GetPointsValue(order.Symbol,
                    order.Side * (order.PriceExit.Value - order.PriceEnter));
                order.ResultBase =  order.Volume * order.Side * (order.PriceExit.Value - order.PriceEnter);
                order.ResultDepo = DalSpot.Instance.CalculateProfitInDepoCurrency(order, quotes, AccountInfo.Currency) ?? 0;
            }
        }
    }
}
