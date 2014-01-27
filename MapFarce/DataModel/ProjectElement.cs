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

    public interface IProjectControl
    {
        void SetName(string name);
    }

    public abstract class ProjectElement<T, U> : Savable
        where T : ProjectElement<T, U>
        where U : UserControl, IProjectControl
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (ProjectControl != null)
                    ProjectControl.SetName(value);
            }
        }
        public bool HasChanges { get; set; }

        public U ProjectControl { get; set; }

        public abstract bool InitializeNew();

        const string boundsNodeName = "Bounds", xposAttr = "X", yposAttr = "Y", widthAttr = "W", heightAttr = "H";
        protected void SaveBounds(XmlNode node)
        {
            var pos = node.OwnerDocument.CreateElement(boundsNodeName);
            node.AppendChild(pos);

            var attrib = node.OwnerDocument.CreateAttribute(xposAttr);
            attrib.InnerText = ProjectControl.Bounds.X.ToString();
            pos.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute(yposAttr);
            attrib.InnerText = ProjectControl.Bounds.Y.ToString();
            pos.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute(widthAttr);
            attrib.InnerText = ProjectControl.Bounds.Width.ToString();
            pos.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute(heightAttr);
            attrib.InnerText = ProjectControl.Bounds.Height.ToString();
            pos.Attributes.Append(attrib);
        }

        protected internal void LoadBounds(XmlNode node)
        {
            var pos = node.FirstChild;
            if (pos == null || pos.Name != boundsNodeName)
                throw new FormatException();

            var attrib = pos.Attributes[xposAttr];
            if ( attrib == null )
                throw new FormatException();
            int x = int.Parse(attrib.Value);

            attrib = pos.Attributes[yposAttr];
            if (attrib == null)
                throw new FormatException();
            int y = int.Parse(attrib.Value);

            attrib = pos.Attributes[widthAttr];
            if (attrib == null)
                throw new FormatException();
            int w = int.Parse(attrib.Value);

            attrib = pos.Attributes[heightAttr];
            if (attrib == null)
                throw new FormatException();
            int h = int.Parse(attrib.Value);

            ProjectControl.Bounds = new Rectangle(x, y, w, h);
        }
    }
}
