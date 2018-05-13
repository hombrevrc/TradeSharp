using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Candlechart;
using Candlechart.ChartMath;
using TradeSharp.Client.Util;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    [LocalizedDisplayName("TitleAccountInfoCommentScript")]
    public class AccountInfoCommentScript : TerminalScript
    {
        private const string CommentSpecName = "AccountInfo";

        public AccountInfoCommentScript()
        {
            ScriptName = Localizer.GetString("TitleAccountInfoCommentScript"); ;
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            throw new NotImplementedException();
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
            var scriptText = $"[b] Balance {accountData.Balance}";
            return scriptText;
        }
    }
}