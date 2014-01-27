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
        public List<Connection> Inputs = new List<Connection>();
        public List<Connection> Outputs = new List<Connection>();

        public override bool InitializeNew()
        {
            var popup = new MappingCreatePopup();
            popup.Populate(this);
            return popup.ShowDialog() == DialogResult.OK;
        }
        
        const string mappingNodeName = "Mapping", inputsNodeName = "Inputs", outputsNodeName = "Outputs", connectionNodeName = "Connection", linkNodeName = "Link", sourceAttrName = "source", typeAttrName = "type", nameAttrName = "name";
        public override XmlNode CreateXmlNode(XmlNode parent)
        {
            var node = parent.OwnerDocument.CreateElement(mappingNodeName);
            parent.AppendChild(node);

            var attr = parent.OwnerDocument.CreateAttribute(nameAttrName);
            node.Attributes.Append(attr);
            attr.Value = Name;

            SaveBounds(node);

            SaveToXml(node);
            return node;
        }

        protected override void SaveToXml(XmlNode node)
        {
            SaveConnections(node, inputsNodeName, Inputs);
            SaveConnections(node, outputsNodeName, Outputs);

            // also save the contents of this mapping
        }

        private void SaveConnections(XmlNode node, string name, List<Connection> connections)
        {
            var root = node.OwnerDocument.CreateElement(name);
            node.AppendChild(root);

            foreach (var connection in connections)
            {
                var conNode = node.OwnerDocument.CreateElement(connectionNodeName);
                root.AppendChild(conNode);

                foreach (var dt in connection.DataTypes)
                {
                    var sourceNode = node.OwnerDocument.CreateElement(linkNodeName);
                    conNode.AppendChild(sourceNode);

                    var attrib = node.OwnerDocument.CreateAttribute(sourceAttrName);
                    attrib.InnerText = dt.SourceBase.Name;
                    sourceNode.Attributes.Append(attrib);

                    attrib = node.OwnerDocument.CreateAttribute(typeAttrName);
                    attrib.InnerText = dt.Name;
                    sourceNode.Attributes.Append(attrib);
                }
            }
        }

        public static Mapping LoadFromXml(XmlNode node)
        {
            Mapping m = new Mapping();

            //m.LoadBounds(node); can't run this now, cos it has no ProjectControl to apply this to

            var attr = node.Attributes[nameAttrName];
            if (attr == null)
                throw new FormatException();
            m.Name = attr.Value;

            m.PopulateFromXml(node);
            return m;
        }

        public override void PopulateFromXml(XmlNode node)
        {
            LoadConnections(node, inputsNodeName, Inputs);
            LoadConnections(node, outputsNodeName, Outputs);
        }

        private void LoadConnections(XmlNode node, string name, List<Connection> connections)
        {
            var root = node[name];
            if (root == null)
                throw new FormatException();

            foreach (XmlNode conNode in root.ChildNodes)
            {
                var connection = new Connection();
                connections.Add(connection);

                foreach (XmlNode sourceNode in conNode.ChildNodes)
                {
                    if (sourceNode.Name != linkNodeName)
                        continue;

                    var sourceAttr = sourceNode.Attributes[sourceAttrName];
                    var typeAttr = sourceNode.Attributes[typeAttrName];

                    if (sourceAttr == null || typeAttr == null)
                        throw new FormatException();

                    var source = Project.Instance.Sources.Where(s => s.Name == sourceAttr.Value).FirstOrDefault();
                    if (source == null)
                        throw new Exception("Source not found: " + sourceAttr.Value);

                    foreach (var type in source)
                        if (type.Name == typeAttr.Value)
                        {
                            connection.DataTypes.Add(type);
                            break;
                        }
                }
            }
        }

        public class Connection
        {
            public List<DataType> DataTypes = new List<DataType>();
        }
    }
}
