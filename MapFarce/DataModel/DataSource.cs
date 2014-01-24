using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MapFarce.UI;
using System.Xml;
using MapFarce.EditProperties;

namespace MapFarce.DataModel
{
    public abstract class DataSource : ProjectElement<DataSource, DataSourceControl>, IEnumerable<DataType>
    {
        public Mode DataMode { get; set; }
        
        public abstract bool CanAddDataTypes { get; }
        public abstract bool CanEditTypeFields { get; }

        public abstract int NumDataTypes { get; }

        private static SortedList<string, Type> sourceTypesByName;
        public static SortedList<string, Type> GetAllTypes()
        {
            if ( sourceTypesByName == null )
            {
                var types = typeof(DataSources.DataSourceCSV).Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(DataSource)))
                    .OrderBy(t => t.Name);

                sourceTypesByName = new SortedList<string,Type>();
                foreach (var type in types)
                {
                    string descriptor = DataSourceDescriptorAttribute.GetName(type);
                    if (descriptor != null)
                        sourceTypesByName[descriptor] = type;
                }
            }
        
            return sourceTypesByName;
        }

        const string nodeName = "Source", modeAttribName = "mode", typeAttribName = "type", nameAttribName = "name";

        public override XmlNode CreateXmlNode(XmlNode parent)
        {
            var node = parent.OwnerDocument.CreateElement(nodeName);

            var attrib = node.OwnerDocument.CreateAttribute(modeAttribName);
            attrib.Value = DataMode.ToString();
            node.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute(typeAttribName);
            attrib.Value = DataSourceDescriptorAttribute.GetName(GetType()) ?? "unknown";
            node.Attributes.Append(attrib);

            attrib = node.OwnerDocument.CreateAttribute(nameAttribName);
            attrib.Value = Name;
            node.Attributes.Append(attrib);

            parent.AppendChild(node);

            SaveBounds(node);

            SaveToXml(node);
            return node;
        }

        protected override void SaveToXml(XmlNode node)
        {
            foreach (var prop in UIEditablePropertyAttribute.GetAll(GetType()))
            {
                object val = prop.Property.GetValue(this, null);
                if (val == null)
                    continue;

                var child = node.OwnerDocument.CreateElement(prop.Property.Name);
                node.AppendChild(child);
                child.InnerText = prop.GetStringFromValue(val);
            }
        }
        
        public static DataSource LoadFromXml(XmlNode node)
        {
            var attrib = node.Attributes[typeAttribName];
            var types = DataSource.GetAllTypes();

            if (attrib == null || !types.ContainsKey(attrib.Value))
                throw new FormatException();

            DataSource source = (DataSource)Activator.CreateInstance(types[attrib.Value]);

            attrib = node.Attributes[nameAttribName];
            if ( attrib == null || attrib.Value.Trim() == string.Empty )
                throw new FormatException();

            source.Name = attrib.Value;

            attrib = node.Attributes[modeAttribName];
            if ( attrib == null )
                throw new FormatException();
            
            if ( attrib.Value == Mode.Input.ToString() )
                source.DataMode = Mode.Input;
            else if (attrib.Value == Mode.Output.ToString())
                source.DataMode = Mode.Output;

            source.PopulateFromXml(node);
            return source;
        }

        public override void PopulateFromXml(XmlNode node)
        {
            foreach (var prop in UIEditablePropertyAttribute.GetAll(GetType()))
            {
                var child = node[prop.Property.Name];
                if (child == null)
                    continue;

                prop.SetValueFromString(this, child.InnerText);
            }
        }

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
