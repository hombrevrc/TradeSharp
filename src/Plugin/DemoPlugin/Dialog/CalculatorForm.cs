using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Candlechart;
using Candlechart.ChartMath;
using Entity;
using TradeSharp.Util;

namespace DemoPlugin.Dialog
{
    public partial class CalculatorForm : Form
    {
        private readonly CandleChartControl chart;

        private readonly PointD worldCoords;

        private readonly Random rand = new Random();

        private enum OptionType
        {
            Classic = 0, Touch
        }

        public CalculatorForm()
        {
            InitializeComponent();
        }

        public CalculatorForm(CandleChartControl chart, PointD worldCoords) : this()
        {
            this.chart = chart;
            this.worldCoords = worldCoords;
            var time = chart.chart.StockSeries.GetCandleOpenTimeByIndex((int)worldCoords.X);
            var price = worldCoords.Y;
            dpTimeCalc.Value = time;
            dpTimeExpires.Value = time.AddMinutes(chart.Timeframe.TotalMinutes * 200);
            tbPriceAtTime.Text = price.ToStringUniformPriceFormat(true);
            tbStrikePrice.Text = tbPriceAtTime.Text;
        }

        private void btnCalcPremium_Click_1(object sender, System.EventArgs e)
        {
            var optTypeString = (string) cbOptionType.SelectedItem;
            var optionSide = optTypeString.StartsWith("PUT") ? -1 : 1;
            var optionType = optTypeString.EndsWith("TOUCH") ? OptionType.Touch : OptionType.Classic;
            var start = tbStrikePrice.Text.ToDoubleUniform();
            var strike = tbPriceAtTime.Text.ToDoubleUniform();

            var candles = AtomCandleStorage.Instance.GetAllMinuteCandles(chart.Symbol);
            var listOc = candles.Select(c => Math.Abs((double)(c.close - c.open))).OrderBy(c => c).ToList();
            // убрать тренд
            //var avg = listOc.Average();
            //listOc = listOc.Select(l => l - avg).ToList();
            var model = new PriceModel(listOc, 5);

            const int checksCount = 5000;

            var money = 0.0;
            var totalMinutes = (dpTimeExpires.Value - dpTimeCalc.Value).TotalMinutes;
            //var avgTf = chart.chart.Timeframe.Intervals.Average();
            var tfCount = (int) Math.Round(totalMinutes);
            if (tfCount < 1) tfCount = 1;

            var volume = tbVolume.Text.Replace(" ", "").ToInt();
            for (var i = 0; i < checksCount; i++)
            {
                money += MoneyAtStroke(model, start, strike, optionType, optionSide, tfCount, volume);
            }
            var premium = money/checksCount;
            LoggMessageSafe($"Премия: " + premium.ToStringUniformMoneyFormat());
        }

        private double MoneyAtStroke(PriceModel model, double startPrice, double strike, OptionType type, int optSide, int periods, int volume)
        {
            var price = startPrice;
            for (var i = 0; i < periods; i++)
            {
                var delta = model.GetRandomDelta();
                var sign = rand.Next(2) > 0 ? 1 : -1;
                price += delta * sign;
                if (type == OptionType.Touch)
                {
                    if (optSide > 0 && price >= strike) return volume;
                    if (optSide < 0 && price <= strike) return volume;
                }
            }
            if (optSide > 0 && price >= strike) return volume * (price - strike);
            if (optSide < 0 && price <= strike) return volume * (strike - price);
            return 0;
        }

        private void LoggMessageSafe(string msg)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(LoggMessageUnsafe), msg);
            else
                LoggMessageUnsafe(msg);
        }

        private void LoggMessageUnsafe(string msg)
        {
            tbResult.AppendText(msg + Environment.NewLine);
        }
    }
}
