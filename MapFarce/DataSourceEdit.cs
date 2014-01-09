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

        private DataSource source;
        public void Populate(DataSource source)
        {
            this.source = source;

            var groups = new SortedList<string, GroupBox>();

            foreach (var prop in source.GetType().GetProperties())
            {
                var attributes = prop.GetCustomAttributes(typeof(UIEditableProperty), false);
                if (attributes.Length == 0)
                    continue;

                UIEditableProperty attrib = attributes[0] as UIEditableProperty;
                
                GroupBox group;
                if (!groups.TryGetValue(attrib.Group, out group))
                {
                    group = CreateGroupBox(attrib.Group);
                    groups.Add(attrib.Group, group);
                }

                AddEditControl(group, prop, attrib);
            }
        }

        private GroupBox CreateGroupBox(string name)
        {
            GroupBox gb = new GroupBox();
            gb.Margin = new Padding(0);
            gb.Width = Width - 8;
            gb.Height = 10;
            gb.Text = name;

            flowLayoutPanel1.Controls.Add(gb);
            return gb;
        }

        private void AddEditControl(GroupBox group, PropertyInfo prop, UIEditableProperty attrib)
        {
            Control c;
            if (prop.PropertyType == typeof(FileInfo))
            {
                c = new FileEditProperty(prop.Name, attrib.Description);
            }
            else if (prop.PropertyType == typeof(bool))
            {
                c = new BoolEditProperty(prop.Name, attrib.Description);
            }
            else if (prop.PropertyType == typeof(char))
            {
                c = new CharEditProperty(prop.Name, attrib.Description);
            }
            else if (prop.PropertyType == typeof(string))
            {
                c = new StringEditProperty(prop.Name, attrib.Description);
            }
            else
                throw new NotImplementedException("Not configured to edit " + prop.PropertyType.Name + " properties!");

            c.Left = 0;
            c.Top = group.Controls.Count * 48;
            group.Controls.Add(c);
            group.Height += c.Height;
        }

        public void Save()
        {

            source = null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Save();
            Close();
        }
    }
}
