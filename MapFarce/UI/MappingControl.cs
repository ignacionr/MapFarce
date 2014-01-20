using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFarce.DataModel;

namespace MapFarce.UI
{
    public partial class MappingControl : UserControl
    {
        public MappingControl()
        {
            InitializeComponent();

            foreach (Control control in Controls)
            {
                control.MouseDown += (o,e) => OnMouseDown(e);
                control.MouseUp += (o,e) => OnMouseUp(e);
                control.MouseMove += (o,e) => OnMouseMove(e);
            }
        }

        public Mapping Mapping { get; private set; }

        public void Populate(Mapping mapping)
        {
            Mapping = mapping;
            lblName.Text = mapping.Name;
        }
    }
}
