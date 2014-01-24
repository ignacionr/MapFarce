using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MapFarce.UI;
using System.Xml;
using System.Windows.Forms;

namespace MapFarce.DataModel
{
    public abstract class Savable
    {
        public abstract XmlNode CreateXmlNode(XmlNode parent);
        protected abstract void SaveToXml(XmlNode node);
        public abstract void PopulateFromXml(XmlNode node);
    }

    public abstract class ProjectElement<T, U> : Savable
        where T : ProjectElement<T, U>
        where U : UserControl
    {
        public abstract string Name { get; }
        public bool HasChanges { get; set; }

        public U ProjectControl { get; set; }

        public abstract bool InitializeNew();

        protected void SaveBounds(XmlNode node)
        {
            var pos = node.OwnerDocument.CreateElement("Bounds");
            node.AppendChild(pos);

            var attrib = node.OwnerDocument.CreateAttribute("X");
            attrib.InnerText = ProjectControl.Bounds.X.ToString();
            pos.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute("Y");
            attrib.InnerText = ProjectControl.Bounds.Y.ToString();
            pos.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute("W");
            attrib.InnerText = ProjectControl.Bounds.Width.ToString();
            pos.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute("H");
            attrib.InnerText = ProjectControl.Bounds.Height.ToString();
            pos.Attributes.Append(attrib);
        }
    }
}
