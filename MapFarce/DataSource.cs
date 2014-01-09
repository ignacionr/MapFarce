using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MapFarce
{
    public abstract class DataSource
    {
        public abstract string Name { get; }
        
        public abstract void BeginRead();
        public abstract DataItem ReadNextItem();
        public abstract void FinishRead();
    }

    public abstract class DataSource<DataType, FieldType> : DataSource
        where DataType : DataType<FieldType>
        where FieldType : IDataField
    {
        protected abstract IList<DataType> RetrieveDataTypes();
        private IList<DataType> DataTypes;
        public IList<DataType> GetDataTypes()
        {
            if (DataTypes == null)
                DataTypes = RetrieveDataTypes();
            return DataTypes;
        }

        IEnumerator<DataType> readEnumerator;
        public override void BeginRead()
        {
            readEnumerator = GetDataTypes().GetEnumerator();
            readEnumerator.MoveNext();
            readEnumerator.Current.StartRead();
        }
        
        public override DataItem ReadNextItem()
        {
            return ReadNext();
        }
        
        public DataItem ReadNext()
        {
            while (!readEnumerator.Current.HasMoreData())
            {
                readEnumerator.Current.FinishRead();
                if (!readEnumerator.MoveNext())
                    return null;

                readEnumerator.Current.StartRead();
            }

            return readEnumerator.Current.ReadNext();
        }

        public override void FinishRead()
        {
            readEnumerator = null;
        }
    }


    public abstract class DataType<DataField>
        where DataField : IDataField
    {
        public abstract string Name { get; }

        protected abstract IList<DataField> RetrieveFields();
        private IList<DataField> Fields;
        public IList<DataField> GetFields()
        {
            if (Fields == null)
                Fields = RetrieveFields();
            return Fields;
        }

        public abstract void StartRead();
        public abstract void FinishRead();

        public abstract DataItem ReadNext();
        public abstract bool HasMoreData();
    }


    public class DataItem
    {
        public DataItem(int fieldCount)
        {
            FieldCount = fieldCount;
            Values = new SortedList<IDataField, object>(FieldCount);
        }

        public int FieldCount { get; private set; }
        public IDataField GetField(int i) { return Values.Keys[i]; }
        
        public object GetValue(IDataField field)
        {
            return Values[field];
        }

        public object GetValue(int i)
        {
            return Values.Values[i];
        }

        public void AddValue(IDataField field, object value)
        {
            Values.Add(field, value);
        }

        SortedList<IDataField, object> Values;
    }

    public interface IDataField : IComparable<IDataField>
    {
        string Name { get; }
        //Type Type { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class UIEditableProperty : System.Attribute
    {
        public string Description { get; private set; }
        public string Group { get; private set; }
        public bool ReloadData { get; private set; }

        public UIEditableProperty(string desc, string group, bool reloadData)
        {
            Description = desc;
            Group = group;
            ReloadData = reloadData;
        }
    }
}
