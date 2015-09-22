using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Candlechart;
using Candlechart.ChartMath;
using Candlechart.Series;
using Entity;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    [DisplayName("Импорт сделок MT4 (CSV)")]
    public class ScriptImportMt4CsvOrders : TerminalScript
    {
        private static readonly DateTime defaultTime = new DateTime(1970, 1, 1);

        private int shiftHours = 1;
        [DisplayName("Смещение времени")]
        [Category("Основные")]
        [Description("Смещение времени МТ4 -> T#")]
        [PropertyXMLTag("ShiftHours")]
        public int ShiftHours
        {
            get { return shiftHours; }
            set { shiftHours = value; }
        }

        public override bool CanBeTriggered
        {
            get { return true; }
        }

        public ScriptImportMt4CsvOrders()
        {
            ScriptTarget = TerminalScriptTarget.График;
            ScriptName = "Импорт сделок MT4 (CSV)";
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "*.csv|*.csv|*.*|*.*",
                FilterIndex = 0,
                DefaultExt = "csv",
                Title = "Открыть файл сделок"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return "";
          
            // прочитать ордера
            var allOrders = ReadOrdersFromCsv(dlg.FileName);
            var chartOrders = allOrders.Where(o => o.Symbol.Contains(chart.Symbol)).ToList();
            if (chartOrders.Count == 0)
            {
                var msg = string.Format("{0} ордеров прочитано, ни один не относится к графику {1}",
                    allOrders.Count, chart.Symbol);
                return msg;
            }

            if (allOrders.Count > 1000)
            {
                //var msg = string.Format("Прочитано {0} ордеров. Отобразить ордера на выбранном временном отрезке?",
                //    chartOrders.Count);
                var ordersDlg = new Mt4ImportDlg(chartOrders);
                if (ordersDlg.ShowDialog() == DialogResult.OK)
                    chartOrders = ordersDlg.selectedOrders;
            }

            ShowOrders(chartOrders, chart);
            return string.Empty;
        }

        public override string ActivateScript(string ticker)
        {
            throw new Exception("Неверный тип вызова скрипта \"ScriptImportMt4CsvOrders\"");
        }

        public override string ActivateScript(bool byTrigger)
        {
            throw new Exception("Неверный тип вызова скрипта \"ScriptImportMt4CsvOrders\"");
        }

        private List<MarketOrder> ReadOrdersFromCsv(string path)
        {
            var orders = new List<MarketOrder>();
            if (!File.Exists(path)) return orders;

            try
            {
                using (var sr = new StreamReader(path, Encoding.ASCII))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        var order = ParseFileLine(line);
                        if (order != null) orders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Ошибка чтения CSV ({0}): {1}", path, ex);
            }
            return orders;
        }

        private MarketOrder ParseFileLine(string line)
        {
            var parts = line.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim('"')).ToArray();
            if (parts.Length < 20) return null;

            try
            {
                // TICKET,LOGIN,SYMBOL,DIGITS,CMD,VOLUME,OPEN_TIME,OPEN_PRICE,SL,TP,CLOSE_TIME,
                // EXPIRATION,CONV_RATE1,CONV_RATE2,COMMISSION,COMMISSION_AGENT,SWAPS,CLOSE_PRICE,
                // PROFIT,TAXES,COMMENT,INTERNAL_ID,MARGIN_RATE,TIMESTAMP,MODIFY_TIME,REASON,GW_VOLUME,GW_OPEN_PRICE,GW_CLOSE_PRICE
                var order = new MarketOrder
                {
                    ID = parts[0].ToInt(),
                    AccountID = parts[1].ToInt(),
                    Symbol = parts[2],
                    Side = parts[4].ToInt() == 0 ? 1 : -1,
                    Volume = parts[5].ToInt(),
                    TimeEnter = DateTime.ParseExact(parts[6], 
                        "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).AddHours(shiftHours),
                    PriceEnter = parts[7].ToFloatUniform(),
                    StopLoss = parts[8].ToFloatUniform(),
                    TakeProfit = parts[9].ToFloatUniform(),
                    TimeExit = DateTime.ParseExact(parts[10], 
                        "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    PriceExit = parts[17].ToFloatUniform(),
                    ResultDepo = parts[18].ToFloatUniform(),
                    Comment = parts[20]
                };
                if (order.TimeExit == defaultTime)
                    order.TimeExit = null;
                else 
                    order.TimeExit = order.TimeExit.Value.AddHours(shiftHours);
                if (order.PriceExit == 0) order.PriceExit = null;
                return order;
            }
            catch
            {
                return null;
            }
        }

        private void ShowOrders(List<MarketOrder> orders, CandleChartControl chart)
        {
            var markers = new List<DealMarker>();
            foreach (var order in orders)
            {
                var isSo = !string.IsNullOrEmpty(order.Comment) &&
                    (order.Comment.Contains("stopout") || order.Comment.Contains("[so]"));
                var candleIndex = chart.chart.StockSeries.GetDoubleIndexByTime(order.TimeEnter);
                
                var title = string.Format("#{0} {1} {2} @ {3}",
                    order.ID, order.Side > 0 ? "BUY" : "SELL", order.Symbol,
                    order.PriceExit.Value.ToStringUniformPriceFormat(true));

                var resultComment = !order.IsClosed
                    ? ""
                    : string.Format(" - {0}, {1} USD",
                        order.PriceExit.Value.ToStringUniformPriceFormat(true),
                        order.ResultDepo.ToStringUniformMoneyFormat());
                var comment = string.Format("#{0} {1} {2} {3} @{4}{5}",
                    order.ID, order.Side > 0 ? "BUY" : "SELL", order.Volume, order.Symbol,
                    order.PriceEnter.ToStringUniformPriceFormat(true), resultComment);

                var enterSign = new DealMarker(chart.chart, markers,
                    DealMarker.DealMarkerType.Вход, (DealType) order.Side, candleIndex, order.PriceEnter,
                    order.TimeEnter)
                {
                    Comment = comment,
                    ColorText = isSo ? Color.Red : Color.Black,
                    Name = title
                };
                markers.Add(enterSign);
                chart.seriesMarker.data.Add(enterSign);

                if (order.TimeExit.HasValue)
                {
                    var candleExitIndex = chart.chart.StockSeries.GetDoubleIndexByTime(order.TimeExit.Value);
                    var exitSign = new DealMarker(chart.chart, markers,
                        DealMarker.DealMarkerType.Выход, (DealType) order.Side, candleExitIndex, order.PriceExit.Value,
                        order.TimeExit.Value)
                    {
                        Comment = comment,
                        ColorText = isSo ? Color.Red : Color.Black,
                        Name = title
                    };
                    enterSign.exitPair = exitSign.id;
                    markers.Add(exitSign);
                    chart.seriesMarker.data.Add(exitSign);
                }
            }
        }
    }
}
