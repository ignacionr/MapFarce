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
    public partial class MappingControl : ProjectControl<Mapping, MappingControl>
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

        public override void Populate(Mapping element)
        {
            Element = element;
            lblName.Text = element.Name;
        }
    }
}
