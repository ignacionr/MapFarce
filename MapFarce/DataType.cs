using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MapFarce
{
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
}
