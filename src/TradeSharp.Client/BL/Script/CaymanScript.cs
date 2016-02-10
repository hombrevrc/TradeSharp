using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Candlechart;
using Candlechart.Series;
using Entity;

namespace TradeSharp.Client.BL.Script
{
    public abstract class CaymanScript : TerminalScript
    {
        public Color colorBuy = Color.CadetBlue;
        [Category("Визуальные")]
        [PropertyXMLTag("ColorBuy")]
        [DisplayName("Цвет покупок")]
        [Description("Цвет маркеров - покупок")]
        public Color ColorBuy
        {
            get { return colorBuy; }
            set { colorBuy = value; }
        }

        public Color colorSell = Color.Coral;
        [Category("Визуальные")]
        [PropertyXMLTag("ColorSell")]
        [DisplayName("Цвет продаж")]
        [Description("Цвет маркеров - продаж")]
        public Color ColorSell
        {
            get { return colorSell; }
            set { colorSell = value; }
        }

        protected CandleChartControl chart;

        protected string CommentSpecName { get; set; }

        protected int LineMagic { get; set; }

        protected int skippedCandles;

        protected bool removeOldSigns;

        protected void MakeChartGraph(List<TrendLine> lines)
        {
            if (removeOldSigns)
                RemoveOldSigns();

            int totalLosses = 0, totalProfits = 0;
            double totalPoints = 0;

            foreach (var line in lines)
            {
                line.Owner = chart.seriesTrendLine;
                chart.seriesTrendLine.data.Add(line);
                var sign = line.LineColor == ColorBuy ? 1 : -1;
                var deltaPoints = sign * (line.linePoints[1].Y - line.linePoints[0].Y);
                deltaPoints = DalSpot.Instance.GetPointsValue(chart.Symbol, (float)deltaPoints);
                totalPoints += deltaPoints;
                if (deltaPoints > 0) totalProfits++;
                if (deltaPoints < 0) totalLosses++;

                var comment = new ChartComment
                {
                    Magic = LineMagic,
                    PivotIndex = line.linePoints[1].X,
                    PivotPrice = line.linePoints[1].Y,
                    ArrowAngle = 120,
                    ArrowLength = 30,
                    Color = line.LineColor,
                    ColorText = chart.chart.visualSettings.SeriesForeColor,
                    Text = string.Format("{0:f1} пп", deltaPoints),
                    Owner = chart.seriesComment
                };
                chart.seriesComment.data.Add(comment);
            }

            var message = string.Format("{0:f1} пунктов всего, {1} \"профитов\", {2} \"лоссов\"",
                totalPoints, totalProfits, totalLosses);
            MessageBox.Show(message);
        }

        protected void RemoveOldSigns()
        {
            for (var i = 0; i < chart.seriesTrendLine.DataCount; i++)
            {
                if (chart.seriesTrendLine.data[i].Magic == LineMagic &&
                    chart.seriesTrendLine.data[i].Comment == CommentSpecName)
                {
                    chart.seriesTrendLine.data.RemoveAt(i);
                    i--;
                }
            }

            for (var i = 0; i < chart.seriesComment.DataCount; i++)
            {
                if (chart.seriesComment.data[i].Magic == LineMagic)
                {
                    chart.seriesComment.data.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
