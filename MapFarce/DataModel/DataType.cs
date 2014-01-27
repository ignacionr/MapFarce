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

        const string typeNodeName = "Type", nameAttribName = "name";
        public override XmlNode CreateXmlNode(XmlNode parent)
        {
            var node = parent.OwnerDocument.CreateElement(typeNodeName);
            parent.AppendChild(node);

            var attrib = node.OwnerDocument.CreateAttribute(nameAttribName);
            attrib.Value = Name;
            node.Attributes.Append(attrib);

            SaveToXml(node);
            return node;
        }

        protected override void SaveToXml(XmlNode node)
        {
            foreach (var field in this)
            {
                var fieldNode = field.CreateXmlNode(node);
                node.AppendChild(fieldNode);
            }
        }

        public static DataType LoadFromXml(XmlNode node, DataSource source)
        {
            var type = source.GetDataTypeType();

            DataType dt = (DataType)Activator.CreateInstance(type);

            var attrib = node.Attributes[nameAttribName];
            if (attrib == null || attrib.Value.Trim() == string.Empty)
                throw new FormatException();

            dt.Name = attrib.Value;

            dt.PopulateFromXml(node);
            return dt;
        }

        public override void PopulateFromXml(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if (child.Name == DataField.fieldNodeName)
                    AddField(DataField.LoadFromXml(child));
        }

        public abstract DataSource SourceBase { get; }
        public abstract string Name { get; set; }

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

    public abstract class DataType<SourceType, Data> : DataType
        where SourceType : DataSource<SourceType, Data>
        where Data : DataType<SourceType, Data>, new()
    {
        public override DataSource SourceBase { get { return Source; } }
        public SourceType Source { get; set; }

        protected abstract List<DataField> RetrieveFields();
        private List<DataField> Fields;
        public IList<DataField> GetFields()
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
            if (Fields == null)
                Fields = new List<DataField>();
            Fields.Add(field as DataField);
            field.FieldNumber = Fields.Count;
        }

        public override void RemoveField(DataField field)
        {
            Fields.Remove(field as DataField);

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
