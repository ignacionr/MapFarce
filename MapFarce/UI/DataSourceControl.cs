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

        ContextMenu dataTypeRightClick;

        public DataSource Source { get; private set; }

        public void Populate(DataSource source)
        {
            Source = source;
            lblName.Text = source.Name;

            treeView.Nodes.Clear();

            dataTypeRightClick = new ContextMenu();
            if (source.DataMode == DataSource.Mode.Input)
            {
                dataTypeRightClick.MenuItems.Add(new MenuItem("&Edit data type", (o, e) => ShowInputDataTypeEdit()));
                dataTypeRightClick.MenuItems.Add(new MenuItem("&View data", (o, e) => MessageBox.Show("not implemented")));
            }
            else
            {
                dataTypeRightClick.MenuItems.Add(new MenuItem("&Edit data type", (o, e) => MessageBox.Show("not implemented")));
            }

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
                typeNode.ContextMenu = dataTypeRightClick;
            }

            treeView.ExpandAll();
        }

        private void ShowInputDataTypeEdit()
        {
            var node = treeView.SelectedNode;
            if (node == null)
                return;

            if (!(node.Tag is DataType))
                return;

            var type = node.Tag as DataType;
            var popup = new InputDataTypeEditPopup();

            popup.Populate(node, type);
            popup.ShowDialog();
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
