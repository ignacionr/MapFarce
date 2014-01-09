using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    class FileEditProperty : EditPropertyBase
    {
        public FileEditProperty(string name, string desc)
            : base(name, desc)
        {
            tb = new TextBox();
            tb.Width = 125;
            Controls.Add(tb);
            tb.Top = 8;
            tb.Left = 100;

            b = new Button();
            b.Text = "Browse";
            b.Width = 75;
            Controls.Add(b);
            b.Top = 8;
            b.Left = 235;
        }

        TextBox tb;
        Button b;
    }
}
