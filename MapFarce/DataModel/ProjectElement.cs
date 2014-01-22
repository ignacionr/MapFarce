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
    public abstract class ProjectElement<T, U> 
        where T : ProjectElement<T, U>
        where U : UserControl
    {
        public abstract string Name { get; }
        public bool HasChanges { get; set; }

        public U ProjectControl { get; set; }

        public abstract bool InitializeNew();

        public abstract void SaveToXml(XmlNode node);
    }
}
