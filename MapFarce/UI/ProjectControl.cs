using MapFarce.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce.UI
{
    public abstract class ProjectControl<T, U> : UserControl
        where T : ProjectElement<T, U>
        where U : ProjectControl<T, U>
    {
        public T Element { get; protected set; }

        public abstract void Populate(T t);
    }
}
