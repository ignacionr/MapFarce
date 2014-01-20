using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFarce.UI;
using System.Windows.Forms;

namespace MapFarce.DataModel
{
    public class Mapping : ProjectElement<Mapping, MappingControl>
    {
        private string name;
        public override string Name { get { return name; } }
        public void SetName(string n)
        {
            name = n;
            if (ProjectControl != null)
                ProjectControl.lblName.Text = name;
        }

        public List<DataType> Inputs = new List<DataType>();
        public List<DataType> Outputs = new List<DataType>();

        public override bool InitializeNew()
        {
            var popup = new MappingEditPopup();
            popup.Populate(this);
            return popup.ShowDialog() == DialogResult.OK;
        }
    }
}
