﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFarce.DataSources;
using System.IO;

namespace MapFarce
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        /*
        private void btnCreate_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";
            
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            FileInfo fi = new FileInfo(ofd.FileName);
            DataSourceCSV csvSource = new DataSourceCSV(fi);

            if (sourceControl == null)
            {
                sourceControl = new DataSourceControl();
                sourceControl.Location = new Point(12, 41);
                Controls.Add(sourceControl);
            }

            sourceControl.Populate(csvSource);

            source = csvSource;
        }

        DataSourceControl sourceControl;
        DataSource source;

        private void btnRead_Click(object sender, EventArgs e)
        {
            source.BeginRead();

            DataItem item = source.ReadNextItem();

            DataTable dt = new DataTable();

            List<IDataField> fields = new List<IDataField>();
            for (int i = 0; i < item.FieldCount; i++)
            {
                var field = item.GetField(i); 
                fields.Add(field);
                dt.Columns.Add(field.Name);
            }
            
            while (item != null)
            {
                var values = new List<object>();
                for ( int i=0; i<fields.Count; i++ )
                    values.Add(item.GetValue(i));
                dt.Rows.Add(values.ToArray());
                
                item = source.ReadNextItem();
            }

            source.FinishRead();

            dataGridView1.DataSource = dt;
        }
        */
    }
}