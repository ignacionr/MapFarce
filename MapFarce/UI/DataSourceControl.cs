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
    public partial class DataSourceControl : ProjectControl<DataSource, DataSourceControl>
    {
        public DataSourceControl()
        {
            InitializeComponent();
        }

        ContextMenu dataTypeRightClick;

        public override void Populate(DataSource element)
        {
            Element = element;
            element.ProjectControl = this;
            lblName.Text = element.Name;

            treeView.Nodes.Clear();

            dataTypeRightClick = new ContextMenu();
            dataTypeRightClick.MenuItems.Add(new MenuItem("&Edit data type", (o, e) => ShowDataTypeEdit()));

            if (element.DataMode == DataSource.Mode.Input)
            {
                dataTypeRightClick.MenuItems.Add(new MenuItem("&View data", (o, e) => MessageBox.Show("not implemented")));
            }
            else
            {

            }

            foreach (var type in element)
            {
                if (!type.IsEnabled)
                    continue;

                TreeNode typeNode = new TreeNode();
                typeNode.Tag = type;
                typeNode.Text = type.Name;

                PopulateTypeTree(type, typeNode);

                treeView.Nodes.Add(typeNode);
                typeNode.ContextMenu = dataTypeRightClick;
            }

            treeView.ExpandAll();
        }

        public static void PopulateTypeTree(DataType type, TreeNode typeNode)
        {
            typeNode.Nodes.Clear();

            foreach (var field in type)
            {
                TreeNode fieldNode = new TreeNode();
                fieldNode.Tag = field;
                fieldNode.Text = field.DisplayName;
                typeNode.Nodes.Add(fieldNode);
            }

        }

        private void ShowDataTypeEdit()
        {
            var node = treeView.SelectedNode;
            if (node == null)
                return;

            if (!(node.Tag is DataType))
                return;

            var type = node.Tag as DataType;
            var popup = new DataTypeEditPopup();

            popup.Populate(Element, node, type);
            popup.ShowDialog();
        }

        public void Repopulate()
        {
            Element.PropertiesChanged();
            Populate(Element);
        }

        private void lnkEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Element == null)
                return;

            var editor = new DataSourceEditPopup();
            editor.Populate(this);
            editor.ShowDialog();
        }
    }
}
