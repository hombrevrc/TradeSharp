using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Entity
{
    public class GraphPainter
    {
        //Количество пикселей на одно деление шкалы сетки
        const int GridStepPxl = 40;
        const int GrigStepCount = 10;
        const int CandlesForScreenshot = 15;
        const int PriceMarginPxl = 80;
        private readonly Font LabelFont = new Font(FontFamily.GenericSansSerif, 12f);

        public GraphPainter() { }

        /// <param name="isBuy">Side > 0 ? "BUY" : "SELL"</param>
        public void SaveGraphToFile(string folderName, BarSettings timeframe, string symbol, bool isBuy)
        {
            var tempFileName = Path.Combine(folderName, $"{symbol}.png");
            var candles = GetCandles(timeframe, symbol);
            if (candles.Count == 0)
                return;

            using (var btm = GetGraph(candles, isBuy))
                btm.Save(tempFileName, ImageFormat.Png);
        }

        /// <param name="isBuy">Side > 0 ? "BUY" : "SELL"</param>
        public Bitmap GetGraphImage(BarSettings timeframe, string symbol, bool isBuy)
        {
            var candles = GetCandles(timeframe, symbol);
            if (candles.Count == 0)
                return null;

            return GetGraph(candles, isBuy);
        }

        private Bitmap GetGraph(List<CandleData> candles, bool isBuy)
        {
            var startDate = candles.First().timeOpen;
            var entTime = candles.Last().close;
            var maxPip = candles.Max(x => x.high);
            var minPip = candles.Min(x => x.low);
            var gridStepPip = (maxPip - minPip) / (GrigStepCount - 2);
            var pxlOnPip = GridStepPxl / gridStepPip;

            int width = (CandlesForScreenshot + 3) * GridStepPxl + PriceMarginPxl;
            int height = GrigStepCount * GridStepPxl + 1;

            Bitmap btm = new Bitmap(width, height);

            using (Graphics grf = Graphics.FromImage(btm))
            {
                DrawGrig(grf, new double[] { minPip, minPip + 4 * gridStepPip, maxPip }, width, height);

                var currentPrice = candles.Last().close;
                var currentPricePxlY = GridStepPxl + (maxPip - currentPrice) * pxlOnPip - 2;
                var currentPricePxlX = candles.Count * GridStepPxl;
                grf.DrawLine(Pens.Gray, currentPricePxlX, currentPricePxlY, width - PriceMarginPxl, currentPricePxlY);
                var currentPriceText = currentPrice.ToString("F4");
                grf.DrawString(currentPriceText, LabelFont, Brushes.Gray, width - PriceMarginPxl + 2, currentPricePxlY - 10);

                using (Pen candelBorderPen = new Pen(Color.Black, 1))
                {
                    int x = 0;
                    for (int candleNumder = 0; candleNumder < candles.Count; candleNumder++)
                    {
                        var candle = candles[candleNumder];
                        x = (candleNumder + 1) * GridStepPxl;

                        DrawCandel(
                            grf,
                            candle.high, candle.low, candle.close, candle.open,
                            x,
                            candelBorderPen,
                            maxPip,
                            pxlOnPip);

                        //Прорисовываем подписи к шкале Х (время)
                        if (candleNumder % 4 == 0)
                        {
                            var dateTimeText = candle.timeOpen.ToString("dd.MM HH:mm");
                            grf.DrawString(dateTimeText, LabelFont, Brushes.Gray, x, height - 20);
                        }
                    }
                    DrawArrow(grf, isBuy, candles.Last().high, candles.Last().low, x, maxPip, pxlOnPip);
                }
            }

            return btm;
        }

        private void DrawGrig(Graphics grf, Double[] pipLables, int width, int height)
        {
            //Нарисовать сетку
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                var rightScaleX = width - PriceMarginPxl;

                for (int i = 0; i < GrigStepCount; i++)
                {
                    var y = i * GridStepPxl;

                    grf.DrawLine(gridPen, 0, y, rightScaleX, y);

                    if (i == 1)
                    {
                        var pipText = pipLables[2].ToString("F4");
                        grf.DrawString(pipText, LabelFont, Brushes.Gray, rightScaleX + 2, y - 10);
                    }
                    else if (i == 9)
                    {
                        var pipText = pipLables[0].ToString("F4");
                        grf.DrawString(pipText, LabelFont, Brushes.Gray, rightScaleX + 2, y - 10);
                    }
                }


                grf.DrawLine(gridPen, rightScaleX, 0, rightScaleX, height);
            }

            //Нарисовать рамку
            using (Pen gridPen = new Pen(Color.Black, 1))
            {
                grf.DrawLine(gridPen, 0, 0, width - 1, 0);
                grf.DrawLine(gridPen, width - 1, 0, width - 1, height - 1);
                grf.DrawLine(gridPen, width - 1, height - 1, 0, height - 1);
                grf.DrawLine(gridPen, 0, 0, 0, height);
            }
        }

        private void DrawCandel(
            Graphics grf,
            float candleHigh,
            float candleLow,
            float candleClose,
            float candleOpen,
            int x,
            Pen border,
            float maxPip,
            float pxlOnPip)
        {
            var y1 = GridStepPxl + (maxPip - candleHigh) * pxlOnPip;
            var y2 = GridStepPxl + (maxPip - candleLow) * pxlOnPip;

            grf.DrawLine(border, x, y1, x, y2);

            var isRising = candleClose > candleOpen;
            var y3 = GridStepPxl + (maxPip - (isRising ? candleClose : candleOpen)) * pxlOnPip;
            var candelRectangel = new Rectangle(x - 5, (int)y3, 10, (int)(Math.Abs(candleOpen - candleClose) * pxlOnPip));

            grf.FillRectangle(isRising ? Brushes.YellowGreen : Brushes.IndianRed, candelRectangel);
            grf.DrawRectangle(border, candelRectangel);
        }

        private void DrawArrow(
            Graphics grf,
            bool isBuy,
            float candleHigh,
            float candleLow,
            float x,
            float maxPip,
            float pxlOnPip)
        {
            var yCentr = GridStepPxl + (maxPip - (candleHigh + candleLow) / 2) * pxlOnPip;
            Pen pen = null;

            if (isBuy)
            {
                pen = new Pen(Color.MediumBlue, 8);
                pen.StartCap = LineCap.ArrowAnchor;
            }
            else
            {
                pen = new Pen(Color.IndianRed, 8);
                pen.EndCap = LineCap.ArrowAnchor;
            }

            grf.DrawLine(pen, x + 15, yCentr - 10, x + 15, yCentr + 10);
            grf.DrawString(isBuy ? "Buy" : "Sell", new Font(FontFamily.GenericSansSerif, 10f), new SolidBrush(pen.Color), x + 5, yCentr + 15);
        }

        private List<CandleData> GetCandles(BarSettings timeframe, string symbol)
        {
            DateTime nowTime = DateTime.Now;
            var startTime = timeframe.GetDistanceTime(CandlesForScreenshot, -1, nowTime);
            var minuteCandles = AtomCandleStorage.Instance.GetAllMinuteCandles(symbol, startTime, nowTime) ?? new List<CandleData>();

            var packer = new CandlePacker(timeframe);
            var candles = new List<CandleData>();
            foreach (var minuteCandle in minuteCandles)
            {
                var candle = packer.UpdateCandle(minuteCandle);
                if (candle != null)
                    candles.Add(candle);
            }

            var tail = minuteCandles.Where(x => x.timeOpen > candles.Last().timeClose).ToArray();
            if (tail.Length > 0)
            {
                float open = tail.First().open;
                float close = tail.Last().close;
                float high = tail.Max(x => x.high);
                float low = tail.Min(x => x.low);

                DateTime timeOpen = tail.First().timeOpen;
                DateTime timeClose = tail.Last().timeClose;

                var currentCandel = new CandleData(open, high, low, close, timeOpen, timeClose);
                candles.Add(currentCandel);
            }

            return candles;
        }
    }
}