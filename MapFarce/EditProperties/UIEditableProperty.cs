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
        public string Label { get; private set; }
        public string Tooltip { get; private set; }
        public string Group { get; private set; }
        public PropertyInfo Property { get; set; }

        public UIEditableProperty(string label, string tooltip, string group)
        {
            Label = label;
            Tooltip = tooltip;
            Group = group;
        }
    }
}
