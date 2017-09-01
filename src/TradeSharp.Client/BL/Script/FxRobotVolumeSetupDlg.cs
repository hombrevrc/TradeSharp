using System.Windows.Forms;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    public partial class FxRobotVolumeSetupDlg : Form
    {
        public ScriptSetupFxRobotVolumes.FxRobotVolumeSets RobotSets { get; set; }

        private string ticker;

        public FxRobotVolumeSetupDlg()
        {
            InitializeComponent();
        }

        public FxRobotVolumeSetupDlg(ScriptSetupFxRobotVolumes.FxRobotVolumeSets robotSets)
        {
            InitializeComponent();
            RobotSets = robotSets;
            ticker = robotSets.TargetRobot.Graphics[0].a;
            var msgRobot = robotSets.RobotSource == ScriptSetupFxRobotVolumes.RobotSourceType.РоботыLive
                ? (robotSets.FarmIsStopped ? "live-торговля, пауза" : "live-торговля, запущена (настройки не будут применены)")
                : "тестер стратегий";
            lblCaption.Text = msgRobot + ", " + ticker;
            tbMaxOrders.Text = robotSets.TargetRobot.MaxDealsInSeries.ToString();
            tbStartVolume.Text = (robotSets.TargetRobot.FixedVolume ?? 0).ToStringUniformMoneyFormat();
            tbVolumeStep.Text = robotSets.TargetRobot.VolumeStep.ToStringUniformMoneyFormat();
        }

        private void btnRecalcVolumes_Click(object sender, System.EventArgs e)
        {
            var dlg = new FxRobotVolumeWizzardDlg
            {
                MaxOrders = tbMaxOrders.Text.ToIntSafe() ?? 0,
                Ticker = ticker,
                RobotSets = RobotSets,
                RobotsCount = tbRobotsCount.Text.ToIntSafe() ?? 4
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            tbStartVolume.Text = RobotSets.VolumeStart.ToStringUniformMoneyFormat();
            tbVolumeStep.Text = RobotSets.VolumeStep.ToStringUniformMoneyFormat();
            tbMessage.Text = RobotSets.Message;
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (RobotSets.RobotSource == ScriptSetupFxRobotVolumes.RobotSourceType.РоботыLive &&
                    RobotSets.FarmIsStopped == false)
            {
                tbMessage.Text = "Невозможно применить настройки, торговля в процессе";
                return;
            }
            RobotSets.TargetRobot.VolumeStep = IntFromText(tbVolumeStep.Text);
            RobotSets.TargetRobot.FixedVolume = IntFromText(tbStartVolume.Text);
            RobotSets.TargetRobot.MaxDealsInSeries = IntFromText(tbMaxOrders.Text);
            DialogResult = DialogResult.OK;
        }

        private static int IntFromText(string txt)
        {
            return txt.Replace(" ", "").ToIntSafe() ?? 0;
        }
    }
}
