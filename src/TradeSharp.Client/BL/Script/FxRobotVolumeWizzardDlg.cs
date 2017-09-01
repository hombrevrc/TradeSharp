using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Entity;
using TradeSharp.Contract.Entity;
using TradeSharp.Contract.Util.BL;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    public partial class FxRobotVolumeWizzardDlg : Form
    {
        public ScriptSetupFxRobotVolumes.FxRobotVolumeSets RobotSets { get; set; }

        public int MaxOrders
        {
            get => tbMaxOrders.Text.ToIntSafe() ?? 1;
            set => tbMaxOrders.Text = value.ToString();
        }

        public string Ticker { get; set; }

        public int RobotsCount
        {
            get => tbRobotsCount.Text.ToIntSafe() ?? 4;
            set => tbRobotsCount.Text = value.ToString();
        }

        private Dictionary<string, QuoteData> quotes;

        private VolumeStep minVolume;

        private TradeTicker contract;

        public FxRobotVolumeWizzardDlg()
        {
            InitializeComponent();
        }

        private void FxRobotVolumeWizzardDlg_Load(object sender, EventArgs e)
        {
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // посчитать объемы
            var maxLever = tbMaxLeverage.Text.ToDecimalUniformSafe() ?? 10;
            var startLever = tbStartLever.Text.ToDecimalUniformSafe() ?? 10;
            var robotsCount = RobotsCount;
            var maxOrders = MaxOrders;
            quotes = QuoteStorage.Instance.ReceiveAllData();
            minVolume = DalSpot.Instance.GetMinStepLot(Ticker, RobotSets.Account.Group);
            contract = DalSpot.Instance.GetTickerByName(Ticker);

            // стартовый объем в валюте ордера
            var volm = LeverageToOrderVolume(startLever / robotsCount);
            if (volm == 0) return;
            
            // округлить объем            
            var startVolume = MarketOrder.RoundDealVolume(volm, VolumeRoundType.Ближайшее,
                minVolume.minVolume, minVolume.volumeStep);
            startVolume = Math.Max(startVolume, minVolume.minVolume);

            // посчитать шаг объема
            var step = 0;
            if (maxOrders > 1)
            {
                var totalVolume = LeverageToOrderVolume(maxLever / robotsCount);
                step = 2 * (totalVolume - startVolume * maxOrders) / (maxOrders * (maxOrders - 1));
            }
            RobotSets.VolumeStep = step;
            RobotSets.VolumeStart = startVolume;
            CommentOnVolumes();
            DialogResult = DialogResult.OK;
        }

        private int LeverageToOrderVolume(decimal leverage)
        {
            var depoVolume = leverage * RobotSets.Account.Equity;            
            var volm = DalSpot.Instance.ConvertSourceCurrencyToTargetCurrency(contract.ActiveBase,
                RobotSets.Account.Currency, (double)depoVolume, quotes, out var error);
            if (volm == null)
            {
                lblComment.Text = error;
                return 0;
            }
            return (int)volm.Value;
        }

        private void CommentOnVolumes()
        {
            // посчитать объемы после округления
            var volumes = new List<int> {RobotSets.VolumeStart};
            for (var i = 1; i < MaxOrders; i++)
            {
                var volm = RobotSets.VolumeStart + RobotSets.VolumeStep * i;
                volm = MarketOrder.RoundDealVolume(volm, VolumeRoundType.Ближайшее,
                    minVolume.minVolume, minVolume.volumeStep);
                volumes.Add(volm);
            }
            // посчитать сумму и плечо
            var total = volumes.Sum();
            var totalDepo = DalSpot.Instance.ConvertSourceCurrencyToTargetCurrency(RobotSets.Account.Currency,
                contract.ActiveBase, total, quotes, out var error);
            var lever = totalDepo * RobotsCount / RobotSets.Account.Equity;
            // вывести сообщение
            var volStr = string.Join(", ", volumes.Select(v => v.ToStringUniformMoneyFormat()));
            RobotSets.Message = $"Объемы: {volStr}, итоговое плечо {lever:F2}, роботов: {RobotsCount}";
        }
    }
}
