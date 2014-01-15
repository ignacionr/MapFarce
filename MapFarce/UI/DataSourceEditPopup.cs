using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using MapFarce.EditProperties;
using MapFarce.DataModel;

namespace MapFarce.UI
{
    public partial class DataSourceEditPopup : Form
    {
        public DataSourceEditPopup()
        {
            InitializeComponent();
        }

        private DataSourceControl source;
        public void Populate(DataSourceControl source)
        {
            this.source = source;

            var groups = new SortedList<string, GroupBox>();

            foreach (var prop in source.Source.GetType().GetProperties())
            {
                var attributes = prop.GetCustomAttributes(typeof(UIEditablePropertyAttribute), false);
                if (attributes.Length == 0)
                    continue;

                UIEditablePropertyAttribute attrib = attributes[0] as UIEditablePropertyAttribute;
                attrib.Property = prop;

                GroupBox group;
                if (!groups.TryGetValue(attrib.Group, out group))
                {
                    group = CreateGroupBox(attrib.Group);
                    groups.Add(attrib.Group, group);
                }

                AddEditControl(group, attrib);
            }

            if (ShouldShowDataTypeTable())
            {
                GroupBox group = CreateGroupBox("Data Types");
                AddDataTypeTable(group);
            }
        }

        private bool ShouldShowDataTypeTable()
        {
            return source.Source.CanAddDataTypes || source.Source.NumDataTypes > 1;
        }

        CheckedListBox lstDataTypes = null;
        private void AddDataTypeTable(GroupBox group)
        {
            lstDataTypes = new CheckedListBox();
            group.Controls.Add(lstDataTypes);
            lstDataTypes.Location = new Point(120, 10);
            lstDataTypes.Size = new Size(185, 80);
            
            foreach (var type in source.Source)
                lstDataTypes.Items.Add(type, type.IsEnabled);

            if (lstDataTypes.Items.Count > 0)
                lstDataTypes.SelectedIndex = 0;

            if (source.Source.DataMode == DataSource.Mode.Output && source.Source.CanAddDataTypes)
            {
                LinkLabel lnkAddType = new LinkLabel();
                lnkAddType.Text = "Add Data Type";
                group.Controls.Add(lnkAddType);
                lnkAddType.Location = new Point(4, 32);
            }

            group.Height = 96;
        }

        static Padding groupBoxMargin = new Padding(4, 4, 4, 0);
        static Padding groupBoxPadding = new Padding(2, 12, 2, 4);
        private GroupBox CreateGroupBox(string name)
        {
            GroupBox gb = new GroupBox();
            gb.Margin = groupBoxMargin;
            gb.Padding = groupBoxPadding;

            gb.Width = Width - groupBoxPadding.Horizontal - groupBoxMargin.Horizontal - 2;
            gb.Height = groupBoxPadding.Vertical;
            gb.Text = name;

            flowLayoutPanel1.Controls.Add(gb);
            return gb;
        }

        private void AddEditControl(GroupBox group, UIEditablePropertyAttribute attrib)
        {
            EditPropertyBase c;
            if (attrib.Property.PropertyType == typeof(FileInfo))
                c = new FileEditProperty(attrib.Label);
            else if (attrib.Property.PropertyType == typeof(bool))
                c = new BoolEditProperty(attrib.Label);
            else if (attrib.Property.PropertyType == typeof(char))
                c = new CharEditProperty(attrib.Label);
            else if (attrib.Property.PropertyType == typeof(string))
                c = new StringEditProperty(attrib.Label);
            else
                throw new NotImplementedException("Not configured to edit " + attrib.Property.PropertyType.Name + " properties!");

            c.SetValue(attrib.Property.GetValue(source.Source, null));
            c.SetToolTip(toolTip1, attrib.Tooltip);

            c.Tag = attrib;
            c.Left = groupBoxPadding.Left;
            c.Width = group.Width - groupBoxPadding.Horizontal;
            group.Height += c.Height;
            group.Controls.Add(c);
            c.Top = group.Height - groupBoxPadding.Bottom - c.Height;
        }

        public void Save()
        {
            foreach (GroupBox group in flowLayoutPanel1.Controls)
                foreach (EditPropertyBase control in group.Controls)
                {
                    var attribute = control.Tag as UIEditablePropertyAttribute;
                    attribute.Property.SetValue(source.Source, control.GetValue(), null);
                }

            if (lstDataTypes != null)
            {
                // only the checked types should be enabled
                foreach (DataType type in lstDataTypes.Items)
                    type.IsEnabled = false;
                foreach (DataType type in lstDataTypes.CheckedItems)
                    type.IsEnabled = true;
            }
            source.Repopulate();
            source = null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Save();
            Close();
        }
    }
}
