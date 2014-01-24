using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;

namespace MapFarce.DataModel
{
    public abstract class DataType : Savable, IEnumerable<DataField>
    {
        protected DataType()
        {
            IsEnabled = true;
        }

        public override XmlNode CreateXmlNode(XmlNode parent)
        {
            var node = parent.OwnerDocument.CreateElement("Type");
            parent.AppendChild(node);

            SaveToXml(node);
            return node;
        }

        protected override void SaveToXml(XmlNode node)
        {
            var attrib = node.OwnerDocument.CreateAttribute("name");
            attrib.Value = Name;
            node.Attributes.Append(attrib);

            foreach (var field in this)
            {
                var fieldNode = field.CreateXmlNode(node);
                node.AppendChild(fieldNode);
            }
        }

        public static DataType LoadFromXml(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public override void PopulateFromXml(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public abstract DataSource SourceBase { get; }
        public abstract string Name { get; }

        public bool IsEnabled { get; set; }

        public override string ToString() { return string.Format("{0} - {1}", SourceBase.Name, Name); }

        public abstract void SourceChanged();

        public abstract IEnumerator<DataField> GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public abstract IEnumerable<DataItem> ReadData();

        public abstract DataField CreateField();

        public abstract int FieldCount { get; }
        public abstract void AddField(DataField field);
        public abstract void RemoveField(DataField field);
        public abstract DataField GetField(int pos);
        public abstract void SortFields();

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

        protected abstract List<Field> RetrieveFields();
        private List<Field> Fields;
        public IList<Field> GetFields()
        {
            if (Fields == null)
            {
                Fields = RetrieveFields();

                for (int i = 0; i < Fields.Count; i++)
                    Fields[i].FieldNumber = i + 1;
            }
            return Fields;
        }

        public override IEnumerator<DataField> GetEnumerator()
        {
            return GetFields().GetEnumerator();
        }

        public override int FieldCount { get { return Fields.Count; } }

        public override void AddField(DataField field)
        {
            Fields.Add(field as Field);
            field.FieldNumber = Fields.Count;
        }

        public override void RemoveField(DataField field)
        {
            Fields.Remove(field as Field);

            foreach (var other in Fields)
                if (other.FieldNumber > field.FieldNumber)
                    other.FieldNumber--;
        }

        public override DataField GetField(int pos)
        {
            return Fields[pos - 1];
        }

        public override void SortFields()
        {
            Fields.Sort();
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
