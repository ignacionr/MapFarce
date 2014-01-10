using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MapFarce
{
    public abstract class DataSource : IEnumerable<DataType>
    {
        public abstract string Name { get; }
        public Mode DataMode { get; set; }
        public abstract bool InitializeNew();

        public abstract IEnumerator<DataType> GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public virtual void BeginRead()
        {
            Active = this;
        }
        public virtual void FinishRead()
        {
            Active = null;
        }

        public virtual void BeginWrite()
        {
            Active = this;
        }
        public virtual void FinishWrite()
        {
            Active = null;
        }

        public DataSet ReadDataSet()
        {
            DataSet ds = new DataSet();

            BeginRead();

            foreach (var type in this)
                ds.Tables.Add(type.ReadDataTable());

            FinishRead();

            return ds;
        }

        public abstract void PropertiesChanged();

        public enum Mode
        {
            Input,
            Output,
        }

        public static DataSource Active { get; set; }
    }

    public abstract class DataSource<Source, Data, Field> : DataSource
        where Source : DataSource<Source, Data, Field>
        where Data : DataType<Source, Data, Field>
        where Field : DataField
    {
        protected abstract IList<Data> RetrieveDataTypes();
        protected IList<Data> DataTypes;

        public override IEnumerator<DataType> GetEnumerator()
        {
            if (DataTypes == null)
                DataTypes = RetrieveDataTypes();
            return DataTypes.GetEnumerator();
        }
        
        public override void PropertiesChanged()
        {
            // i guess the available data types would change if the properties change.
            // I don't see how we'd keep data connections hooked up if the data types are completely replaced.
            // will hold off for now.
            //DataTypes = null;
            foreach (var type in this)
                type.SourceChanged();
        }

        public override void BeginRead()
        {
            if (DataTypes == null)
                DataTypes = RetrieveDataTypes();
        }
    }

    public abstract class DataType : IEnumerable<DataField>
    {
        public abstract DataSource SourceBase { get; }
        public abstract string Name { get; }
        public abstract void SourceChanged();

        public abstract IEnumerator<DataField> GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public abstract IEnumerable<DataItem> ReadData();

        public virtual void BeginRead()
        {
            if (DataSource.Active != SourceBase)
                SourceBase.BeginRead();
        }
        public virtual void FinishRead() { }

        public virtual void BeginWrite()
        {
            if (DataSource.Active != SourceBase)
                SourceBase.BeginWrite();
        }
        public virtual void FinishWrite() { }

        public DataTable ReadDataTable()
        {
            BeginRead();

            DataTable dt = new DataTable();
            foreach (var field in this)
                dt.Columns.Add(field.Name);

            foreach (var item in ReadData())
            {
                var values = new List<object>();
                for (int i = 0; i < dt.Columns.Count; i++)
                    values.Add(item.GetValue(i));
                dt.Rows.Add(values.ToArray());
            }

            FinishRead();
            return dt;
        }
    }

    public abstract class DataType<SourceType, Data, Field> : DataType
        where SourceType : DataSource<SourceType, Data, Field>
        where Data : DataType<SourceType, Data, Field>
        where Field : DataField
    {
        protected DataType(SourceType source)
        {
            Source = source;
        }

        public override DataSource SourceBase { get { return Source; } }
        public SourceType Source { get; protected set; }

        protected abstract IList<Field> RetrieveFields();
        private IList<Field> Fields;
        public IList<Field> GetFields()
        {
            if (Fields == null)
                Fields = RetrieveFields();
            return Fields;
        }

        public override IEnumerator<DataField> GetEnumerator()
        {
            return GetFields().GetEnumerator();
        }

        public override void SourceChanged()
        {
            Fields = null; // let these be reloaded
        }

        public override IEnumerable<DataItem> ReadData()
        {
            while (HasMoreData())
                yield return ReadNext();
        }

        protected abstract DataItem ReadNext();
        public abstract bool HasMoreData();
    }


    public class DataItem
    {
        public DataItem(int fieldCount)
        {
            FieldCount = fieldCount;
            Values = new SortedList<DataField, object>(FieldCount);
        }

        public int FieldCount { get; private set; }
        public DataField GetField(int i) { return Values.Keys[i]; }
        
        public object GetValue(DataField field)
        {
            return Values[field];
        }

        public object GetValue(int i)
        {
            return Values.Values[i];
        }

        public void AddValue(DataField field, object value)
        {
            Values.Add(field, value);
        }

        SortedList<DataField, object> Values;
    }

    public abstract class DataField : IComparable<DataField>
    {
        public abstract string Name { get; protected set; }
        //Type Type { get; }

        public abstract int CompareTo(DataField other);
    }
}
