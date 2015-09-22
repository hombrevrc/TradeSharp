using System.Windows.Forms;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    public partial class CaymanDivergenceSetupDlg : Form
    {
        public int SkipCandles
        {
            get { return tbSkipCandles.Text.ToIntSafe() ?? 2; }
        }

        public CaymanDivergenceScript.CaymanCandlePrice CheckedPrices
        {
            get
            {
                return cbCandlePrice.SelectedIndex == 0
                    ? CaymanDivergenceScript.CaymanCandlePrice.Close
                    : cbCandlePrice.SelectedIndex == 1
                        ? CaymanDivergenceScript.CaymanCandlePrice.OpenClose
                        : CaymanDivergenceScript.CaymanCandlePrice.AllPrices;
            }
        }

        public bool RemoveOldSigns
        {
            get { return cbRemoveSigns.Checked; }
        }

        public CaymanDivergenceSetupDlg()
        {
            InitializeComponent();
            cbCandlePrice.SelectedIndex = 0;
        }
    }
}
