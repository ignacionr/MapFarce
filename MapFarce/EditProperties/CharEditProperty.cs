using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    class CharEditProperty : EditPropertyBase
    {
        public CharEditProperty(string name)
            : base(name)
        {
            tb = new TextBox();
            tb.Width = 20;
            tb.MaxLength = 1;
            Controls.Add(tb);
            tb.TextAlign = HorizontalAlignment.Center;
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
            return tb.Text.Length > 0 ? tb.Text[0] : 'X';
        }
    }
}
