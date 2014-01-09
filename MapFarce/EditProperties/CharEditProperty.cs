using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    class CharEditProperty : EditPropertyBase
    {
        public CharEditProperty(string name, string desc)
            : base(name, desc)
        {
            tb = new TextBox();
            tb.Width = 20;
            tb.MaxLength = 1;
            Controls.Add(tb);
            tb.Top = 8;
            tb.Left = 100;
        }

        TextBox tb;
    }
}
