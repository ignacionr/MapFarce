using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MapFarce.DataModel
{
    public class DataField : Savable, IComparable<DataField>
    {
        public DataField(string name, FieldType type)
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

        public const string fieldNodeName = "Field";
        public override XmlNode CreateXmlNode(XmlNode parent)
        {
            var node = parent.OwnerDocument.CreateElement(fieldNodeName);
            parent.AppendChild(node);

            SaveToXml(node);
            return node;
        }

        private const string nameAttributeName = "name", displayNameAttributeName = "display", typeAttributeName = "type";
        protected override void SaveToXml(XmlNode node)
        {
            var attrib = node.OwnerDocument.CreateAttribute(nameAttributeName);
            attrib.Value = Name;
            node.Attributes.Append(attrib);

            if (DisplayName != Name)
            {
                attrib = node.OwnerDocument.CreateAttribute(displayNameAttributeName);
                attrib.Value = DisplayName;
                node.Attributes.Append(attrib);
            }

            attrib = node.OwnerDocument.CreateAttribute(typeAttributeName);
            attrib.Value = Type.Name;
            node.Attributes.Append(attrib);
        }

        public static DataField LoadFromXml(XmlNode node)
        {
            var attrib = node.Attributes[nameAttributeName];
            if (attrib == null)
                throw new FormatException();
            string name = attrib.Value;

            attrib = node.Attributes[typeAttributeName];
            if (attrib == null)
                throw new FormatException();
            FieldType type = FieldType.GetByName(attrib.Value);

            var df = new DataField(name, type);
            df.PopulateFromXml(node);
            return df;
        }

        public override void PopulateFromXml(XmlNode node)
        {
            var attrib = node.Attributes[nameAttributeName];
            if (attrib != null)
                DisplayName = attrib.Value;
        }
    }
}
