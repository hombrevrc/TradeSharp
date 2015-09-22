using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization.Advanced;
using Candlechart.Chart;
using Candlechart.ChartMath;
using Candlechart.Core;
using Entity;
using TradeSharp.Util;

namespace Candlechart.Series
{
    public class DealMarker : IChartInteractiveObject
    {
        [Browsable(false)]
        public InteractiveObjectSeries Owner { get; set; }

        [Browsable(false)]
        public string ClassName { get { return Localizer.GetString("TitleDealMarker"); } }

        private readonly List<DealMarker> markers;

        public static int nextNumber = 1;

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public int id;

        /// <summary>
        /// парный маркер, "закрывающий" данный маркер входа
        /// </summary>
        public int? exitPair;

        public enum DealMarkerType { Вход = 0, Выход }

        private readonly ChartControl owner;

        public DealMarkerType MarkerType { get; set; }

        private DealType side = DealType.Buy;
        [LocalizedDisplayName("TitleDirection")]
        [LocalizedCategory("TitleMain")]
        [PropertyOrder(1, 1)]
        public DealType Side
        {
            get { return side; }
            set { side = value; }
        }

        public double candleIndex;

        [LocalizedDisplayName("TitlePrice")]
        [LocalizedCategory("TitleMain")]
        public double Price { get; set; }

        [LocalizedDisplayName("TitleComment")]
        [LocalizedCategory("TitleMain")]
        public string Comment { get; set; }

        [DisplayName("Magic")]
        [LocalizedCategory("TitleMain")]
        public int Magic { get; set; }

        private Color colorText = Color.Black;
        [LocalizedDisplayName("TitleColor")]
        [LocalizedCategory("TitleVisuals")]
        public Color ColorText
        {
            get { return colorText; }
            set { colorText = value; }
        }

        private static readonly PointF[] shapePoints = 
                                                  {
                                                      new PointF(0, 0), new PointF(7, 5),
                                                      new PointF(17, 5), new PointF(17, -5),
                                                      new PointF(7, -5)
                                                  };

        private static PointF textCenter = new PointF(12, 0);

        private RectangleF markerScreenRegion;

        private readonly ChartObjectMarker[] editMarkers =
            { new ChartObjectMarker { action = ChartObjectMarker.MarkerAction.Move } };

        public DealMarker(ChartControl owner, List<DealMarker> markers)
        {
            this.markers = markers;
            this.owner = owner;
        }

        public DealMarker(ChartControl owner, List<DealMarker> markers,
            DealMarkerType markerType, DealType side,
            double candleIndex, double price, DateTime time)
        {
            this.markers = markers;
            var maxId = markers.Count == 0 ? 0 : markers.Max(m => m.id);
            if (nextNumber <= maxId) nextNumber = maxId + 1;
            id = nextNumber++;

            Name = string.Format("{0} {1}", ClassName, id);
            this.owner = owner;
            MarkerType = markerType;
            this.candleIndex = candleIndex;
            Price = price;
            DateStart = time;
            this.side = side;
        }

        public void Draw(Graphics g, PointD ptPivot, Font font)
        {
            using (var pen = new Pen(colorText, Selected ? 2f : 1f))
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, Color.White)))
                {
                    var points = GetMarkerShapePoints(ptPivot);
                    // получить обрамляющий прямоугольник
                    var topX = points.Min(p => p.X);
                    var topY = points.Min(p => p.Y);
                    var rightX = points.Max(p => p.X);
                    var bottmY = points.Max(p => p.Y);

                    markerScreenRegion = new RectangleF(topX + owner.StockPane.CanvasRect.Left,
                        topY, rightX - topX, bottmY - topY);
                    // значок
                    g.FillPolygon(brush, points.ToArray());
                    g.DrawPolygon(pen, points.ToArray());
                    // текст
                    var ptText = MarkerType == DealMarkerType.Вход
                        ? new PointF((float)ptPivot.X - textCenter.X,
                                            (float)ptPivot.Y - textCenter.Y)
                        : new PointF((float)ptPivot.X + textCenter.X,
                                            (float)ptPivot.Y + textCenter.Y);

                    var str = side == DealType.Buy ? "b" : "s";
                    using (var brushText = new SolidBrush(colorText))
                    using (var brushBack = new SolidBrush(owner.visualSettings.SeriesBackColor))
                    {
                        g.DrawString(str, font, brushText, ptText.X, ptText.Y,
                                     new StringFormat
                                     {
                                         Alignment = StringAlignment.Center,
                                         LineAlignment = StringAlignment.Center
                                     });
                        // нарисовать результат по "сделке" в пунктах справа
                        if (MarkerType == DealMarkerType.Выход)
                        {
                            var markerEnter = markers.FirstOrDefault(m => m.exitPair == id);
                            if (markerEnter != null)
                            {
                                var delta = (int)markerEnter.Side * (Price - markerEnter.Price);
                                var deltaPoints =
                                    (int)
                                    DalSpot.Instance.GetPointsValue(owner.Symbol, (decimal)delta);
                                str = deltaPoints.ToString();
                                g.DrawString(str, font, brushText, rightX + 1, bottmY + 1);
                            }
                        }

                        // комментарий - для выделенного маркера входа
                        if (Selected && !string.IsNullOrEmpty(Comment) && MarkerType == DealMarkerType.Вход)
                        {
                            var size = g.MeasureString(Comment, font);
                            var top = new Point((int) ptPivot.X - 14, (int) ptPivot.Y - 4);
                            var comArea = new Rectangle(top.X - (int) size.Width, top.Y - (int) size.Height,
                                (int) size.Width, (int) size.Height);
                            comArea.Inflate(2, 2);
                            g.FillRectangle(brushBack, comArea);
                            g.DrawRectangle(pen, comArea);
                            
                            g.DrawString(Comment, font, brushText, top.X, top.Y,
                                     new StringFormat
                                     {
                                         Alignment = StringAlignment.Far,
                                         LineAlignment = StringAlignment.Far
                                     });
                        }
                    }
                }
            }
        }

        public bool PointIsIn(float x, float y)
        {
            return markerScreenRegion.Contains(x, y);
        }

        public string GetToolTip()
        {
            DealMarker enter = MarkerType == DealMarkerType.Вход
                    ? this
                    : markers.Find(m => m.exitPair.HasValue ? m.exitPair.Value == id : false);
            DealMarker exit =
                MarkerType == DealMarkerType.Вход
                ? exitPair.HasValue ? markers.Find(m => m.id == exitPair.Value) : null
                : this;

            string strPriceExit = "?", strTimeExit = "?", strResult = "-";
            string strPriceEnter = enter.Price.ToString("f4");
            string strTimeEnter = enter.Time.ToString("dd.MM.yy HH:mm");

            if (exit != null)
            {
                strPriceExit = exit.Price.ToString("f4");
                strTimeExit = exit.Time.ToString("dd.MM.yy HH:mm");
                var delta = (int)enter.Side * (exit.Price - enter.Price);
                var deltaPoints = DalSpot.Instance.GetPointsValue(owner.Symbol, (decimal)delta);
                strResult = deltaPoints.ToString("f1");
            }
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} - {1}", strPriceEnter, strPriceExit));
            sb.AppendLine(string.Format("{0} - {1}", strTimeEnter, strTimeExit));
            sb.AppendLine(string.Format("{0} пп", strResult));
            return sb.ToString();
        }

        private List<PointF> GetMarkerShapePoints(PointD ptPivot)
        {
            var points = new List<PointF>();
            foreach (var pt in shapePoints)
            {
                if (MarkerType == DealMarkerType.Вход)
                    points.Add(new PointF((float)ptPivot.X - pt.X, (float)ptPivot.Y + pt.Y));
                else
                    points.Add(new PointF((float)ptPivot.X + pt.X, (float)ptPivot.Y + pt.Y));
            }
            return points;
        }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (!selected && value)
                    editMarkers[0].centerModel = new PointD(candleIndex, Price);
                selected = value;
            }
        }

        [LocalizedDisplayName("TitleName")]
        [LocalizedCategory("TitleMain")]
        public string Name { get; set; }

        private DateTime dateStart;
        [LocalizedDisplayName("TitleTime")]
        [LocalizedCategory("TitleMain")]
        private DateTime Time
        {
            get { return dateStart; }
            set
            {
                dateStart = value;
                // пересчитать индекс
                candleIndex = owner.StockSeries.GetDoubleIndexByTime(dateStart);
            }
        }

        public DateTime? DateStart
        {
            get { return dateStart; }
            set
            {
                if (!value.HasValue) return;
                Time = value.Value;
            }
        }

        public int IndexStart
        {
            get { return (int)candleIndex; }
        }

        public void SaveInXML(XmlElement parentNode, CandleChartControl owner)
        {
            var node = parentNode.AppendChild(parentNode.OwnerDocument.CreateElement("Marker"));

            var attrName = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("name"));
            attrName.Value = Name;

            node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("magic")).Value = Magic.ToString();

            var attrPrice = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("price"));
            attrPrice.Value = Price.ToString(CultureProvider.Common);

            var attrComment = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("comment"));
            attrComment.Value = Comment;

            var attrType = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("type"));
            attrType.Value = MarkerType.ToString();

            var attrSide = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("side"));
            attrSide.Value = Side.ToString();

            var time = dateStart == new DateTime()
                ? owner.chart.StockSeries.GetCandleOpenTimeByIndex((int)candleIndex) : dateStart;
            var attrStart = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("time"));
            attrStart.Value = time.ToString("ddMMyyyy HHmmss", CultureProvider.Common);

            var attrId = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("id"));
            attrId.Value = id.ToString();

            if (exitPair.HasValue)
            {
                var attrExitId = node.Attributes.Append(parentNode.OwnerDocument.CreateAttribute("exitId"));
                attrExitId.Value = exitPair.ToString();
            }
        }

        public void LoadFromXML(XmlElement itemNode, CandleChartControl owner)
        {
            if (itemNode.Attributes["name"] != null)
                Name = itemNode.Attributes["name"].Value;

            if (itemNode.Attributes["magic"] != null) Magic = itemNode.Attributes["magic"].Value.ToIntSafe() ?? 0;

            if (itemNode.Attributes["price"] != null)
                Price = double.Parse(itemNode.Attributes["price"].Value, CultureProvider.Common);

            if (itemNode.Attributes["comment"] != null)
                Comment = itemNode.Attributes["comment"].Value;

            if (itemNode.Attributes["type"] != null)
                MarkerType = (DealMarkerType)Enum.Parse(typeof(DealMarkerType), itemNode.Attributes["type"].Value);

            if (itemNode.Attributes["side"] != null)
                Side = (DealType)Enum.Parse(typeof(DealType), itemNode.Attributes["side"].Value);

            if (itemNode.Attributes["time"] != null)
            {
                dateStart = DateTime.ParseExact(itemNode.Attributes["time"].Value, "ddMMyyyy HHmmss",
                                                CultureProvider.Common);
                candleIndex = owner.chart.StockSeries.GetIndexByCandleOpen(dateStart);
            }

            if (itemNode.Attributes["id"] != null)
                id = int.Parse(itemNode.Attributes["id"].Value);

            if (itemNode.Attributes["exitId"] != null)
                exitPair = int.Parse(itemNode.Attributes["exitId"].Value);
        }

        public ChartObjectMarker IsInMarker(int screenX, int screenY, Keys modifierKeys)
        {
            var ptClient = Owner.Owner.PointToClient(new Point(screenX, screenY));
            return editMarkers[0].IsIn(ptClient.X, ptClient.Y,
                Owner.Owner.WorldRect, Owner.Owner.CanvasRect) ? editMarkers[0] : null;
        }

        public void OnMarkerMoved(ChartObjectMarker marker)
        {
            marker.RecalculateModelCoords(Owner.Owner.WorldRect, Owner.Owner.CanvasRect);
            candleIndex = marker.centerModel.X;
            Price = marker.centerModel.Y;
        }

        public void DrawMarker(Graphics g, RectangleD worldRect, Rectangle canvasRect,
            PenStorage penStorage, BrushesStorage brushStorage)
        {
            editMarkers[0].CalculateScreenCoords(worldRect, canvasRect);
            editMarkers[0].Draw(g, penStorage, brushStorage);
        }

        public void AjustColorScheme(CandleChartControl chart)
        {
            var clBack = chart.chart.visualSettings.ChartBackColor;
            colorText = ChartControl.ChartVisualSettings.AdjustColor(colorText, clBack);
        }

        public Image CreateSample(Size sizeHint)
        {
            var result = new Bitmap(sizeHint.Width, sizeHint.Height);
            var ptCenter = new PointD(sizeHint.Width / 2.0, sizeHint.Height / 2.0);
            Draw(Graphics.FromImage(result), ptCenter, new Font(FontFamily.GenericSansSerif, 8));
            return result;
        }
    }
}
