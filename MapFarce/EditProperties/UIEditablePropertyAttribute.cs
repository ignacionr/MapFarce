using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace MapFarce.EditProperties
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class UIEditablePropertyAttribute : System.Attribute
    {
        public string Label { get; private set; }
        public string Tooltip { get; private set; }
        public string Group { get; private set; }
        public PropertyInfo Property { get; set; }

        public UIEditablePropertyAttribute(string label, string tooltip, string group)
        {
            Label = label;
            Tooltip = tooltip;
            Group = group;
        }

        private static SortedList<Type, List<UIEditablePropertyAttribute>> attribsByType = new SortedList<Type,List<UIEditablePropertyAttribute>>();
        public static List<UIEditablePropertyAttribute> GetAll(Type type)
        {
            List<UIEditablePropertyAttribute> attribs;
            if ( !attribsByType.TryGetValue(type, out attribs) )
            {
                attribs = new List<UIEditablePropertyAttribute>();

                foreach (var prop in type.GetProperties())
                {
                    var attributes = prop.GetCustomAttributes(typeof(UIEditablePropertyAttribute), false);
                    if (attributes.Length == 0)
                        continue;

                    UIEditablePropertyAttribute attrib = attributes[0] as UIEditablePropertyAttribute;
                    attrib.Property = prop;
                    attribs.Add(attrib);
                }

                attribsByType.Add(type, attribs);
            }

            return attribs;
        }

        public EditPropertyBase GetControlToEdit(object o)
        {
            EditPropertyBase c;
            if (Property.PropertyType == typeof(FileInfo))
                c = new FileEditProperty(Label);
            else if (Property.PropertyType == typeof(bool))
                c = new BoolEditProperty(Label);
            else if (Property.PropertyType == typeof(char))
                c = new CharEditProperty(Label);
            else if (Property.PropertyType == typeof(string))
                c = new StringEditProperty(Label);
            else
                throw new Exception("Not configured to edit " + Property.PropertyType.Name + " properties!");

            c.SetValue(Property.GetValue(o, null));
            return c;
        }

        public string GetStringFromValue(object val)
        {
            if (Property.PropertyType == typeof(FileInfo))
                return ((FileInfo)val).FullName;
            else
                return val.ToString();
        }

        public void SetValueFromString(object o, string strVal)
        {
            object val;
            if (Property.PropertyType == typeof(FileInfo))
                val = new FileInfo(strVal);
            else if (Property.PropertyType == typeof(bool))
                val = bool.Parse(strVal);
            else if (Property.PropertyType == typeof(char))
                val = strVal[0];
            else if (Property.PropertyType == typeof(string))
                val = strVal;
            else
                throw new Exception("Not configured to edit " + Property.PropertyType.Name + " properties!");

            Property.SetValue(o, val, null);
        }
    }
}
