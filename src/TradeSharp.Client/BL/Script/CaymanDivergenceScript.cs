using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class CaymanDivergenceScript : CaymanScript
    {
        public enum CaymanCandlePrice
        {
            Close = 0,
            OpenClose,
            AllPrices
        }

        #region Переменные состояния
        private CaymanCandlePrice checkedPrices;
        #endregion

        public CaymanDivergenceScript()
        {
            ScriptTarget = TerminalScriptTarget.График;
            ScriptName = "Сигналы по Кайману";
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            this.chart = chart;
            CommentSpecName = "CaymanDivergenceScript";
            LineMagic = 120;
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
