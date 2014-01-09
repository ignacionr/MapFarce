using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    class StringEditProperty : EditPropertyBase
    {
        public StringEditProperty(string name)
            : base(name)
        {
            tb = new TextBox();
            tb.Width = 200;
            Controls.Add(tb);
        }

        TextBox tb;

        public override void SetToolTip(ToolTip tip, string text)
        {
            base.SetToolTip(tip, text);
            tip.SetToolTip(tb, text);
        }

        public override void SetValue(object val)
        {
            tb.Text = val.ToString();
        }

        public override object GetValue()
        {
            return tb.Text;
        }
    }
}
