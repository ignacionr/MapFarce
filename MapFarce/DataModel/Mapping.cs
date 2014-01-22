using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFarce.UI;
using System.Windows.Forms;
using System.Xml;

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

        public List<Connection> Inputs = new List<Connection>();
        public List<Connection> Outputs = new List<Connection>();

        public override bool InitializeNew()
        {
            var popup = new MappingCreatePopup();
            popup.Populate(this);
            return popup.ShowDialog() == DialogResult.OK;
        }

        public override void SaveToXml(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public static Mapping LoadFromXml(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public class Connection
        {
            public List<DataType> DataTypes = new List<DataType>();
        }
    }
}
