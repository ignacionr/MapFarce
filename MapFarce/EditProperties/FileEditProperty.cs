using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MapFarce.EditProperties
{
    class FileEditProperty : EditPropertyBase
    {
        public FileEditProperty(string name)
            : base(name)
        {
            tb = new TextBox();
            tb.Width = 140;
            Controls.Add(tb);

            b = new Button();
            b.Text = "Browse";
            b.Width = 70;
            Controls.Add(b);
        }

        TextBox tb;
        Button b;

        public override void SetToolTip(ToolTip tip, string text)
        {
            base.SetToolTip(tip, text);
            tip.SetToolTip(tb, text);

            tip.SetToolTip(b, "Click to browse for a file");
        }

        public override void SetValue(object val)
        {
            tb.Text = val.ToString();
        }

        public override object GetValue()
        {
            return new FileInfo(tb.Text);
        }
    }
}
