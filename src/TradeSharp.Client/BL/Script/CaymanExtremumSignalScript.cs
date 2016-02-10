using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Candlechart;
using Candlechart.ChartMath;
using Candlechart.Indicator;
using Candlechart.Series;
using Entity;

namespace TradeSharp.Client.BL.Script
{
    [DisplayName("Торговля от экстремумов Каймана")]
    public class CaymanExtremumSignalScript : CaymanScript
    {       
        #region Переменные состояния
        private int upperMargin, lowerMargin;
        #endregion

        public CaymanExtremumSignalScript()
        {
            ScriptTarget = TerminalScriptTarget.График;
            ScriptName = "Торговля от экстремумов Каймана";
            CommentSpecName = "CaymanExtremumSignalScript";
            LineMagic = 140;
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            this.chart = chart;
            var dlg = new CaymanExtremumSignalScriptDlg();
            if (dlg.ShowDialog() == DialogResult.Cancel) return "";
            skippedCandles = dlg.CandlesCount;
            removeOldSigns = dlg.RemoveOldSigns;
            upperMargin = dlg.UpperMargin;
            lowerMargin = dlg.LowerMargin;
            return BuildSeries();
        }

        public override string ActivateScript(string ticker)
        {
            throw new Exception("Неверный тип вызова скрипта \"CaymanExtremumSignalScript\"");
        }

        public override string ActivateScript(bool byTrigger)
        {
            throw new Exception("Неверный тип вызова скрипта \"CaymanExtremumSignalScript\"");
        }

        protected string BuildSeries()
        {
            var indi = chart.indicators.FirstOrDefault(i => i.GetType() == typeof(IndicatorExternSeries));
            if (indi == null)
                return "Нет данных для построения (данные из файла)";
            var indiData = indi.SeriesResult[0] as CandlestickSeries;
            if (indiData.DataCount == 0) return "Индикатор пуст";
            var candles = chart.chart.StockSeries.Data.Candles;
            var max = Math.Min(candles.Count, indiData.DataCount);

            var lines = new List<TrendLine>();
            TrendLine trendLine = null;
            var dealSign = 0;
            var caymanSigns = new RestrictedQueue<int>(skippedCandles);

            for (var i = 0; i < max; i++)
            {
                var candle = indiData.Data[i];
                var chartCandle = candles[i];
                var extremumSign = candle.close < lowerMargin ? -1 : candle.close > upperMargin ? 1 : 0;
                caymanSigns.Add(extremumSign);

                if (dealSign != 0)
                {
                    trendLine.AddPoint(i, chartCandle.close);
                    if ((dealSign < 0 && candle.close > 50) ||
                        (dealSign > 0 && candle.close < 50))
                    {
                        trendLine = null;
                        dealSign = 0;
                    }
                    continue;
                }

                dealSign = caymanSigns.Last;
                if (dealSign == 0) continue;
                if (caymanSigns.Any(s => s != dealSign))
                {
                    dealSign = 0;
                    continue;
                }

                trendLine = new TrendLine
                {
                    Comment = CommentSpecName,
                    Magic = LineMagic,
                    LineColor = dealSign > 0 ? colorSell : ColorBuy
                };
                trendLine.AddPoint(i, chartCandle.close);
                trendLine.AddPoint(i, chartCandle.close);
                lines.Add(trendLine);                
            }

            MakeChartGraph(lines);
            return "Построено " + lines.Count + " областей";
        }
    }
}
