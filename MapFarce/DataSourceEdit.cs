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

namespace MapFarce
{
    public partial class DataSourceEdit : Form
    {
        public DataSourceEdit()
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
                var attributes = prop.GetCustomAttributes(typeof(UIEditableProperty), false);
                if (attributes.Length == 0)
                    continue;

                UIEditableProperty attrib = attributes[0] as UIEditableProperty;
                attrib.Property = prop;

                GroupBox group;
                if (!groups.TryGetValue(attrib.Group, out group))
                {
                    group = CreateGroupBox(attrib.Group);
                    groups.Add(attrib.Group, group);
                }

                AddEditControl(group, attrib);
            }
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

        private void AddEditControl(GroupBox group, UIEditableProperty attrib)
        {
            EditPropertyBase c;
            if (attrib.Property.PropertyType == typeof(FileInfo))
                c = new FileEditProperty(attrib.Property.Name);
            else if (attrib.Property.PropertyType == typeof(bool))
                c = new BoolEditProperty(attrib.Property.Name);
            else if (attrib.Property.PropertyType == typeof(char))
                c = new CharEditProperty(attrib.Property.Name);
            else if (attrib.Property.PropertyType == typeof(string))
                c = new StringEditProperty(attrib.Property.Name);
            else
                throw new NotImplementedException("Not configured to edit " + attrib.Property.PropertyType.Name + " properties!");

            c.SetValue(attrib.Property.GetValue(source.Source, null));
            c.SetToolTip(toolTip1, attrib.Description);

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
                    var attribute = control.Tag as UIEditableProperty;
                    attribute.Property.SetValue(source.Source, control.GetValue(), null);
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
