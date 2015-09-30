using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Entity;
using TradeSharp.Util;

namespace Candlechart.Indicator
{
    public class CheckedListBoxSeriesUITypeEditor : UITypeEditor
    {
        protected IWindowsFormsEditorService es;

        protected BaseChartIndicator indi;

        public CheckedListBox cbx = new CheckedListBox();       

        public CheckedListBoxSeriesUITypeEditor()
        {
            cbx.Leave += Leave;
            cbx.KeyDown += CbxKeyDown;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get { return true; }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            es = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
            if (es == null) return null;

            indi = (BaseChartIndicator) context.Instance;

            LoadListBoxItems();
            es.DropDownControl(cbx);
            return value;
        }

        public string GetListBoxValue()
        {
            return string.Join(Separators.SourcesDelimiter[0], cbx.CheckedItems.Cast<string>().ToArray());
        }

        protected void LoadListBoxItems()
        {
            cbx.Items.Clear();
            cbx.Items.Add(Localizer.GetString("TitleCourse"));
            var chart = indi.owner.Owner;
            foreach (var i in chart.indicators)
            {
                if (i == indi) continue;
                foreach (var ser in i.SeriesResult)
                {
                    cbx.Items.Add(i.UniqueName + Separators.IndiNameDelimiter[0] + ser.Name);
                }
            }
            if (string.IsNullOrEmpty(indi.SeriesSourcesDisplay)) return;
            
            var items = indi.SeriesSourcesDisplay.Split(Separators.SourcesDelimiter, StringSplitOptions.None);
            foreach (var item in items)
            {
                for (var i = 0; i < cbx.Items.Count; i++)
                {
                    if ((string) cbx.Items[i] != item) continue;
                    // совпало, отмечаем 
                    cbx.SetItemChecked(i, true);
                    i = cbx.Items.Count;
                }
            }
        }

        private void Leave(Object sender, EventArgs e)
        {
            if (cbx.CheckedItems.Count == 0)
            {
                indi.SeriesSourcesDisplay = string.Empty;
                return;
            }
            indi.SeriesSourcesDisplay = GetListBoxValue();                            
        }
        
        private void CbxKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                var selIndex = cbx.SelectedIndex;
                if (selIndex <= 0) return;
                var isChecked = cbx.GetItemChecked(selIndex);
                var obj = cbx.Items[selIndex];
                cbx.Items.RemoveAt(selIndex);
                cbx.Items.Insert(selIndex - 1, obj);
                cbx.SetItemChecked(selIndex - 1, isChecked);
                cbx.SelectedIndex = selIndex;
            }

            if (e.KeyCode == Keys.Down)
            {
                var selIndex = cbx.SelectedIndex;
                if (selIndex <= 0 || selIndex == cbx.Items.Count - 1) return;
                var isChecked = cbx.GetItemChecked(selIndex);
                var obj = cbx.Items[selIndex];
                cbx.Items.RemoveAt(selIndex);
                cbx.Items.Insert(selIndex + 1, obj);
                cbx.SetItemChecked(selIndex + 1, isChecked);
                cbx.SelectedIndex = selIndex + 1;
            }
        }
    }
}
