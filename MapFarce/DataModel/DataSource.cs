using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MapFarce.DataModel
{
    public abstract class DataSource : IEnumerable<DataType>
    {
        public abstract string Name { get; }
        public Mode DataMode { get; set; }
        public abstract bool InitializeNew();

        public abstract bool CanAddDataTypes { get; }
        public abstract int NumDataTypes { get; }

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
        private void EnsureDataTypes()
        {
            if (DataTypes == null)
                DataTypes = RetrieveDataTypes();
        }

        protected abstract IList<Data> RetrieveDataTypes();
        protected IList<Data> DataTypes;

        public override int NumDataTypes
        {
            get
            {
                EnsureDataTypes();
                return DataTypes.Count;
            }
        }

        public override IEnumerator<DataType> GetEnumerator()
        {
            EnsureDataTypes();
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
            EnsureDataTypes();
        }
    }
}
