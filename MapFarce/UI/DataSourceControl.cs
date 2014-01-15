using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFarce.DataModel;

namespace MapFarce.UI
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

            ProjectPanel project = Parent as ProjectPanel;

            foreach (var type in source)
            {
                if (!type.IsEnabled)
                    continue;

                TreeNode typeNode = new TreeNode();
                typeNode.Tag = type;
                typeNode.Text = type.Name;

                foreach (var field in type)
                {
                    TreeNode fieldNode = new TreeNode();
                    fieldNode.Tag = field;
                    fieldNode.Text = field.DisplayName;
                    typeNode.Nodes.Add(fieldNode);
                }

                treeView.Nodes.Add(typeNode);
                if (source.DataMode == DataSource.Mode.Input)
                    typeNode.ContextMenu = project.inputDataTypeRightClick;
                else if (source.DataMode == DataSource.Mode.Output)
                    typeNode.ContextMenu = project.outputDataTypeRightClick;
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

            var editor = new DataSourceEditPopup();
            editor.Populate(this);
            editor.ShowDialog();
        }
    }
}
