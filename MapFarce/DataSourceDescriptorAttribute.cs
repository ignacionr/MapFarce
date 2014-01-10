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
    }
}
