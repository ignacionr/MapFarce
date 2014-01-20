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
    public partial class MappingEditPopup : Form
    {
        public MappingEditPopup()
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
                        chkListInput.Items.Add(type, mapping.Inputs.Contains(type));
                    else if (source.DataMode == DataSource.Mode.Output)
                        chkListOutput.Items.Add(type, mapping.Inputs.Contains(type));

            CheckOK();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            mapping.Inputs.Clear();
            foreach ( DataType type in chkListInput.CheckedItems )
                mapping.Inputs.Add(type);

            mapping.Outputs.Clear();
            foreach (DataType type in chkListOutput.CheckedItems)
                mapping.Outputs.Add(type);

            mapping.SetName(txtName.Text);

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
