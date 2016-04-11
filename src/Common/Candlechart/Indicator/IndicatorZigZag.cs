using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Candlechart.Chart;
using Candlechart.ChartMath;
using Candlechart.Core;
using Candlechart.Series;
using Entity;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    [LocalizedDisplayName("TitleZigzag")]
    [LocalizedCategory("TitleTrending")]
    [TypeConverter(typeof(PropertySorter))]
    public class IndicatorZigZag : BaseChartIndicator, IChartIndicator
    {
        [Browsable(false)]
        public override string Name => Localizer.GetString("TitleZigzag");

        public override BaseChartIndicator Copy()
        {
            var zig = new IndicatorZigZag();
            Copy(zig);
            return zig;
        }

        [LocalizedDisplayName("TitleCreateDrawingPanel")]
        [LocalizedCategory("TitleMain")]
        [Description("Создавать свою панель отрисовки")]
        public override bool CreateOwnPanel { get; set; }

        #region Визуальные

        [LocalizedDisplayName("TitleTrendColor")]
        [Description("Цвет линии индикатора")]
        [LocalizedCategory("TitleVisuals")]
        public Color TrendColor { get; set; } = Color.Red;

        [LocalizedDisplayName("TitleCorrectionColor")]
        [Description("Цвет линии индикатора")]
        [LocalizedCategory("TitleVisuals")]
        public Color CorrectionColor { get; set; } = Color.Blue;

        [LocalizedDisplayName("TitleThickness")]
        [Description("Толщина линии индикатора, пикселей")]
        [LocalizedCategory("TitleVisuals")]
        public decimal LineWidth { get; set; } = 1;

        [LocalizedDisplayName("TitleLineStyle")]
        [Description("Стиль линии индикатора (сплошная, штирх, штрих-пунктир...)")]
        [LocalizedCategory("TitleVisuals")]
        public DashStyle LineStyle { get; set; } = DashStyle.Solid;

        #endregion

        [LocalizedDisplayName("TitleThresholdInPercents")]
        [Description("Пороговая величина, %, на которую должен отклониться курс для формирования нового экстремума")]
        [LocalizedCategory("TitleMain")]
        public float ThresholdPercent { get; set; } = 1;

        [LocalizedDisplayName("TitleCorrectionPercent")]
        [Description("Пороговая величина, %, на которую должен откатиться курс для формирования коррекции")]
        [LocalizedCategory("TitleMain")]
        public float CorrectionPercent { get; set; } = 0.5f;

        [LocalizedDisplayName("TitleZigzagPrice")]
        [Description("Цены Зиг-Зага")]
        [LocalizedCategory("TitleMain")]
        public ZigZagSource ZigZagSourceType { get; set; }
        
        private PartSeries seriesZigZag;

        public IndicatorZigZag()
        {
            CreateOwnPanel = false;
        }

        public override void Copy(BaseChartIndicator indi)
        {
            var zig = (IndicatorZigZag)indi;
            CopyBaseSettings(zig);
            zig.TrendColor = TrendColor;
            zig.CorrectionColor = CorrectionColor;
            zig.LineWidth = LineWidth;
            zig.LineStyle = LineStyle;
            zig.ThresholdPercent = ThresholdPercent;
            zig.CorrectionPercent = CorrectionPercent;
            zig.seriesZigZag = seriesZigZag;
            zig.ZigZagSourceType = ZigZagSourceType;
        }

        public void BuildSeries(ChartControl chart)
        {
            if (DrawPane != owner.StockPane)
                DrawPane.Title = UniqueName;
            seriesZigZag.parts.Clear();
            if (chart.StockSeries.Data.Count < 3) return;
            
            seriesZigZag.LineColor = TrendColor;
            seriesZigZag.LineWidth = (float) LineWidth;
            //seriesZigZag.LineDashStyle = lineStyle;

            // индекс - индикатор
            var correctionPercent = CorrectionPercent < ThresholdPercent ? CorrectionPercent : ThresholdPercent;
            var pivots = ZigZag.GetPivots(chart.StockSeries.Data.Candles, ThresholdPercent, correctionPercent,
                chart.StockSeries.Data.Candles.Count - 1, ZigZagSourceType);
            if (pivots.Count == 0) return;

            // построить отрезки
            for (var i = 1; i < pivots.Count; i++)
            {
                var color = pivots[i - 1].nextArc == ZigZagPivot.ZigZagArc.Correction ? CorrectionColor : TrendColor;
                seriesZigZag.parts.Add(new PartSeries.Polyline(
                    color,
                    new PartSeriesPoint(pivots[i - 1].index, (decimal) pivots[i - 1].price),
                    new PartSeriesPoint(pivots[i].index, (decimal) pivots[i].price)));
            }
        }

        public void Add(ChartControl chart, Pane ownerPane)
        {
            owner = chart;
            // инициализируем индикатор
            seriesZigZag = new PartSeries(Localizer.GetString("TitleZigzag"))
            {
                LineColor = TrendColor,
                LineWidth = ((float)LineWidth)
            };
            SeriesResult = new List<Series.Series> { seriesZigZag };
            EntitleIndicator();
        }

        public void Remove()
        {
            seriesZigZag.parts.Clear();
        }

        public void AcceptSettings()
        {
        }        

        /// <summary>
        /// пересчитать индикатор для последней добавленной свечки
        /// </summary>        
        public void OnCandleUpdated(CandleData updatedCandle, List<CandleData> newCandles)
        {
            BuildSeries(owner);
        }

        public string GetHint(int x, int y, double index, double price, int tolerance)
        {
            return string.Empty;
        }

        public List<IChartInteractiveObject> GetObjectsUnderCursor(int screenX, int screenY, int tolerance)
        {
            return new List<IChartInteractiveObject>();
        }
    }
}
