using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MapFarce.DataModel
{
    public abstract class DataField : Savable, IComparable<DataField>
    {
        protected DataField(string name, FieldType type)
        {
            Name = DisplayName = name;
            Type = type;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public FieldType Type { get; set; }
        public int FieldNumber { get; set; }

        public int CompareTo(DataField other)
        {
            int diff = FieldNumber.CompareTo(other.FieldNumber);
            return diff != 0 ? diff : Name.CompareTo(other.Name);
        }

        public override XmlNode CreateXmlNode(XmlNode parent)
        {
            var node = parent.OwnerDocument.CreateElement("Field");
            parent.AppendChild(node);

            SaveToXml(node);
            return node;
        }

        protected override void SaveToXml(XmlNode node)
        {
            var attrib = node.OwnerDocument.CreateAttribute("name");
            attrib.Value = Name;
            node.Attributes.Append(attrib);

            if (DisplayName != Name)
            {
                attrib = node.OwnerDocument.CreateAttribute("display");
                attrib.Value = DisplayName;
                node.Attributes.Append(attrib);
            }

            attrib = node.OwnerDocument.CreateAttribute("type");
            attrib.Value = Type.Name;
            node.Attributes.Append(attrib);
        }

        public static DataType LoadFromXml(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public override void PopulateFromXml(XmlNode node)
        {
            throw new NotImplementedException();
        }
    }
}
