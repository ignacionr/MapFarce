using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    public abstract class EditPropertyBase : FlowLayoutPanel
    {
        public EditPropertyBase(string labelText)
        {
            Height = 28;
            
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = false;

            if (labelText != null)
            {
                label = new Label();
                label.Text = labelText;

                label.AutoSize = false;
                label.Width = 80;
                label.Margin = new Padding(2,6,0,0);

                Controls.Add(label);
            }
        }

        Label label;

        public virtual void SetToolTip(ToolTip tip, string text)
        {
            tip.SetToolTip(this, text);

            if (label != null)
                tip.SetToolTip(label, text);
        }

        public abstract void SetValue(object val);
        public abstract object GetValue();
    }
}
