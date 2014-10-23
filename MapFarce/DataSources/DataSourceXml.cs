namespace MapFarce.DataSources
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MapFarce.DataModel;
    using MapFarce.EditProperties;

    [DataSourceDescriptor("Xml")]
    public class DataSourceXml: DataSource<DataSourceXml, DataSourceXml.DataTypeXml>
    {
        [UIEditablePropertyAttribute(null, "The file to read from", "File")]
        public FileInfo File
        {
            get { return file; }
            set
            {
                file = value;
                Name = file == null ? "XML Source" : file.Name;
            }
        }
        private FileInfo file;

        public class DataTypeXml : DataType<DataSourceXml, DataTypeXml>
        {
            protected override List<DataField> RetrieveFields()
            {
                throw new NotImplementedException();
            }

            protected override DataItem ReadNext()
            {
                throw new NotImplementedException();
            }

            public override bool HasMoreData()
            {
                throw new NotImplementedException();
            }

            public override string Name
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override DataField CreateField()
            {
                throw new NotImplementedException();
            }
        }

        protected override IList<DataSourceXml.DataTypeXml> RetrieveDataTypes()
        {
            throw new NotImplementedException();
        }

        public override bool CanAddDataTypes
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanEditTypeFields
        {
            get { throw new NotImplementedException(); }
        }

        public override bool InitializeNew()
        {
            throw new NotImplementedException();
        }
    }
}
