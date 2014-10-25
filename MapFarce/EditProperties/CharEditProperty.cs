using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    class CharEditProperty : EditPropertyBase
    {
        private CharEditorControl ce;
        
        public CharEditProperty(string name)
            : base(name)
        {
            this.ce = new CharEditorControl();
            //tb.Width = 20;
            //tb.MaxLength = 1;
            Controls.Add(this.ce);
        }

        public override void SetToolTip(ToolTip tip, string text)
        {
            base.SetToolTip(tip, text);
            tip.SetToolTip(this.ce, text);
        }

        public override void SetValue(object val)
        {
            this.ce.Value = (char)val;
        }

        public override object GetValue()
        {
            return this.ce.Value;
        }
    }
}
