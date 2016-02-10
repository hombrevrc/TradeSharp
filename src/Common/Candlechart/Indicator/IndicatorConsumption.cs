using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using Candlechart.Chart;
using Candlechart.ChartMath;
using Candlechart.Core;
using Candlechart.Series;
using Entity;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    [LocalizedDisplayName("TitleIndicatorConsumption")]
    [LocalizedCategory("TitleTrending")]
    [TypeConverter(typeof(PropertySorter))]
    public class IndicatorConsumption : BaseChartIndicator, IChartIndicator
    {
        #region Настройки

        [Browsable(false)]
        public override string Name => Localizer.GetString("TitleIndicatorConsumption");

        [LocalizedDisplayName("TitleIndicatorConsumptionStyle")]
        [LocalizedCategory("TitleMain")]
        [LocalizedDescription("TitleIndicatorConsumptionStyle")]
        public CandleConsumptionFinder.CandleConsumptionStyle ConsumptionStyle { get; set; } = 
            CandleConsumptionFinder.CandleConsumptionStyle.СтрогаяПроверка;

        [LocalizedDisplayName("TitleIndicatorConsumptionMinCount")]
        [LocalizedCategory("TitleMain")]
        [LocalizedDescription("TitleIndicatorConsumptionMinCount")]
        public int MinCount { get; set; } = 2;

        [LocalizedDisplayName("TitleCreateDrawingPanel")]
        [LocalizedCategory("TitleMain")]
        [LocalizedDescription("MessageCreateDrawingPanelDescription")]
        public override bool CreateOwnPanel { get; set; } = false;

        [LocalizedDisplayName("TitleColor")]
        [LocalizedDescription("MessageLineColorDescription")]
        [LocalizedCategory("TitleVisuals")]
        public Color ClLine { get; set; } = Color.Green;

        [LocalizedDisplayName("TitleBandStyle")]
        [LocalizedDescription("MessageLineStyleDescription")]
        [LocalizedCategory("TitleVisuals")]
        public DashStyle LineStyle { get; set; } = DashStyle.Dot;

        #endregion

        private RegionSeries seriesOutline;

        public override BaseChartIndicator Copy()
        {
            var indi = new IndicatorConsumption();
            Copy(indi);
            return indi;
        }

        public override void Copy(BaseChartIndicator indi)
        {
            var indiBol = (IndicatorConsumption) indi;
            CopyBaseSettings(indiBol);
            indiBol.ConsumptionStyle = ConsumptionStyle;
            indiBol.MinCount = MinCount;
        }

        public void BuildSeries(ChartControl chart)
        {
            seriesOutline.data.Clear();

            var candles = chart.StockSeries.Data.Candles;
            if (SeriesSources.Any(s => s is CandlestickSeries))
                candles = ((CandlestickSeries) SeriesSources.First(s => s is CandlestickSeries)).Data.Candles;

            var finder = new CandleConsumptionFinder(candles, MinCount);
            var consumptions = finder.Search();

            foreach (var cons in consumptions)
            {
                seriesOutline.data.Add(new BarRegion
                {
                    Color = ClLine,
                    lineStyle = LineStyle == DashStyle.Solid ? BarRegionLineStyle.SolidThin : BarRegionLineStyle.DotThick,
                    IndexStart = cons.startIndex,
                    IndexEnd = cons.consumerIndex,
                    LowerBound = (float) cons.lower,
                    UpperBound = (float) cons.upper
                });
            }
        }

        public void Add(ChartControl chart, Pane ownerPane)
        {
            owner = chart;
            seriesOutline = new RegionSeries(Localizer.GetString("TitleIndicatorConsumption"))
            {
                DrawAsFrame = true
            };
            // инициализируем индикатор
            EntitleIndicator();
            SeriesResult = new List<Series.Series> { seriesOutline };
        }

        public void Remove()
        {
            seriesOutline?.data.Clear();
        }

        public void AcceptSettings()
        {
        }

        public void OnCandleUpdated(CandleData updatedCandle, List<CandleData> newCandles)
        {
            if (newCandles == null) return;
            if (newCandles.Count == 0) return;
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
