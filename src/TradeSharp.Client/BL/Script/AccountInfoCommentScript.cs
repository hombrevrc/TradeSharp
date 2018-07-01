using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Candlechart;
using Candlechart.ChartMath;
using Candlechart.Series;
using TradeSharp.Client.Util;
using TradeSharp.Contract.Entity;
using TradeSharp.Contract.Util.Proxy;
using TradeSharp.SiteBridge.Lib.Finance;
using TradeSharp.SiteBridge.Lib.Quotes;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    [LocalizedDisplayName("TitleAccountInfoCommentScript")]
    public class AccountInfoCommentScript : TerminalScript
    {
        private const string CommentSpecName = "ScriptAccountInfo";
        private string chartSymbol;

        public AccountInfoCommentScript()
        {
            ScriptName = Localizer.GetString("TitleAccountInfoCommentScript");
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            chartSymbol = chart.Symbol;
            var comment = chart
                .seriesComment
                .data
                .FirstOrDefault(c => c.Name == CommentSpecName);

            // удалить существующий
            if (comment != null)
            {
                chart.seriesComment.data.Remove(comment);
                chart.RedrawChartSafe();
                return string.Empty;
            }

            var accountData = AccountStatus.Instance.AccountData;
            var accountStatistics = TradeSharpAccountStatistics.Instance.proxy.GetAccountProfit1000(accountData.ID);
            var ordersBySymbol = GetMarketOrders(accountData.ID).Where(x => x.Symbol == chart.Symbol);
            var scriptText = GetCommentText(accountData, accountStatistics, ordersBySymbol);

            comment = new ChartComment
            {
                FillTransparency = 80,
                ColorFill = Color.Gray,
                HideArrow = true,
                ArrowAngle = 90,
                ArrowLength = 1,
                PivotIndex = worldCoords.X,
                PivotPrice = worldCoords.Y,
                Owner = chart.seriesComment,
                Name = CommentSpecName,
                Text = scriptText,
                ColorText = Color.Black,
                Color = Color.Black
            };
            chart.seriesComment.data.Add(comment);
            chart.RedrawChartSafe();
            return string.Empty;
        }

        public override string ActivateScript(string ticker)
        {
            throw new Exception("Неверный тип вызова скрипта \"AccountInfoCommentScript\"");
        }

        public override string ActivateScript(bool byTrigger)
        {
            if (!byTrigger)
                throw new Exception("Неверный тип вызова скрипта \"AccountInfoCommentScript\"");

            var accountData = AccountStatus.Instance.AccountData;
            

            // обновить комментарии на графиках
            var charts = MainForm.Instance.GetChartList(true);
            var accountStatistics = TradeSharpAccountStatistics.Instance.proxy.GetAccountProfit1000(accountData.ID);
            var orders = GetMarketOrders(accountData.ID);

            foreach (var chart in charts)
            {
                var comment = chart.seriesComment.data.FirstOrDefault(c => c.Name == CommentSpecName);
                if (comment == null) continue;

                var ordersBySymbol = orders.Where(x => x.Symbol == chart.Symbol);
                comment.Text = GetCommentText(accountData, accountStatistics, ordersBySymbol);
            }

            return string.Empty;
        }

        /// <summary>
        /// формируем текст для коментария
        /// </summary>
        private string GetCommentText(Account accountData, List<EquityOnTime> accountStatistics, IEnumerable<MarketOrder> ordersBySymbol)
        {
            var result = new StringBuilder();
            var profitStatistic = GetProfitStatistic(accountStatistics, accountData.Balance, accountData.Equity);
            var ordersStatistic = GetOrdersStatistic(ordersBySymbol);
            result.Append(profitStatistic);
            result.Append(ordersStatistic);
            return result.ToString();
        }

        private StringBuilder GetProfitStatistic(List<EquityOnTime> accountStatistics, decimal accountBalance, decimal accountEquity)
        {
            var result = new StringBuilder();
            var balanceChangesText = "- ";
            var balanceChangesPercentText = "- ";
            var currentDrawDownText = "- ";
            var maxDrawDownText = "- ";

            try
            {
                if (accountStatistics.Count > 1)
                {
                    var equtyByDay = accountStatistics.Skip(Math.Max(0, accountStatistics.Count - 7)).Select(x => x.equity).ToList();
                    var profitWeek = 0F;
                    for (int i = 0; i < equtyByDay.Count - 1; i++)
                        profitWeek += (equtyByDay[i + 1] - equtyByDay[i]);
                    var profitWeekPercent
                        = (equtyByDay.Last() - equtyByDay.First()) / equtyByDay.Last();
                    var profitToday =
                        equtyByDay[equtyByDay.Count - 1] - equtyByDay[equtyByDay.Count - 2];
                    var profitTodayPercent =
                        (equtyByDay[equtyByDay.Count - 1] - equtyByDay[equtyByDay.Count - 2]) / equtyByDay[equtyByDay.Count - 1];

                    balanceChangesText = $"{profitToday:F3} / {profitWeek:F3}";
                    balanceChangesPercentText = $"{profitTodayPercent * 100:F3} % / {profitWeekPercent * 100:F3} %";

                    var stat = new AccountStatistics();
                    var maxDrawDown = stat.CalculateMaxDrawdown(accountStatistics);
                    maxDrawDownText = $"{maxDrawDown:F3} %";

                    var currentDrawDown = accountEquity < accountBalance 
                        ? 100 * (accountEquity - accountBalance) / accountBalance 
                        : 0;
                    currentDrawDownText = $"{currentDrawDown} %";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка скрипта AccountInfoCommentScript", ex);
            }

            result.AppendLine($"[b] Баланс {accountBalance}");
            result.AppendLine($"[b] Прирост за сутки / неделю: {balanceChangesText}");
            result.AppendLine($"[b] Прирост за сутки / неделю (%/%): {balanceChangesPercentText}");
            result.AppendLine($"[b] Текущее draw down (%): {currentDrawDownText}");
            result.AppendLine($"[b] Max draw down (%): {maxDrawDownText}");

            return result;
        }

        private StringBuilder GetOrdersStatistic(IEnumerable<MarketOrder> ordersBySymbol)
        {
            var result = new StringBuilder();
            var pipChangeText = "- ";
            var pipAverageText = "- ";
            var successDealPercentText = "- ";

            try
            {
                if (ordersBySymbol != null || ordersBySymbol.Count() > 0)
                {
                    var pips = ordersBySymbol.Where(x => x.ResultPoints != 0).Select(x => x.ResultPoints).ToArray();
                    pipChangeText = $"{pips.Sum():F3}";
                    pipAverageText = $"{pips.Average():F3}";

                    //Ордера с ResultDepo == 0 отфильтровываем
                    var successDeals = ordersBySymbol.Count(x => x.ResultDepo > 0);
                    var successDealPercent = (float)successDeals / ordersBySymbol.Count(x => x.ResultDepo != 0);

                    successDealPercentText = $"{successDealPercent * 100:F3} %";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка скрипта AccountInfoCommentScript", ex);
            }

            result.AppendLine($"[b] Прирост в pip: {pipChangeText}");
            result.AppendLine($"[b] Среднее значение pip: {pipAverageText}");
            result.AppendLine($"[b] % успешных сделок: {successDealPercentText}");

            return result;
        }

        private List<BalanceChange> GetBalanceChanges(int accountId, DateTime startDate)
        {
            List<BalanceChange> balanceChanges;
            var status = TradeSharpAccount.Instance.proxy.GetBalanceChanges(accountId, startDate, out balanceChanges);
            if (status != RequestStatus.OK || balanceChanges == null)
                return null;

            return balanceChanges;
        }

        private List<MarketOrder> GetMarketOrders(int accountId, DateTime? startDate = null)
        {
            List<MarketOrder> historyOrders = null;
            var status =  TradeSharpAccount.Instance.proxy.GetHistoryOrders(accountId, startDate, out historyOrders);

            if (status != RequestStatus.OK || historyOrders == null)
                return null;

            return historyOrders;
        }
    }
}