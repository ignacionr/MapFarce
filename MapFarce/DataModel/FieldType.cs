using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MapFarce.DataModel
{
    public struct FieldType
    {
        public static FieldType String = new FieldType(typeof(string));
        public static FieldType Bool = new FieldType(typeof(bool));
        public static FieldType Byte = new FieldType(typeof(byte));
        public static FieldType Short = new FieldType(typeof(short));
        public static FieldType Int = new FieldType(typeof(int));
        public static FieldType Long = new FieldType(typeof(long));
        public static FieldType Float = new FieldType(typeof(float));
        public static FieldType Double = new FieldType(typeof(double));
        public static FieldType Decimal = new FieldType(typeof(decimal));
        public static FieldType DateTime = new FieldType(typeof(DateTime));

        private static FieldType[] allTypes = null;
        public static FieldType[] AllTypes
        {
            get
            {
                if (allTypes == null)
                {
                    List<FieldType> types = new List<FieldType>();

                    var type = typeof(FieldType);
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                    foreach (var info in fields)
                    {
                        var instance = (FieldType)Activator.CreateInstance(type, true);
                        object value = info.GetValue(instance);

                        if (value is FieldType)
                            types.Add((FieldType)value);
                    }

                    allTypes = types.ToArray();
                }

                return allTypes;
            }
        }

        private FieldType(Type type)
        {
            this.type = type;
        }

        private Type type;

        public string Name { get { return type.Name; } }
        public Type Type { get { return type; } }
    }
}
