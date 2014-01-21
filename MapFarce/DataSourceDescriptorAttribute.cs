using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFarce
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class DataSourceDescriptorAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public DataSourceDescriptorAttribute(string name)
        {
            Name = name;
        }

        private static SortedList<Type, string> namesByType = new SortedList<Type, string>();
        public static string GetName(Type type)
        {
            string name;
            if (namesByType.TryGetValue(type, out name))
                return name;

            var attributes = type.GetCustomAttributes(typeof(DataSourceDescriptorAttribute), false);
            if (attributes.Length == 0)
                return null;

            DataSourceDescriptorAttribute attrib = attributes[0] as DataSourceDescriptorAttribute;
            namesByType.Add(type, attrib.Name);
            return attrib.Name;
        }
    }
}
