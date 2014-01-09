using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.EditProperties
{
    public partial class EditPropertyBase : UserControl
    {
        public EditPropertyBase(string name, string desc)
        {
            InitializeComponent();
            lblName.Text = name;
            lblDesc.Text = desc;
        }
    }
}
