using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MapFarce.EditProperties
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class UIEditableProperty : System.Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Group { get; private set; }
        public bool ReloadData { get; private set; }
        public PropertyInfo Property { get; set; }

        public UIEditableProperty(string desc, string group, bool reloadData)
        {
            Description = desc;
            Group = group;
            ReloadData = reloadData;
        }
    }
}
