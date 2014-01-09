using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    class BoolEditProperty : EditPropertyBase
    {
        public BoolEditProperty(string name)
            : base(name)
        {
            cb = new CheckBox();
            Controls.Add(cb);
        }

        CheckBox cb;

        public override void SetToolTip(ToolTip tip, string text)
        {
            base.SetToolTip(tip, text);
            tip.SetToolTip(cb, text);
        }

        public override void SetValue(object val)
        {
            if (val is bool)
                cb.Checked = (bool)val;
            else
                cb.Checked = false;
        }

        public override object GetValue()
        {
            return cb.Checked;
        }
    }
}
