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

    public abstract class DataSource<DataType, ItemType, FieldType> : DataSource
        where DataType : DataType<ItemType, FieldType>
        where ItemType : DataItem<FieldType>
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
        
        public ItemType ReadNext()
        {
            while (!readEnumerator.Current.HasMoreData())
            {
                readEnumerator.Current.FinishRead();
                if (!readEnumerator.MoveNext())
                    return default(ItemType);

                readEnumerator.Current.StartRead();
            }

            return readEnumerator.Current.ReadNext();
        }

        public override void FinishRead()
        {
            readEnumerator = null;
        }
    }


    public abstract class DataType<DataItem, DataField>
        where DataItem : DataItem<DataField>
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


    public abstract class DataItem
    {
        public abstract int FieldCount { get; }
        public abstract IDataField GetField(int i);
        public abstract object GetValue(int i);
    }


    public abstract class DataItem<DataField> : DataItem
        where DataField : IDataField
    {
        protected IList<DataField> Fields { get; set; }

        public override int FieldCount { get { return Fields.Count; } }
        public override IDataField GetField(int i) { return (IDataField)Fields[i]; }

        public abstract object GetValue(DataField field);

        public override object GetValue(int i)
        {
            return GetValue(Fields[i]);
        }
    }


    public interface IDataField
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
