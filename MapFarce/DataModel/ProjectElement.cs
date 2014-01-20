using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MapFarce.DataModel
{
    public abstract class ProjectElement
    {
        public abstract string Name { get; }
        public Point Location { get; set; }
        public bool HasChanges { get; set; }

        public abstract bool InitializeNew();
    }
}
