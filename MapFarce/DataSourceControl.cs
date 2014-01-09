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

        public DataSource Source { get; private set; }

        public void Populate(DataSource source)
        {
            Source = source;
            lblName.Text = source.Name;

            treeView.Nodes.Clear();

            foreach (var type in source.GetDataTypesBase())
            {
                TreeNode typeNode = new TreeNode();
                typeNode.Text = type.Name;

                foreach (var field in type.GetFieldsBase())
                {
                    TreeNode fieldNode = new TreeNode();
                    fieldNode.Text = field.Name;
                    typeNode.Nodes.Add(fieldNode);
                }

                treeView.Nodes.Add(typeNode);
            }

            treeView.ExpandAll();
        }

        public void Repopulate()
        {
            Source.PropertiesChanged();
            Populate(Source);
        }

        private void lnkEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Source == null)
                return;

            var editor = new DataSourceEdit();
            editor.Populate(this);
            editor.ShowDialog();
        }
    }
}
