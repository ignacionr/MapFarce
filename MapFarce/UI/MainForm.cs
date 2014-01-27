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
            Project.Instance = new Project();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckCloseExisting())
                return;

            Project.Instance = new Project();
            projectPanel.Reset();
            projectPanel.Show();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Instance.Save();
        }

        private void saveProjectasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Instance.SaveAs();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckCloseExisting())
                return;

            if (Project.Open(projectPanel))
                projectPanel.Show();
        }

        private bool CheckCloseExisting()
        {
            if (Project.Instance.HasChanges)
            {
                var retVal = MessageBox.Show("Save project changes?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (retVal == DialogResult.Cancel)
                    return false;

                if (retVal == DialogResult.Yes && !Project.Instance.Save())
                    return false;
            }
            return true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (var kvp in DataSource.GetAllTypes())
            {
                ToolStripButton b = new ToolStripButton();
                b.Text = kvp.Key;
                b.Click += (s, a) => projectPanel.AddSource(DataSource.Mode.Input, kvp.Value);

                btnAddInput.DropDownItems.Add(b);


                b = new ToolStripButton();
                b.Text = kvp.Key;
                b.Click += (s, a) => projectPanel.AddSource(DataSource.Mode.Output, kvp.Value);

                btnAddOutput.DropDownItems.Add(b);
            }
        }

        private void btnAddMapping_Click(object sender, EventArgs e)
        {
            projectPanel.AddMapping();
        }

        private void btnTestRead_Click(object sender, EventArgs e)
        {
            projectPanel.TestRead();
        }
    }
}
