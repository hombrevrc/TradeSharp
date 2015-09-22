using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using Candlechart.ChartMath;
using Candlechart.Core;
using Entity;

namespace Candlechart.Series
{
    [LocalizedSeriesToolButton(CandleChartControl.ChartTool.Marker, "TitleDealMarkers", ToolButtonImageIndex.DealMark)]
    public class SeriesMarker : InteractiveObjectSeries
    {
        public List<DealMarker> data = new List<DealMarker>();
        public override int DataCount { get { return data.Count; } }

        public SeriesMarker(string name) : base(name, CandleChartControl.ChartTool.Marker)
        {
        }

        public void AddOrRemovePoint(PointF ptScreen, bool buySide)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var marker = data[i];
                if (!marker.PointIsIn(ptScreen.X, ptScreen.Y)) continue;
                // дополнительно убить маркер выхода из сделки
                if (marker.MarkerType == DealMarker.DealMarkerType.Вход && marker.exitPair.HasValue)
                {
                    var exitId = marker.exitPair;
                    var index = data.FindIndex(0, m => m.id == exitId);
                    if (index >= 0)
                    {
                        data.RemoveAt(index);
                        data.Remove(marker);
                    }
                    return;
                }
                // снять ссылку с маркера входа
                if (marker.MarkerType == DealMarker.DealMarkerType.Выход)
                {
                    var pairId = marker.id;
                    var enter = data.FirstOrDefault(m => m.exitPair == pairId);
                    if (enter != null) enter.exitPair = null;
                }

                data.RemoveAt(i);
                return;
            }
            AddPoint(ptScreen, buySide);
        }

        public void AddPoint(PointF ptScreen, bool buySide)
        {
            var ptWorld = Conversion.ScreenToWorld(new PointD(ptScreen.X, ptScreen.Y),
                                                        Owner.WorldRect, Owner.CanvasRect);
            var side = buySide ? DealType.Buy : DealType.Sell;
            // определить время с точностью до минуты
            var timeOpen = Chart.StockSeries.GetCandleOpenTimeByIndex((int) ptWorld.X);
            var timeNext = Chart.StockSeries.GetCandleOpenTimeByIndex((int)ptWorld.X + 1);
            var deltaMinutes = (timeNext - timeOpen).TotalMinutes;
            var time = timeOpen.AddMinutes(deltaMinutes*(ptWorld.X - (int) ptWorld.X));
            // вход для этого выхода
            var enterMarket =
                data.FirstOrDefault(m => m.MarkerType == DealMarker.DealMarkerType.Вход && !m.exitPair.HasValue);
            // новый объект
            var dm = new DealMarker(Chart, data,
                DealMarker.DealMarkerType.Вход, side, ptWorld.X, ptWorld.Y, time) { Owner = this };
            if (Owner.Owner.Owner.AdjustObjectColorsOnCreation)
                dm.AjustColorScheme(Owner.Owner.Owner);
            // закрыть пару и проставить ссылку на выход из сделки
            if (enterMarket != null)
            {
                dm.MarkerType = DealMarker.DealMarkerType.Выход;
                dm.Side = enterMarket.Side;
                enterMarket.exitPair = dm.id;
            }

            data.Add(dm);
        }

        public override void Draw(Graphics g, RectangleD worldRect, Rectangle canvasRect)
        {
            base.Draw(g, worldRect, canvasRect);
            
            using (var penStorage = new PenStorage())
            using (var brushStorage = new BrushesStorage())
            foreach (var marker in data)
            {
                var ptScreen = Conversion.WorldToScreen(new PointD(marker.candleIndex, marker.Price),
                                                        worldRect, canvasRect);                
                marker.Draw(g, ptScreen, Chart.Font);

                if (marker.exitPair.HasValue)
                {
                    var pairId = marker.exitPair;
                    var pair = data.FirstOrDefault(m => m.id == pairId);
                    if (pair != null)
                    {
                        var ptPair = Conversion.WorldToScreen(new PointD(pair.candleIndex, pair.Price),
                                                        worldRect, canvasRect);                
                        // соединить две точки
                        var pen = penStorage.GetPen(Color.DarkSlateBlue, 1,
                                                    DashStyle.Dash);
                        g.DrawLine(pen, ptScreen.ToPointF(), ptPair.ToPointF());
                    }
                }
                if (marker.Selected) marker.DrawMarker(g, worldRect, canvasRect, penStorage, brushStorage);
            }            
        }

        public bool GetObjectToolTip(PointF ptScreen, ref string toolTip)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var marker = data[i];
                if (!marker.PointIsIn(ptScreen.X, ptScreen.Y)) continue;
                toolTip = marker.GetToolTip();
                return true;
            }
            return false;
        }

        #region SeriesMarker
        public override bool GetXExtent(ref double left, ref double right)
        {
            return false;
        }

        public override bool GetYExtent(double left, double right, ref double top, ref double bottom)
        {
            return false;
        }
        #endregion

        protected override void OnMouseDown(List<SeriesEditParameter> parameters,
            MouseEventArgs e, Keys modifierKeys, out IChartInteractiveObject objectToEdit)
        {
            objectToEdit = null;
            if (e.Button != MouseButtons.Left) return;
            var sellSide = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            var clientPoint = Chart.PointToScreen(new Point(e.X, e.Y));
            clientPoint = Chart.StockPane.PointToClient(clientPoint);
            AddOrRemovePoint(new PointF(clientPoint.X, clientPoint.Y), !sellSide);
        }

        public override void AddObjectsInList(List<IChartInteractiveObject> interObjects)
        {
            foreach (var item in data) interObjects.Add(item);
        }

        public override IChartInteractiveObject LoadObject(XmlElement objectNode, CandleChartControl owner, bool trimObjectsOutOfHistory = false)
        {
            var obj = new DealMarker(Chart, data);
            obj.LoadFromXML(objectNode, owner);
            obj.Owner = this;
            data.Add(obj);
            return obj;
        }

        public override void RemoveObjectFromList(IChartInteractiveObject interObject)
        {
            if (interObject == null) return;
            if (interObject is DealMarker == false) return;
            data.Remove((DealMarker)interObject);
        }

        public override void RemoveObjectByNum(int num)
        {
            data.RemoveAt(num);
        }

        public override IChartInteractiveObject GetObjectByNum(int num)
        {
            return data[num];
        }

        public override List<IChartInteractiveObject> GetObjectsUnderCursor(int screenX, int screenY, int tolerance)
        {
            var ptClient = Owner.PointToClient(new Point(screenX, screenY));
            var list = new List<IChartInteractiveObject>();
            foreach (var marker in data)
            {
                if (marker.PointIsIn(ptClient.X, ptClient.Y)) list.Add(marker);
            }
            return list;
        }

        public override void ProcessLoadingCompleted(CandleChartControl owner){}

        public override void AdjustColorScheme(CandleChartControl chart)
        {
            foreach (var obj in data)
                obj.AjustColorScheme(chart);
        }

        public override IChartInteractiveObject FindObject(Func<IChartInteractiveObject, bool> predicate, out int objIndex)
        {
            objIndex = -1;
            for (var i = 0; i < data.Count; i++)
            {
                if (predicate(data[i]))
                {
                    objIndex = i;
                    return data[i];
                }
            }
            return null;
        }
    }    
}
