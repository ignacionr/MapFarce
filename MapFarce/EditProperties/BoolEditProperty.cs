using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    class BoolEditProperty : EditPropertyBase
    {
        public BoolEditProperty(string name, string desc)
            : base(name, desc)
        {
            cb = new CheckBox();
            Controls.Add(cb);
            cb.Top = 8;
            cb.Left = 100;
        }

        CheckBox cb;
    }
}
