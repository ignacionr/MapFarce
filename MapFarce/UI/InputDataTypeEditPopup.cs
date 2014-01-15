using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFarce.DataModel;

namespace MapFarce.UI
{
    public partial class InputDataTypeEditPopup : Form
    {
        public InputDataTypeEditPopup()
        {
            InitializeComponent();

            colFieldType.DisplayMember = "Name";
            foreach (var fieldType in FieldType.AllTypes)
                colFieldType.Items.Add(fieldType);
        }

        private DataSourceControl source;
        private DataType type;
        public void Populate(DataSourceControl source, DataType type)
        {
            this.source = source;
            this.type = type;

            fieldGrid.AllowUserToAddRows = fieldGrid.AllowUserToDeleteRows = false;
            colFieldType.ReadOnly = true; // want something like source.Source.CanEditInputTypes here

            //fieldGrid.AllowUserToAddRows = fieldGrid.AllowUserToDeleteRows = source.Source.CanAddDataTypes;
            //colFieldType.ReadOnly = false;

            foreach (var field in type)
            {
                int index = fieldGrid.Rows.Add();
                var row = fieldGrid.Rows[index];

                row.Cells[0].Value = field.DisplayName;
                row.Cells[1].Value = field.Type;
            }
        }

        class FieldChange
        {
            DataType Type { get; set; }
            DataField Existing { get; set; }

            public string NewName { get; set; }
            public DataType NewType { get; set; }
            public int NewPosition { get; set; }
        }

        List<FieldChange> FieldChanges = new List<FieldChange>();
    }
}
