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

        public override XmlNode CreateXmlNode(XmlNode parent)
        {
            var node = parent.OwnerDocument.CreateElement("Mapping");
            parent.AppendChild(node);

            SaveBounds(node);

            SaveToXml(node);
            return node;
        }

        protected override void SaveToXml(XmlNode node)
        {
            SaveConnections(node, "Inputs", Inputs);
            SaveConnections(node, "Outputs", Outputs);

            // also save the contents of this mapping
        }

        private void SaveConnections(XmlNode node, string name, List<Connection> connections)
        {
            var root = node.OwnerDocument.CreateElement(name);
            node.AppendChild(root);

            foreach (var connection in connections)
            {
                var conNode = node.OwnerDocument.CreateElement("Connection");
                root.AppendChild(conNode);

                foreach (var dt in connection.DataTypes)
                {
                    var attrib = node.OwnerDocument.CreateAttribute("source");
                    attrib.InnerText = dt.SourceBase.Name;
                    conNode.Attributes.Append(attrib);

                    attrib = node.OwnerDocument.CreateAttribute("type");
                    attrib.InnerText = dt.Name;
                    conNode.Attributes.Append(attrib);
                }
            }
        }

        public static Mapping LoadFromXml(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public override void PopulateFromXml(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public class Connection
        {
            public List<DataType> DataTypes = new List<DataType>();
        }
    }
}
