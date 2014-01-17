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
    public partial class DataTypeEditPopup : Form
    {
        public DataTypeEditPopup()
        {
            InitializeComponent();

            colFieldType.DisplayMember = "Name";
            foreach (var fieldType in FieldType.AllTypes)
                colFieldType.Items.Add(fieldType);
        }

        private TreeNode node;
        private DataType type;
        public void Populate(DataSource source, TreeNode node, DataType type)
        {
            this.node = node;
            this.type = type;

            bool fullEdit = source.DataMode == DataSource.Mode.Output && source.CanEditTypeFields;
            lnkMoveUp.Visible = lnkMoveDown.Visible = fieldGrid.AllowUserToAddRows = fieldGrid.AllowUserToDeleteRows = fullEdit;
            colFieldName.ReadOnly = colFieldType.ReadOnly = !fullEdit;

            foreach (var field in type)
            {
                int index = fieldGrid.Rows.Add();
                var row = fieldGrid.Rows[index];
                row.Tag = field;

                row.Cells[0].Value = field.Name;
                row.Cells[1].Value = field.DisplayName == field.Name ? string.Empty : field.DisplayName;

                var cell = row.Cells[2];
                cell.Value = field.Type.Name;
            }

            fieldGrid.CellValueChanged += fieldGrid_CellValueChanged;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplyChanges();

            type.SortFields();
            DataSourceControl.PopulateTypeTree(type, node);
            Close();
        }

        private void ApplyChanges()
        {
            foreach (var change in FieldChanges)
            {
                if (change.Delete)
                {
                    type.RemoveField(change.Field);
                    continue;
                }

                if (change.IsNew)
                {
                    change.Field.FieldNumber = type.FieldCount;
                    type.AddField(change.Field);
                }

                if (change.NewName != null)
                {
                    change.Field.Name = change.NewName;
                    if (string.IsNullOrWhiteSpace(change.Field.DisplayName))
                        change.Field.DisplayName = change.NewName;
                }

                if (change.NewDisplayName != null) // if NewDisplayName = blank, use Name instead.
                    change.Field.DisplayName = string.IsNullOrWhiteSpace(change.NewDisplayName) ? change.Field.Name : change.NewDisplayName;

                if (change.NewType.HasValue)
                    change.Field.Type = change.NewType.Value;

                if (change.PositionOffset != 0)
                {
                    DataField swapWith = type.GetField(change.Field.FieldNumber + change.PositionOffset);
                    change.Field.FieldNumber += change.PositionOffset;
                    swapWith.FieldNumber -= change.PositionOffset;
                }
            }
        }

        private List<DataGridViewRow> GetSelectedRows()
        {
            var list = new List<DataGridViewRow>();

            var selectedRows = fieldGrid.SelectedRows;
            if (selectedRows.Count > 0)
            {
                for (int i = 0; i < selectedRows.Count; i++)
                    list.Add(selectedRows[i]);
            }
            else
            {
                var selectedCells = fieldGrid.SelectedCells;
                for (int i = 0; i < selectedCells.Count; i++)
                {
                    var cell = fieldGrid.SelectedCells[i];

                    // ignore cells from column 2 (unless we only have one cell), cos the dropdowns don't highlight to show the selection
                    if (cell.ColumnIndex == 2 && selectedCells.Count != 1)
                        continue;

                    if (!list.Contains(cell.OwningRow))
                        list.Add(cell.OwningRow);
                }
            }

            list.Sort((r1, r2) => r1.Index.CompareTo(r2.Index));
            return list;
        }

        private void fieldGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = fieldGrid.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            string val = cell.Value == null ? string.Empty : cell.Value.ToString();

            var change = new FieldChange();
            change.Field = row.Tag as DataField;

            if (e.ColumnIndex == 0)
                change.NewName = val;
            else if (e.ColumnIndex == 1)
                change.NewDisplayName = val;
            else if (e.ColumnIndex == 2)
                change.NewType = FieldType.GetByName(val);

            FieldChanges.Add(change);
        }

        private void fieldGrid_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            var change = new FieldChange();
            change.IsNew = true;
            fieldGrid.Rows[fieldGrid.Rows.Count - 2].Tag = change.Field = type.CreateField();
            FieldChanges.Add(change);
        }

        private void fieldGrid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            var change = new FieldChange();
            change.Field = e.Row.Tag as DataField;
            change.Delete = true;
            FieldChanges.Add(change);
        }

        private void lnkMoveUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            List<int> selectedIndices = new List<int>();

            var selectedRows = GetSelectedRows();
            for ( int i=0; i<selectedRows.Count; i++ )
            {
                var row = selectedRows[i];

                int pos = row.Index;
                if (pos == 0 || pos == fieldGrid.Rows.Count - 1)
                    continue; // can't move up the first row, or the very last row which is for entering new rows, and isn't a proper row in itself

                if (fieldGrid.Rows[pos - 1].Selected)
                    continue; // don't move through another selected row

                var change = new FieldChange();
                change.Field = row.Tag as DataField;
                change.PositionOffset = -1;
                FieldChanges.Add(change);

                fieldGrid.Rows.RemoveAt(pos);
                fieldGrid.Rows.Insert(pos - 1, row);
                selectedIndices.Add(pos - 1);
            }

            fieldGrid.ClearSelection();
            foreach (int index in selectedIndices)
                fieldGrid.Rows[index].Selected = true;
        }

        private void lnkMoveDown_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            List<int> selectedIndices = new List<int>();
            
            var selectedRows = GetSelectedRows();
            for (int i = selectedRows.Count-1; i >= 0; i--)
            {
                var row = selectedRows[i];

                int pos = row.Index;
                if (pos >= fieldGrid.Rows.Count-2)
                    continue; // can't move down the last actual row, or the "new" row displayed below that, which isn't a proper row in itself

                if (fieldGrid.Rows[pos + 1].Selected)
                    continue; // don't move through another selected row

                var change = new FieldChange();
                change.Field = row.Tag as DataField;
                change.PositionOffset = 1;
                FieldChanges.Add(change);

                fieldGrid.Rows.RemoveAt(pos);
                fieldGrid.Rows.Insert(pos + 1, row);
                selectedIndices.Add(pos + 1);
            }

            fieldGrid.ClearSelection();
            foreach (int index in selectedIndices)
                fieldGrid.Rows[index].Selected = true;
        }

        List<FieldChange> FieldChanges = new List<FieldChange>();

        class FieldChange
        {
            public DataField Field { get; set; }
            public string NewName { get; set; }
            public string NewDisplayName { get; set; }
            public FieldType? NewType { get; set; }
            public int PositionOffset { get; set; }

            public bool IsNew { get; set; }
            public bool Delete { get; set; }
        }
    }
}
