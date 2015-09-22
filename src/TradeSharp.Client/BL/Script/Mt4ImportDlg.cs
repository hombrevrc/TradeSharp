using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TradeSharp.Contract.Entity;

namespace TradeSharp.Client.BL.Script
{
    public partial class Mt4ImportDlg : Form
    {
        private readonly List<MarketOrder> orders;

        public List<MarketOrder> selectedOrders;

        public Mt4ImportDlg()
        {
            InitializeComponent();
        }

        public Mt4ImportDlg(List<MarketOrder> orders) : this()
        {
            lblOrdersTotal.Text = orders.Count.ToString();
            this.orders = orders;
            ClearInterval();
        }

        private void ClearInterval()
        {
            dpStart.Value = orders.Min(o => o.TimeEnter);
            dpEnd.Value = orders.Max(o => o.TimeExit ?? o.TimeEnter);
            selectedOrders = orders;
            lblSelected.Text = selectedOrders.Count.ToString();
        }

        private void dpEnd_ValueChanged(object sender, EventArgs e)
        {
            selectedOrders = orders.Where(o => o.TimeEnter > dpStart.Value && o.TimeEnter < dpEnd.Value).ToList();
            lblSelected.Text = selectedOrders.Count.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (selectedOrders.Count == 0) return;
            DialogResult = DialogResult.OK;
        }
    }
}
