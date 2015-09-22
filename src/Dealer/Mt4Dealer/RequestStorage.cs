using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BrokerService.Contract.Entity;
using TradeSharp.Util;

namespace Mt4Dealer
{
    public class RequestStorage
    {
        private readonly List<RequestedOrder> tradeRequestList = new List<RequestedOrder>();

        private readonly ReaderWriterLockSlim requestLocker = new ReaderWriterLockSlim();

        private const int LockTimeout = 1000;

        private readonly int checkFloodSeconds = AppConfig.GetIntParam("Mt4Dealer.CheckFloodSeconds", 15);

        private readonly int checkFloodMsgPerSecond = AppConfig.GetIntParam("Mt4Dealer.MsgPerSecond", 15);

        private readonly int checkFloodIntervalSeconds = AppConfig.GetIntParam("Mt4Dealer.CheckFloodIntervalSeconds", 5);

        private readonly ThreadSafeTimeStamp lastTimeFloodChecked = new ThreadSafeTimeStamp();

        #region Singleton
        private static readonly Lazy<RequestStorage> instance = new Lazy<RequestStorage>(() => new RequestStorage());

        public static RequestStorage Instance
        {
            get { return instance.Value; }
        }

        private RequestStorage()
        {
        }
        #endregion

        public void StoreRequest(RequestedOrder request)
        {
            if (!requestLocker.TryEnterWriteLock(LockTimeout))
            {
                Logger.Error("Таймаут сохранения запроса (RequestStorage)");
                return;
            }

            try
            {
                tradeRequestList.Add(request);
            }
            finally
            {
                requestLocker.ExitWriteLock();
            }
        }

        public RequestedOrder FindRequest(int reqId)
        {
            if (!requestLocker.TryEnterWriteLock(LockTimeout))
            {
                Logger.Error("Таймаут извлечения запроса (RequestStorage)");
                return null;
            }

            try
            {
                var index = tradeRequestList.FindIndex(r => r.request.Id == reqId);
                if (index < 0) return null;
                var req = tradeRequestList[index];
                tradeRequestList.RemoveAt(index);
                return req;
            }
            finally
            {
                requestLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// если за последние N (к примеру, 60)
        /// секунд накопилось больше N * K сообщений (к примеру, больше 60 * 20) -
        /// вернуть True
        /// </summary>
        public bool CheckOverflood()
        {
            var lastCheckedTime = lastTimeFloodChecked.GetLastHitIfHitted();
            var secondsSinceLastCheck = lastCheckedTime == null
                ? int.MaxValue
                : (DateTime.Now - lastCheckedTime.Value).TotalSeconds;
            if (secondsSinceLastCheck < checkFloodIntervalSeconds) return false;
            lastTimeFloodChecked.Touch();

            requestLocker.EnterReadLock();
            List<DateTime> requestTimes;
            try
            {
                requestTimes = tradeRequestList.Select(r => r.requestTime).ToList();
            }
            finally
            {
                requestLocker.ExitReadLock();
            }
            if (requestTimes.Count < 10) return false;

            var nowTime = DateTime.Now;
            requestTimes = requestTimes.OrderByDescending(t => t).ToList();
            var countInQueue = 0;
            foreach (var time in requestTimes)
            {
                var secondsLeft = (nowTime - time).TotalSeconds;
                if (secondsLeft > checkFloodSeconds) break;
                countInQueue++;
            }
            var maxMessages = checkFloodSeconds * checkFloodMsgPerSecond;
            if (countInQueue > maxMessages)
            {
                Logger.ErrorFormat("Mt4Dealer - flood, {0} messages for {1} seconds ({2} are allowed)",
                    countInQueue, checkFloodSeconds, maxMessages);
                return true;
            }
            return false;
        }
    }

    public class RequestedOrder
    {
        public TradeTransactionRequest request;

        public TradeSharp.Contract.Entity.MarketOrder requestedOrder;

        public readonly DateTime requestTime;

        public RequestedOrder()
        {
            requestTime = DateTime.Now;
        }

        public RequestedOrder(TradeTransactionRequest request, 
            TradeSharp.Contract.Entity.MarketOrder requestedOrder) : this()
        {
            this.request = request;
            this.requestedOrder = requestedOrder;
        }
    }
}
