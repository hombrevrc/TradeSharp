using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;

namespace Candlechart.Indicator
{
    public class FileBrowseUITypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, 
            IServiceProvider provider, object value)
        {
            var path = value == null ? string.Empty : (string) value;
            var dlg = new OpenFileDialog();
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                dlg.FileName = path;
            return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : value;
        }

        public override bool IsDropDownResizable
        {
            get { return false; }            
        }  
    }
}
