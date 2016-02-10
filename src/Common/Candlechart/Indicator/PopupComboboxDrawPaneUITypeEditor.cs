using System;
using System.Drawing;
using System.Windows.Forms;
using Candlechart.Controls;

namespace Candlechart.Indicator
{
    public class PopupComboboxDrawPaneUITypeEditor : ComboBoxDrawPaneUITypeEditor
    {
        public void ShowDialogAsynch(object selectedObject, Action<object> onUpdated,
            Rectangle bounds, Control parent)
        {
            indi = selectedObject as IChartIndicator;

            LoadItems();
            var popup = new PopupEditorHost(lb,
                0, 0, bounds.Left, bounds.Height, control => GetControlValue(), onUpdated);
            popup.Show(parent, new Point(bounds.Left, bounds.Top));
        }
    }
}
