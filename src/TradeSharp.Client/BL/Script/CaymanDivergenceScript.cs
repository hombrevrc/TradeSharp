using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Candlechart;
using Candlechart.ChartMath;
using Candlechart.Indicator;
using Candlechart.Series;
using Entity;

namespace TradeSharp.Client.BL.Script
{
    [DisplayName("Сигналы по Кайману")]
    public class CaymanDivergenceScript : TerminalScript
    {
        public const string CommentSpecName = "CaymanDivergenceScript";
        public const int LineMagic = 120;

        public enum CaymanCandlePrice
        {
            Close = 0,
            OpenClose,
            AllPrices
        }

        #region Параметры

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
        #endregion

        #region Переменные состояния

        private CandleChartControl chart;

        private int skippedCandles;

        private CaymanCandlePrice checkedPrices;

        private bool removeOldSigns;
        #endregion

        public CaymanDivergenceScript()
        {
            ScriptTarget = TerminalScriptTarget.График;
            ScriptName = "Сигналы по Кайману";
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            this.chart = chart;
            var dlg = new CaymanDivergenceSetupDlg();
            if (dlg.ShowDialog() == DialogResult.Cancel) return "";
            skippedCandles = dlg.SkipCandles;
            checkedPrices = dlg.CheckedPrices;
            removeOldSigns = dlg.RemoveOldSigns;
            return BuildSeries();
        }

        public override string ActivateScript(string ticker)
        {
            throw new Exception("Неверный тип вызова скрипта \"ShowRobotOrdersScript\"");
        }

        public override string ActivateScript(bool byTrigger)
        {
            throw new Exception("Неверный тип вызова скрипта \"ShowRobotOrdersScript\"");
        }

        private string BuildSeries()
        {
            var indi = chart.indicators.FirstOrDefault(i => i.GetType() == typeof (IndicatorExternSeries));
            if (indi == null)
                return "Нет данных для построения (данные из файла)";
            var indiData = indi.SeriesResult[0] as CandlestickSeries;
            if (indiData.DataCount == 0) return "Индикатор пуст";
            var candles = chart.chart.StockSeries.Data.Candles;
            var max = Math.Min(candles.Count, indiData.DataCount);

            var lastSign = 0;
            var lastSigns = new RestrictedQueue<int>(skippedCandles);

            var lines = new List<TrendLine>();
            TrendLine trendLine = null;           

            for (var i = 0; i < max; i++)
            {
                var candle = indiData.Data[i];
                var chartCandle = candles[i];
                var thisSign = GetCaymanSign(candle);
                lastSigns.Add(thisSign);

                // растянуть регион
                if (trendLine != null)
                {
                    trendLine.AddPoint(i, chartCandle.close);
                    
                    if (thisSign == lastSign)
                        continue;
                }

                lastSign = thisSign;
                
                // завершить регион
                if (trendLine != null)
                {
                    trendLine = null;
                    continue;
                }

                if (lastSigns.Any(s => s != lastSign) || lastSigns.Length < skippedCandles) continue;
                // новая линия
                trendLine = new TrendLine
                {
                    Comment = CommentSpecName,
                    Magic = LineMagic,
                    LineColor = thisSign > 0 ? colorSell : ColorBuy                    
                };
                trendLine.AddPoint(i, chartCandle.close);
                lines.Add(trendLine);
            }

            MakeChartGraph(lines);
            return "Построено " + lines.Count + " областей";
        }

        private void MakeChartGraph(List<TrendLine> lines)
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
                var deltaPoints = sign*(line.linePoints[1].Y - line.linePoints[0].Y);
                deltaPoints = DalSpot.Instance.GetPointsValue(chart.Symbol, (float) deltaPoints);
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

        private void RemoveOldSigns()
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

        private int GetCaymanSign(CandleData candle)
        {
            const int midCayman = 50;
            var thisSign =
                checkedPrices == CaymanCandlePrice.Close
                    ? (candle.close < midCayman ? -1 : candle.close > midCayman ? 1 : 0)
                    : checkedPrices == CaymanCandlePrice.OpenClose
                        ? (candle.close < midCayman && candle.open < midCayman
                            ? -1
                            : candle.close > midCayman && candle.open > midCayman ? 1 : 0)
                        : (candle.high < midCayman ? -1 : candle.low > midCayman ? 1 : 0);
            return thisSign;
        }
    }
}
