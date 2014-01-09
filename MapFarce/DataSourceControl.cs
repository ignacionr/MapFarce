using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFarce
{
    public partial class DataSourceControl : UserControl
    {
        public DataSourceControl()
        {
            InitializeComponent();
        }

        private DataSource Source;

        public void Populate<DataType, DataField>(DataSource<DataType, DataField> source)
            where DataType : DataType<DataField>
            where DataField : IDataField
        {
            Source = source;
            lblName.Text = source.Name;

            treeView.Nodes.Clear();

            foreach (var type in source.GetDataTypes())
            {
                TreeNode typeNode = new TreeNode();
                typeNode.Text = type.Name;

                foreach (var field in type.GetFields())
                {
                    TreeNode fieldNode = new TreeNode();
                    fieldNode.Text = field.Name;
                    typeNode.Nodes.Add(fieldNode);
                }

                treeView.Nodes.Add(typeNode);
            }

            treeView.ExpandAll();
        }

        private void lnkEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Source == null)
                return;

            var editor = new DataSourceEdit();
            editor.Populate(Source);
            editor.ShowDialog();
        }
    }
}
