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
            
            lblName.MouseDown += (o,e) => OnMouseDown(e);
            lblName.MouseUp += (o, e) => OnMouseUp(e);
            lblName.MouseMove += (o, e) => OnMouseMove(e);
        }

        public MappingConnector InputConnector { get; private set; }
        public MappingConnector OutputConnector { get; private set; }

        public Mapping Element { get; private set; }

        public void Populate(Mapping element)
        {
            if (Element != null)
                return;

            Element = element;
            element.ProjectControl = this;
            lblName.Text = element.Name;

            InputConnector = new MappingConnector(this, DataSource.Mode.Input);
            OutputConnector = new MappingConnector(this, DataSource.Mode.Output);
            Parent.Controls.Add(InputConnector);
            Parent.Controls.Add(OutputConnector);

            Move += OnMove;
        }

        void OnMove(object sender, EventArgs e)
        {
            InputConnector.Recalculate();
            OutputConnector.Recalculate();
        }

        private void lnkAddInput_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // for now, just duplicate existing connection
            Element.Inputs.Add(Element.Inputs[0]);
            InputConnector.Recalculate();
            
            Parent.Refresh();
        }

        private void lnkAddOutput_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // for now, just duplicate existing connection
            Element.Outputs.Add(Element.Outputs[0]);
            OutputConnector.Recalculate();

            Parent.Invalidate();
        }
    }
}
