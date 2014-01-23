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
    public partial class MappingConnectionPopup : Form
    {
        public MappingConnectionPopup()
        {
            InitializeComponent();
        }

        Mapping mapping;
        Mapping.Connection connection;
        public void Populate(Mapping m, Mapping.Connection c, DataSource.Mode mode)
        {
            mapping = m;
            connection = c;

            if (mode == DataSource.Mode.Output)
                lblDataTypes.Text = "Outputs";

            foreach (DataSource source in Project.Instance.Sources)
                foreach (DataType type in source)
                    if (source.DataMode == mode)
                        chkListDataTypes.Items.Add(type, c.DataTypes.Contains(type));

            CheckOK();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (DataType type in chkListDataTypes.CheckedItems)
                connection.DataTypes.Add(type);

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void chkListDataTypes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckOK(e.NewValue == CheckState.Checked ? 1 : -1, 0);
        }

        private void CheckOK(int inputDiff = 0, int outputDiff = 0)
        {
            btnOK.Enabled = chkListDataTypes.CheckedItems.Count + inputDiff > 0;
        }
    }
}
