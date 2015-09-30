using System;
using System.Reflection;
using System.Windows.Forms;

namespace Candlechart.Controls
{
    public class PopupEditorHost : ToolStripDropDown
    {
        protected Control Content;

        private readonly Func<Control, object> getControlValue;
        /// <summary>
        /// after conversion event
        /// </summary>
        public Action<object> onValueUpdated;

        public PopupEditorHost()
        {
        }

        public PopupEditorHost(Control control,
            int left, int top, int width, int height,
            Func<Control, object> getControlValue, 
            Action<object> onValueUpdated)
            : this()
        {
            this.getControlValue = getControlValue;
            this.onValueUpdated = onValueUpdated;
            Content = control;

            Margin = Padding.Empty;
            Padding = Padding.Empty;
            //AutoSize = true;
            Width = width;
            Height = height;
            Left = left;
            Top = top;
            Content.Dock = DockStyle.Fill;

            BindContentHandlers();

            var host = new ToolStripControlHost(Content)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Width = width,
                Height = height
            };
            Items.Add(host);
            Opened += (sender, e) => Content.Focus();
        }

        private void BindContentHandlers()
        {
            var ctrlType = Content.GetType();
            var onKeyUp = ctrlType.GetEvent("KeyUp");
            if (onKeyUp != null)
            {
                var method = GetType().GetMethod("OnControlKeyUp", BindingFlags.Public | BindingFlags.Instance);
                var handler = Delegate.CreateDelegate(onKeyUp.EventHandlerType, this, method);
                onKeyUp.AddEventHandler(Content, handler);
            }
        }

        public void OnControlKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                return;
            }
            if (e.KeyCode != Keys.Return && e.KeyCode != Keys.Tab)
                return;
            OnValueChanged();
            Close();
        }

        private void OnValueChanged()
        {
            var resultedValue = getControlValue(Content);
            onValueUpdated(resultedValue);            
        }
    }
}
