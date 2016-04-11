using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Candlechart.ChartMath;
using Candlechart.Core;
using Entity;

namespace Candlechart.Series
{
    /// <summary>
    /// Серия ломаных
    /// </summary>
    public class PartSeries : Series
    {
        public class Polyline
        {
            public List<PartSeriesPoint> parts;

            public Color? color;

            public Polyline()
            {
                parts = new List<PartSeriesPoint>();
            }

            public Polyline(params PartSeriesPoint[] lines)
            {
                parts = lines.ToList();
            }

            public Polyline(Color cl, params PartSeriesPoint[] lines) : this (lines)
            {
                color = cl;
            }
        }
          
        /// <summary>
        /// отрезки (индекс данных - цена)
        /// </summary>
        public List<Polyline> parts = new List<Polyline>();

        public override int DataCount { get { return parts.Count; } }

        private Color lineColor = Color.Black;
        /// <summary>
        /// цвет линии
        /// </summary>
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }

        /// <summary>
        /// радиус окружности-маркера (0 - не рисуется)
        /// </summary>
        public float MarkerRadius { get; set; }

        public PartSeries(string name)
            : base(name)
        {
        }
        public override bool GetXExtent(ref double left, ref double right)
        {
            return false;
        }
        public override bool GetYExtent(double left, double right, ref double top, ref double bottom)
        {
            return false;
        }
        public override void Draw(Graphics g, RectangleD worldRect, Rectangle canvasRect)
        {
            base.Draw(g, worldRect, canvasRect);
            DrawParts(g, worldRect, canvasRect);
        }

        private void DrawParts(Graphics g, RectangleD worldRect, Rectangle canvasRect)
        {
            using (var penStor = new PenStorage())
            {
                var markerPen = penStor.GetPen(lineColor);
                using (var markerBrush = new SolidBrush(Color.White))
                {
                    foreach (var chain in parts)
                        DrawRegion(chain, g, worldRect, canvasRect, penStor, markerPen, markerBrush);
                }
            }
        }

        private void DrawRegion(Polyline chain, Graphics g, 
            RectangleD worldRect, Rectangle canvasRect,
            PenStorage penStor, Pen markerPen, Brush markerBrush)
        {
            var linePoints = new List<PointF>();
            for (var i = 0; i < chain.parts.Count; i++)
            {
                var p = Conversion.WorldToScreen(
                    new PointD(chain.parts[i].index - 0.5, (double) chain.parts[i].quote), worldRect, canvasRect);
                linePoints.Add(new PointF((float)p.X, (float)p.Y));
            }

            var pen = penStor.GetPen(chain.color ?? lineColor);

            g.DrawLines(pen, linePoints.ToArray());
            if (MarkerRadius > 0)
            {
                foreach (var point in linePoints)
                {
                    var ellipseRect = new RectangleF(
                        point.X - MarkerRadius, point.Y - MarkerRadius,
                        MarkerRadius*2, MarkerRadius*2);
                    g.FillEllipse(markerBrush, ellipseRect);
                    g.DrawEllipse(markerPen, ellipseRect);
                }
            }
        }
    }

    public struct PartSeriesPoint
    {
        public int index;
        public decimal quote;
        public PartSeriesPoint(int index, decimal quote)
        {
            this.index = index;
            this.quote = quote;
        }
    }
}