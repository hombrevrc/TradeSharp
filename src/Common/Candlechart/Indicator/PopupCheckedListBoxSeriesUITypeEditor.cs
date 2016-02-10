using System;
using System.Drawing;
using System.Windows.Forms;
using Candlechart.Controls;

namespace Candlechart.Indicator
{
    public class PopupCheckedListBoxSeriesUITypeEditor : CheckedListBoxSeriesUITypeEditor
    {
        public void ShowDialogAsynch(object selectedObject, Action<object> onUpdated,
            Rectangle bounds, Control parent)
        {
            indi = (BaseChartIndicator) selectedObject;
            LoadListBoxItems();
            var popup = new PopupEditorHost(cbx,
                0, 0, bounds.Left, bounds.Height, control => GetListBoxValue(), onUpdated);
            popup.Show(parent, new Point(bounds.Left, bounds.Top));
        }
    }
}
