using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Entity;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.Robot.Robot
{
    [DisplayName("Контр-нетто робот")]
    public class CounterNettoRobot : BaseRobot
    {
        class NettoRecord
        {
            public DateTime time;

            public readonly Dictionary<string, double> nettoBySymbol = new Dictionary<string, double>();

            private static string[] symbols;

            public static void ReadHeader(StreamReader sr)
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    symbols = line.Split(new [] {' ', ';', ',', (char) 9}, StringSplitOptions.RemoveEmptyEntries);
                    if (symbols.Length > 0) break;
                }
            }

            public static NettoRecord ReadNext(StreamReader sr)
            {
                if (symbols == null || symbols.Length == 0) return null;

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    // 10.05.2016 22:00 275000 -82000
                    var parts = line.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < symbols.Length + 2) continue;

                    DateTime time;
                    if (!DateTime.TryParseExact(parts[0] + " " + parts[1], "dd.MM.yyyy HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out time)) continue;

                    var record = new NettoRecord
                    {
                        time = time
                    };

                    for (var i = 0; i < symbols.Length; i++)
                    {
                        var numStr = parts[i + 2];
                        var num = numStr.ToDoubleUniformSafe() ?? 0;
                        record.nettoBySymbol.Add(symbols[i], num);
                    }

                    return record;
                }

                return null;
            }
        }

        #region Настройки
        [PropertyXMLTag("Robot.NettoFilePath")]
        [LocalizedDisplayName("TitleFile")]
        [LocalizedCategory("TitleMain")]
        [Description("Путь к файлу данных")]
        [Editor("Candlechart.Indicator.FileBrowseUITypeEditor, System.Drawing.Design.UITypeEditor",
            typeof(UITypeEditor))]
        public string NettoFilePath { get; set; }

        [PropertyXMLTag("Robot.NettoCoeff")]
        [DisplayName("К. объема входа")]
        [Category("Основные")]
        [Description("Коэффициент объема входа")]
        public decimal NettoCoeff { get; set; } = 0.1M;

        #endregion

        #region Переменные

        private Dictionary<string, CandlePacker> packers;

        private StreamReader nettoReader;

        private NettoRecord cur, next;

        private readonly Dictionary<string, CandleDataBidAsk> lastPrices = new Dictionary<string, CandleDataBidAsk>();
        #endregion

        public override BaseRobot MakeCopy()
        {
            var bot = new CounterNettoRobot
            {
                NettoFilePath = NettoFilePath,
                NettoCoeff = NettoCoeff
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
                Logger.DebugFormat("CounterNettoRobot: настройки графиков не заданы");
                return;
            }

            packers = Graphics.ToDictionary(g => g.a, g => new CandlePacker(g.b));

            if (!File.Exists(NettoFilePath))
            {
                Logger.DebugFormat("CounterNettoRobot: файл не найден");
                return;
            }
            nettoReader = new StreamReader(NettoFilePath, Encoding.ASCII);
            NettoRecord.ReadHeader(nettoReader);
            cur = NettoRecord.ReadNext(nettoReader);
            next = NettoRecord.ReadNext(nettoReader);
        }

        public override Dictionary<string, DateTime> GetRequiredSymbolStartupQuotes(DateTime startTrade)
        {
            if (Graphics.Count == 0) return null;
            return new Dictionary<string, DateTime> { { Graphics[0].a, startTrade } };
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            nettoReader?.Close();
        }

        public override List<string> OnQuotesReceived(string[] names, CandleDataBidAsk[] quotes, bool isHistoryStartOff)
        {
            var events = new List<string>();

            var candleBySmb = new Dictionary<string, CandleData>();
            for (var i = 0; i < names.Length; i++)
            {
                lastPrices[names[i]] = quotes[i];
                CandlePacker packer;
                if (!packers.TryGetValue(names[i], out packer)) continue;
                var candle = packer.UpdateCandle(quotes[i]);
                if (candle != null) candleBySmb.Add(names[i], candle);
            }

            if (isHistoryStartOff) return events;

            if (candleBySmb.Count == 0) return events;

            var time = candleBySmb.First().Value.timeClose;
            var record = MoveToNettos(time);
            if (record == null) return events;

            // сравнить купленные / проданные объемы с нетто
            List<MarketOrder> orders;
            GetMarketOrders(out orders);
            var ordersBySmb = orders.GroupBy(o => o.Symbol).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var smbVolume in record.nettoBySymbol)
            {
                var volumeSigned = (decimal)smbVolume.Value * NettoCoeff;
                var sign = Math.Sign(volumeSigned);
                var nettoVolume = RoundVolume(Math.Abs(volumeSigned));

                List<MarketOrder> smbOrders;
                if (!ordersBySmb.TryGetValue(smbVolume.Key, out smbOrders))
                    smbOrders = new List<MarketOrder>();

                // скомпенсировать нетто-позицию покупками или продажами
                BalanceNetto(smbVolume.Key, sign, nettoVolume, smbOrders);
            }

            return events;
        }

        private void BalanceNetto(string smb, int sign, int newVolume, List<MarketOrder> smbOrders)
        {
            var oldVolume = smbOrders.Sum(o => o.Side * o.Volume);
            var oldSign = Math.Sign(oldVolume);
            oldVolume = Math.Abs(oldVolume);
            
            if ((oldVolume == 0 && newVolume == 0) ||
                (oldVolume == newVolume && sign == oldSign)) return;

            if (oldVolume != 0 && newVolume == 0)
            {
                // закрыть все ордера
                foreach (var order in smbOrders)
                    CloseMarketOrder(order.ID);
                return;
            }
            if (oldVolume == 0 && newVolume != 0)
            {
                // открыть новый ордер
                OpenOrder(smb, sign, newVolume);
                return;
            }

            // закрыть все ордера и открыть новый
            if (sign != oldSign)
            {
                foreach (var order in smbOrders)
                    CloseMarketOrder(order.ID);
                OpenOrder(smb, sign, newVolume);
                return;
            }

            // долить
            if (oldVolume < newVolume)
            {
                OpenOrder(smb, sign, newVolume - oldVolume);
                return;
            }

            // закрыть N самых маленьких сделок и долить до нужного значения
            smbOrders = smbOrders.OrderBy(o => o.Volume).ToList();
            while (smbOrders.Count > 0)
            {
                oldVolume -= smbOrders[0].Volume;
                CloseMarketOrder(smbOrders[0].ID);
                if (oldVolume <= newVolume) break;
                smbOrders.RemoveAt(0);
            }
            if (oldVolume != newVolume)
                OpenOrder(smb, sign, newVolume - oldVolume);
        }

        private void OpenOrder(string smb, int sign, int nettoVolume)
        {
            var quote = lastPrices[smb];
            NewOrder(new MarketOrder
            {
                Symbol = smb,
                Side = sign,
                Volume = nettoVolume,
                Comment = "netto correction",
                Magic = Magic
            }, OrderType.Market, (decimal) (sign == 1 ? quote.closeAsk : quote.close), 100);
        }

        private int RoundVolume(decimal volume)
        {
            if (volume < RoundMinVolume)
            {
                if (RoundType == VolumeRoundType.Вверх) return RoundMinVolume;
                if (RoundType == VolumeRoundType.Вниз) return 0;
                return (volume < RoundMinVolume*0.75M) ? 0 : RoundMinVolume;
            }

            var stepsFract = (volume - RoundMinVolume) / RoundVolumeStep;
            var steps = RoundType == VolumeRoundType.Вверх
                ? Math.Ceiling(stepsFract)
                : RoundType == VolumeRoundType.Вниз
                    ? Math.Floor(stepsFract)
                    : Math.Round(stepsFract);
            return RoundMinVolume + (int) steps * RoundVolumeStep;
        }

        private NettoRecord MoveToNettos(DateTime closeTime)
        {
            if (cur == null) return null;
            if (cur.time == closeTime) return cur;
            if (cur.time > closeTime) return null;

            if (next != null)
            {
                if (next.time > closeTime) return cur;
                if (next.time == closeTime)
                {
                    cur = next;
                    next = NettoRecord.ReadNext(nettoReader);
                    return cur;
                }

                while (true)
                {
                    cur = next;
                    next = NettoRecord.ReadNext(nettoReader);
                    if (next == null)
                    {
                        cur = null;
                        return null;
                    }
                    if (next.time > closeTime) return cur;
                    if (next.time == closeTime)
                    {
                        cur = next;
                        next = NettoRecord.ReadNext(nettoReader);
                        return cur;
                    }
                }
            }

            return null;
        }
    }
}
