using System.Windows.Forms;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    public partial class CaymanExtremumSignalScriptDlg : Form
    {
        public int LowerMargin
        {
            get
            {
                var values = tbMarginLevels.Text.ToIntArrayUniform();
                return values.Length > 0 ? values[0] : 35;
            }
        }

        public int UpperMargin
        {
            get
            {
                var values = tbMarginLevels.Text.ToIntArrayUniform();
                return values.Length > 1 ? values[1] : 65;
            }
        }

        public int CandlesCount
        {
            get { return tbCandlesCount.Text.ToIntSafe() ?? 2; }
        }

        public bool RemoveOldSigns
        {
            get { return cbRemoveSigns.Checked; }
        }

        public CaymanExtremumSignalScriptDlg()
        {
            InitializeComponent();
        }
    }
}
