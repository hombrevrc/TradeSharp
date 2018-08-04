using System;
using System.Drawing;
using Entity;

namespace Candlechart.Series
{
    public class ChartCommentPicture : ChartComment, IDisposable
    {
        public BarSettings Timeframe { get; set; }
        public string Symbol { get; set; }
        public DateTime Time { get; set; }

        private Bitmap picture;
        private GraphPainter graphPainter = new GraphPainter();

        public void Draw(Graphics grf, float left, float top, int width, int height)
        {
            if (Timeframe == null || Symbol == null || Time == null)
                return;

            if (picture == null)
                picture = graphPainter.GetGraphSchematic(Timeframe, Symbol, Time);

            grf.FillRectangle(Brushes.WhiteSmoke, left, top, width, height); //фон
            using (var p = new Pen(Color.Black, 1))
                grf.DrawRectangle(p, left, top, width, height); //рамка

            grf.DrawImage(picture, left, top, width, height);
        }

        public void Dispose()
        {
            if (picture != null)
                picture.Dispose();
        }
    }
}