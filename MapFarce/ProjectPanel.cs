using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using MapFarce.DataSources;

namespace MapFarce
{
    public partial class ProjectPanel : UserControl
    {
        public ProjectPanel()
        {
            InitializeComponent();
        }

        private void ProjectPanel_Load(object sender, EventArgs e)
        {
            var types = typeof(DataSources.DataSourceCSV).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DataSource)))
                .OrderBy(type => type.Name);

            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(typeof(DataSourceDescriptorAttribute), false);
                if (attributes.Length == 0)
                    continue;

                DataSourceDescriptorAttribute attrib = attributes[0] as DataSourceDescriptorAttribute;
                
                ToolStripButton b = new ToolStripButton();
                b.Text = attrib.Name;
                b.Click += (s, a) => AddSource(DataSource.Mode.Input, attrib, type);

                btnAddInput.DropDownItems.Add(b);


                b = new ToolStripButton();
                b.Text = attrib.Name;
                b.Click += (s, a) => AddSource(DataSource.Mode.Output, attrib, type);

                btnAddOutput.DropDownItems.Add(b);
            }
        }

        private void AddSource(DataSource.Mode mode, DataSourceDescriptorAttribute attrib, Type type)
        {
            var source = Activator.CreateInstance(type) as DataSource;
            source.DataMode = mode;
            if (!source.InitializeNew())
                return;
            
            var sourceControl = new DataSourceControl();
            sourceControl.Location = new Point(40, 40);
            Controls.Add(sourceControl);
            sourceControl.Populate(source);
        }
    }
}
