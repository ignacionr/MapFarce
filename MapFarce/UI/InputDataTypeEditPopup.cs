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

        private TreeNode node;
        private DataType type;
        public void Populate(TreeNode node, DataType type)
        {
            this.node = node;
            this.type = type;

            fieldGrid.AllowUserToAddRows = fieldGrid.AllowUserToDeleteRows = false;
            colFieldType.ReadOnly = true; // want something like source.Source.CanEditInputTypes here

            //fieldGrid.AllowUserToAddRows = fieldGrid.AllowUserToDeleteRows = source.Source.CanAddDataTypes;
            //colFieldType.ReadOnly = false;

            foreach (var field in type)
            {
                int index = fieldGrid.Rows.Add();
                var row = fieldGrid.Rows[index];
                row.Tag = field;

                row.Cells[0].Value = field.Name;
                row.Cells[1].Value = field.DisplayName;
                row.Cells[2].Value = field.Type;
            }

            fieldGrid.CellValueChanged += fieldGrid_CellValueChanged;
        }

        SortedList<DataField, string> FieldChanges = new SortedList<DataField, string>();

        private void fieldGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = fieldGrid.Rows[e.RowIndex];
            var val = row.Cells[e.ColumnIndex].Value.ToString();

            FieldChanges[row.Tag as DataField] = val;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (var kvp in FieldChanges)
            {
                kvp.Key.DisplayName = kvp.Value;

                for ( int i=0; i<node.Nodes.Count; i++ )
                {
                    var fieldNode = node.Nodes[i];
                    if ( fieldNode.Tag == kvp.Key )
                        fieldNode.Text = kvp.Value;
                }
            }
            Close();
        }
    }
}
