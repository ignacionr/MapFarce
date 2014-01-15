﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFarce.DataModel
{
    public abstract class DataField : IComparable<DataField>
    {
        protected DataField(string name, FieldType type)
        {
            Name = DisplayName = name;
            Type = type;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public FieldType Type { get; set; }

        public abstract int CompareTo(DataField other);
    }
}