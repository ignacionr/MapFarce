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
    public partial class MappingCreatePopup : Form
    {
        public MappingCreatePopup()
        {
            InitializeComponent();
        }

        Mapping mapping;
        public void Populate(Mapping m)
        {
            mapping = m;

            foreach (DataSource source in Project.Instance.Sources)
                foreach ( DataType type in source )
                    if (source.DataMode == DataSource.Mode.Input)
                        chkListInput.Items.Add(type, false/*mapping.Inputs.Contains(type)*/);
                    else if (source.DataMode == DataSource.Mode.Output)
                        chkListOutput.Items.Add(type, false/*mapping.Inputs.Contains(type)*/);

            CheckOK();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Mapping.Connection connection = new Mapping.Connection(mapping);
            foreach ( DataType type in chkListInput.CheckedItems )
                connection.LinkTo(type);

            mapping.Inputs.Add(connection);

            connection = new Mapping.Connection(mapping);
            foreach (DataType type in chkListOutput.CheckedItems)
                connection.LinkTo(type);

            mapping.Outputs.Add(connection);

            mapping.Name = txtName.Text;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void chkListInput_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckOK(e.NewValue == CheckState.Checked ? 1 : -1, 0);
        }

        private void chkListOutput_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckOK(0, e.NewValue == CheckState.Checked ? 1 : -1);
        }

        private void CheckOK(int inputDiff = 0, int outputDiff = 0)
        {
            btnOK.Enabled = chkListInput.CheckedItems.Count + inputDiff > 0
                       && chkListOutput.CheckedItems.Count + outputDiff > 0;
        }
    }
}
