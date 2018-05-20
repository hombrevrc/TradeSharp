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

        public AccountInfoCommentScript()
        {
            ScriptName = Localizer.GetString("TitleAccountInfoCommentScript");
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
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
            var scriptText = GetCommentText(accountData);

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
            var scriptText = GetCommentText(accountData);

            // обновить комментарии на графиках
            var charts = MainForm.Instance.GetChartList(true);
            foreach (var chart in charts)
            {
                var comment = chart.seriesComment.data.FirstOrDefault(c => c.Name == CommentSpecName);
                if (comment == null) continue;

                comment.Text = scriptText;
            }

            return string.Empty;
        }

        /// <summary>
        /// формируем текст для коментария
        /// </summary>
        private string GetCommentText(Account accountData)
        {
            var result = new StringBuilder();
            /*
            var balanceChanges = GetBalanceChanges(accountData.ID, DateTime.Now.AddDays(-7));
            var orders = GetMarketOrders(accountData.ID, DateTime.Now.AddDays(-7));
            var quotes = QuoteStorage.Instance.proxy.GetQuoteData();
            */
            
            var balanceChangesText = "[b] Balance changes: - ";
            var balanceChangesPercentText = "[b] Balance changes (percent): - ";
            var pipChangeText = "[b] Pip change: - ";
            var pipAverageText = "[b] Pip average: - ";
            var successDealPercentText = "[b] Success deal (percent): - ";

            var currentDrawDownText = "[b] Current draw down (percent): - ";
            var maxDrawDownText = "[b] Max draw down (percent): - ";


            var accountStatistics = TradeSharpAccountStatistics.Instance.proxy.GetAccountProfit1000(accountData.ID);
            if (accountStatistics.Count > 1)
            {
                var equtyByDay = accountStatistics.Skip(Math.Max(0, accountStatistics.Count - 7)).Select(x => x.equity).ToList();
                var profitWeek = 0F;
                for (int i = 0; i < equtyByDay.Count - 1 ; i++)
                    profitWeek += (equtyByDay[i + 1] - equtyByDay[i]);
                var profitWeekPercent 
                    = (equtyByDay.Last() - equtyByDay.First()) / equtyByDay.Last();
                var profitToday =
                    equtyByDay[accountStatistics.Count - 1] - equtyByDay[accountStatistics.Count - 2];
                var profitTodayPercent = 
                    (equtyByDay[accountStatistics.Count - 1] - equtyByDay[accountStatistics.Count - 2]) / equtyByDay[accountStatistics.Count - 1];
                
                balanceChangesText = $"[b] Balance changes: {profitToday:F3} / {profitWeek:F3}";
                balanceChangesPercentText = $"[b] Balance changes (percent): {profitTodayPercent * 100:F3} % / {profitWeekPercent * 100:F3} %";

                var stat = new AccountStatistics();
                var maxDrawDown = stat.CalculateDrawdown(accountStatistics);
                maxDrawDownText = $"[b] Max draw down (percent): {maxDrawDown:F3} %";

                if (accountData.Equity < accountData.Balance)
                    currentDrawDownText =
                        $"[b] Current draw down (percent): {100 * (accountData.Equity - accountData.Balance) / accountData.Balance} %"; 
            }

            var orders = GetMarketOrders(accountData.ID);
            if (orders != null || orders.Count > 0)
            {
                var pips = orders.Where(x => x.ResultPoints != 0).Select(x => x.ResultPoints).ToArray();
                pipChangeText = $"[b] Pip change: {pips.Sum():F3}";
                pipAverageText = $"[b] Pip average: {pips.Average():F3}";

                //Ордера с ResultDepo == 0 отфильтровываем
                var failDeals = orders.Count(x => x.ResultDepo < 0);
                var successDeals = orders.Count(x => x.ResultDepo > 0);
                var successDealPercent = failDeals > 0
                    ? successDeals / failDeals
                    : successDeals > 0
                        ? 1 : 0;

                successDealPercentText = $"[b] Success deals (percent): {successDealPercent * 100:F3} %";
            }

            //Statistics.MaxRelDrawDown
            result.AppendLine($"[b] Balance {accountData.Balance}");
            result.AppendLine(balanceChangesText);
            result.AppendLine(balanceChangesPercentText);
            result.AppendLine(pipChangeText);
            result.AppendLine(currentDrawDownText);
            result.AppendLine(maxDrawDownText);
            result.AppendLine(successDealPercentText);
            result.AppendLine(pipAverageText);
            return result.ToString();
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