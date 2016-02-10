using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeSharp.Contract.Util.BL;
using TradeSharp.FakeUser.BL;
using TradeSharp.Linq;
using TradeSharp.Util;

namespace TradeSharp.FakeUser
{
    public partial class CloseAccountsDlg : Form
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        

        public CloseAccountsDlg()
        {
            InitializeComponent();
            worker.DoWork += WorkerOnDoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += (sender, args) =>
            {
                btnCloseAccounts.Text = "Закрыть счета";
            };            
        }

        private void btnCloseAccounts_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                return;
            }

            var accountIds = tbAccounts.Text.ToIntArrayUniform();
            if (accountIds.Length == 0) return;
            worker.RunWorkerAsync(accountIds);
            btnCloseAccounts.Text = "Остановить";
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var accountIds = (int[]) doWorkEventArgs.Argument;

            var completeedCount = 0;
            ReportCompleted(0, accountIds.Length);

            using (var ctx = DatabaseContext.Instance.Make())
            {
                foreach (var id in accountIds)
                {
                    if (worker.CancellationPending) break;
                    CloseAccount(id, ctx);
                    ReportCompleted(completeedCount++, accountIds.Length);
                }
            }
        }

        private void CloseAccount(int id, TradeSharpConnection ctx)
        {
            try
            {
                new AccountCleaner().CloseAccount(id, ctx);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in CloseAccount(" + id + "): " + ex);
            }
            
        }

        private void ReportCompleted(int count, int total)
        {
            Invoke(new Action<int, int>((c, t) =>
            {
                lblCompleted.Text = $"{c} из {t} закрыто";
            }), count, total);
        }

        private void CloseAccountsDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
