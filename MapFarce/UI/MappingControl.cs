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
    public partial class MappingControl : UserControl, IProjectControl
    {
        public MappingControl()
        {
            InitializeComponent();
            
            lblName.MouseDown += (o,e) => OnMouseDown(e);
            lblName.MouseUp += (o, e) => OnMouseUp(e);
            lblName.MouseMove += (o, e) => OnMouseMove(e);
        }

        public void SetName(string name)
        {
            lblName.Text = name;
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
            var popup = new MappingConnectionPopup();

            var connection = new Mapping.Connection(Element);
            popup.Populate(Element, connection, DataSource.Mode.Input);

            if (popup.ShowDialog() != DialogResult.OK)
                return;

            Element.Inputs.Add(connection);
            InputConnector.Recalculate();

            Parent.Invalidate();
        }

        private void lnkAddOutput_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var popup = new MappingConnectionPopup();

            var connection = new Mapping.Connection(Element);
            popup.Populate(Element, connection, DataSource.Mode.Output);

            if (popup.ShowDialog() != DialogResult.OK)
                return;

            Element.Outputs.Add(connection);
            OutputConnector.Recalculate();

            Parent.Invalidate();
        }
    }
}
