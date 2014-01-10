using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFarce.DataModel
{
    public abstract class DataField : IComparable<DataField>
    {
        public abstract string Name { get; protected set; }
        //Type Type { get; }

        public abstract int CompareTo(DataField other);
    }
}
