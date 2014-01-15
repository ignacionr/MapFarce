using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFarce.DataSources;
using System.IO;
using MapFarce.DataModel;

namespace MapFarce.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryCloseProject();
        }

        private void TryCloseProject()
        {
            if (projectPanel.HasChanges)
            {
                var retVal = MessageBox.Show("Save project changes?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (retVal == DialogResult.Cancel)
                    return;

                if (retVal == DialogResult.Yes && !SaveProject())
                    return;
            }

            CloseProject();
        }

        private bool SaveProject()
        {
            return true;
        }

        private void CloseProject()
        {
            projectPanel.Reset();
            projectPanel.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
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
                b.Click += (s, a) => projectPanel.AddSource(DataSource.Mode.Input, attrib, type);

                btnAddInput.DropDownItems.Add(b);


                b = new ToolStripButton();
                b.Text = attrib.Name;
                b.Click += (s, a) => projectPanel.AddSource(DataSource.Mode.Output, attrib, type);

                btnAddOutput.DropDownItems.Add(b);
            }
        }

        private void btnTestRead_Click(object sender, EventArgs e)
        {
            projectPanel.TestRead();
        }
    }
}
