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
        public Mode DataMode { get; set; }
        public abstract bool InitializeNew();

        public abstract IList<DataType> GetDataTypesBase();

        public abstract void BeginRead();
        public abstract DataItem ReadNextItem();
        public abstract void FinishRead();

        public abstract void PropertiesChanged();

        public enum Mode
        {
            Input,
            Output,
        }
    }

    public abstract class DataSource<Data, Field> : DataSource
        where Data : DataType<Field>
        where Field : IDataField
    {
        protected abstract IList<Data> RetrieveDataTypes();
        private IList<Data> DataTypes;
        public IList<Data> GetDataTypes()
        {
            if (DataTypes == null)
                DataTypes = RetrieveDataTypes();
            return DataTypes;
        }

        public override IList<DataType> GetDataTypesBase()
        {
            var types = GetDataTypes();
            List<DataType> typesBase = new List<DataType>(types.Count);
            foreach (var type in types)
                typesBase.Add(type);
            return typesBase;
        }

        public override void PropertiesChanged()
        {
            // i guess the available data types would change if the properties change.
            // I don't see how we'd keep data connections hooked up if the data types are completely replaced.
            // will hold off for now.
            //DataTypes = null;
            foreach (var type in GetDataTypes())
                type.SourceChanged();
        }

        IEnumerator<Data> readEnumerator;
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

    public abstract class DataType
    {
        public abstract string Name { get; }
        public abstract IList<IDataField> GetFieldsBase();
        public abstract void SourceChanged();
    }

    public abstract class DataType<Data> : DataType
        where Data : IDataField
    {
        protected abstract IList<Data> RetrieveFields();
        private IList<Data> Fields;
        public IList<Data> GetFields()
        {
            if (Fields == null)
                Fields = RetrieveFields();
            return Fields;
        }

        public override IList<IDataField> GetFieldsBase()
        {
            var fields = GetFields();
            List<IDataField> fieldsBase = new List<IDataField>(fields.Count);
            foreach (var field in fields)
                fieldsBase.Add(field);
            return fieldsBase;
        }

        public override void SourceChanged()
        {
            Fields = null; // let these be reloaded
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
}
