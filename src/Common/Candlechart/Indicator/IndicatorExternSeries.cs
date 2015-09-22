using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using Candlechart.Chart;
using Candlechart.Core;
using Candlechart.Series;
using Candlechart.Theme;
using Entity;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    [DisplayName("Данные из файла")]
    [LocalizedCategory("TitleOscillator")]
    [TypeConverter(typeof(PropertySorter))]
    // ReSharper disable InconsistentNaming
    public class IndicatorExternSeries : BaseChartIndicator, IChartIndicator
    {
        #region Основные настройки

        [Browsable(false)]
        public override string Name { get { return "Данные из файла"; } }

        [LocalizedDisplayName("TitleFile")]
        [LocalizedCategory("TitleMain")]
        [Description("Путь к файлу данных")]
        [Editor("Candlechart.Indicator.FileBrowseUITypeEditor, System.Drawing.Design.UITypeEditor",
            typeof(UITypeEditor))]
        public string Path { get; set; }

        #endregion

        #region Визуальные настройки

        [LocalizedDisplayName("TitleShowType")]
        [LocalizedCategory("TitleVisuals")]
        [Description("Режим рисования - бары, свечи или ломаная линия")]
        public StockSeries.CandleDrawMode DrawMode { get; set; }

        [LocalizedDisplayName("TitleLineColor")]
        [LocalizedCategory("TitleVisuals")]
        [Description("Цвет линии")]
        public Color OutlineLineColor { get; set; }

        [LocalizedDisplayName("TitleGrowthBricksColorShort")]
        [LocalizedCategory("TitleVisuals")]
        [Description("Растущие бары")]
        public Color UpFillColor { get; set; }

        [LocalizedDisplayName("TitleFallBricksColorShort")]
        [LocalizedCategory("TitleVisuals")]
        [Description("Растущие бары")]        
        public Color DownFillColor { get; set; }

        private int upperBound = 75;

        [LocalizedDisplayName("TitleUpperLimit")]
        [LocalizedCategory("TitleVisuals")]
        [Description("Верхняя отметка на графике Каймана")]
        public int UpperBound
        {
            get { return upperBound; }
            set { upperBound = value; }
        }

        private int lowerBound = 25;

        [LocalizedDisplayName("TitleLowerLimit")]
        [LocalizedCategory("TitleVisuals")]
        [Description("Нижняя отметка на графике Каймана")]
        public int LowerBound
        {
            get { return lowerBound; }
            set { lowerBound = value; }
        }

        #endregion

        private CandlestickSeries candleSeries;

        private PartSeries seriesBound;

        #region Конструкторы - копирование

        public IndicatorExternSeries()
        {
            OutlineLineColor = Color.Black;
            UpFillColor = Color.White;
            DownFillColor = Color.Black;
        }

        public override BaseChartIndicator Copy()
        {
            var rsi = new IndicatorExternSeries();
            Copy(rsi);
            return rsi;
        }

        public override void Copy(BaseChartIndicator indi)
        {
            var extIndi = (IndicatorExternSeries)indi;
            CopyBaseSettings(extIndi);
            extIndi.Path = Path;
            extIndi.UpperBound = UpperBound;
            extIndi.LowerBound = LowerBound;
            extIndi.candleSeries = candleSeries;
            extIndi.OutlineLineColor = OutlineLineColor;
            extIndi.DownFillColor = DownFillColor;
            extIndi.UpFillColor = UpFillColor;
            extIndi.DrawMode = DrawMode;
        }
        #endregion

        public void BuildSeries(ChartControl chart)
        {
            candleSeries.Data.Clear();
            LoadData();
            DrawBounds();
        }

        public void Add(ChartControl chart, Pane ownerPane)
        {
            owner = chart;
            // инициализируем индикатор
            candleSeries = new CandlestickSeries(Name)
            {
                DownFillColor = DownFillColor,
                UpFillColor = UpFillColor,
                UpLineColor = OutlineLineColor,
                DownLineColor = OutlineLineColor,
                BarDrawMode = DrawMode
            };
            seriesBound = new PartSeries("Границы Каймана")
            {
                LineColor = OutlineLineColor,
                ForeColor = OutlineLineColor,
                LineStyle = LineStyle.Dot,                
                LineWidth = 1
            };
            SeriesResult = new List<Series.Series> { candleSeries, seriesBound };
            EntitleIndicator();
        }

        public void Remove()
        {
        }

        public void AcceptSettings()
        {
            candleSeries.UpLineColor = OutlineLineColor;
            candleSeries.DownLineColor = OutlineLineColor;
            candleSeries.UpFillColor = UpFillColor;
            candleSeries.DownFillColor = DownFillColor;
            candleSeries.BarDrawMode = DrawMode;
            seriesBound.LineColor = OutlineLineColor;
        }

        public void OnCandleUpdated(CandleData updatedCandle, List<CandleData> newCandles)
        {
            BuildSeries(owner);
        }

        public string GetHint(int x, int y, double index, double price, int tolerance)
        {
            return string.Format("{0} = {1:f5}", UniqueName, candleSeries.Data[(int)index]);
        }

        public List<IChartInteractiveObject> GetObjectsUnderCursor(int screenX, int screenY, int tolerance)
        {
            return new List<IChartInteractiveObject>();
        }

        public override string GenerateNameBySettings()
        {
            string colorStr;
            GetKnownColor(OutlineLineColor.ToArgb(), out colorStr);
            return string.Format("Данные файла[{0}]{1}",
                                 Path, string.IsNullOrEmpty(colorStr) ? "" : string.Format("({0})", colorStr));
        }

        private void LoadData()
        {
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path)) return;
            var timeframe = DeriveTimeframeMinutes();

            var candles = new Dictionary<DateTime, CandleData>();
            using (var sr = new StreamReader(Path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    var parts = line.Split(new[] {' ', (char) 9}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 7) continue;
                    var dateStr = parts[0] + " " + parts[1];

                    //KS_EURUSD Bid
                    //TIME	OPEN	HIGH	LOW	CLOSE	VOLUME	
                    //29/07/15 11:00:00	41,18	41,18	40,37	40,52	12,00
                    DateTime date;
                    if (!DateTime.TryParseExact(dateStr, "dd/MM/yy HH:mm:ss", CultureProvider.Common, DateTimeStyles.None, out date))
                        continue;
                    var open = parts[2].Replace(',', '.').ToFloatUniformSafe();
                    var high = parts[3].Replace(',', '.').ToFloatUniformSafe();
                    var low = parts[4].Replace(',', '.').ToFloatUniformSafe();
                    var close = parts[5].Replace(',', '.').ToFloatUniformSafe();

                    if (!open.HasValue || !close.HasValue || !high.HasValue || !low.HasValue)
                        continue;
                    candles.Add(date, new CandleData(open.Value, high.Value, low.Value, close.Value,
                        date, date.AddMinutes(timeframe)));                    
                }
            }

            AdjustAndAddCandles(candles, timeframe);
        }

        private void AdjustAndAddCandles(Dictionary<DateTime, CandleData> candles, int timeframe)
        {
            var chartCandles = owner.StockSeries.Data.Candles;
            // добавить в прочитанный список пустые свечи
            foreach (var candle in chartCandles)
            {
                if (candles.ContainsKey(candle.timeOpen)) continue;
                candles.Add(candle.timeOpen, null);
            }
            // удалить из прочитанного списка свечи, коих нет на графике
            var chartCandleTimes = chartCandles.ToDictionary(c => c.timeOpen, c => c.timeOpen);
            foreach (var key in candles.Keys.ToList())
                if (!chartCandleTimes.ContainsKey(key))
                    candles.Remove(key);

            // дать пыстым свечам тела
            var pairList = candles.OrderBy(c => c.Key).ToList();
            var candleList = new List<CandleData>();
            for (var i = 1; i < candles.Count; i++)
            {
                CandleData candle;
                if (pairList[i].Value == null)
                {
                    if (candleList.Count == 0)
                    {
                        candle = new CandleData(50, 50, 50, 50, pairList[i - 1].Key,
                            pairList[i - 1].Key.AddMinutes(timeframe));
                        candleList.Add(candle);
                    }
                    candle = new CandleData(
                        candleList[i - 1].close,
                        candleList[i - 1].close,
                        candleList[i - 1].close,
                        candleList[i - 1].close,
                        candleList[i - 1].timeOpen,
                        candleList[i - 1].timeClose);
                }
                else
                    candle = pairList[i].Value;
                candleList.Add(candle);
            }
            candleSeries.Data.Candles.AddRange(candleList);
        }

        private int DeriveTimeframeMinutes()
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(Path).ToLower();
            if (fileName.EndsWith("m1")) return 1;
            if (fileName.EndsWith("m5")) return 5;
            if (fileName.EndsWith("m15")) return 15;
            if (fileName.EndsWith("m30")) return 30;
            if (fileName.EndsWith("h1")) return 60;
            if (fileName.EndsWith("h4")) return 240;
            if (fileName.EndsWith("d1")) return 1440;
            return owner.Owner.Timeframe.Intervals[0];
        }

        private void DrawBounds()
        {
            seriesBound.parts.Clear();
            // границы
            var levels = new [] {50, upperBound, lowerBound};
            foreach (var level in levels)
            {
                seriesBound.parts.Add(new List<PartSeriesPoint>
                {
                    new PartSeriesPoint(1, level),
                    new PartSeriesPoint(owner.StockSeries.DataCount, level)
                });
            }
        }
    }
    // ReSharper restore InconsistentNaming
}
