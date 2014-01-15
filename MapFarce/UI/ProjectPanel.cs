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
using MapFarce.DataModel;

namespace MapFarce.UI
{
    public partial class ProjectPanel : UserControl
    {
        public ProjectPanel()
        {
            InitializeComponent();
            ControlAdded += (o, e) => { MakeDraggable(e.Control); HasChanges = true; };
        }

        public bool HasChanges { get; set; }
        
        public void Reset()
        {
            Controls.Clear();
            HasChanges = false;
        }

        private void MakeDraggable(Control c)
        {
            c.MouseDown += Draggable_MouseDown;
            c.MouseUp += Draggable_MouseUp;
            c.MouseMove += Draggable_MouseMove;
        }

        Control draggingControl;
        Point dragStartLocation;
        void Draggable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Control c = sender as Control;
            c.BringToFront();
            draggingControl = c;
            Cursor = Cursors.SizeAll;
            dragStartLocation = e.Location;
        }

        void Draggable_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            draggingControl = null;
            Cursor = Cursors.Default;
        }

        void Draggable_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingControl != sender)
                return;

            var location = draggingControl.Location;
            location.Offset(e.Location.X - dragStartLocation.X, e.Location.Y - dragStartLocation.Y);
            draggingControl.Location = location;
            HasChanges = true;
        }

        public void AddSource(DataSource.Mode mode, DataSourceDescriptorAttribute attrib, Type type)
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

        public void TestRead()
        {
            DataSource source = null;
            foreach ( Control c in Controls )
                if (c is DataSourceControl)
                {
                    source = (c as DataSourceControl).Source;
                    break;
                }

            if (source == null)
                return;

            DataSet ds = source.ReadDataSet();
            DataGridView grid = new DataGridView();
            grid.DataSource = ds.Tables[0];
            grid.Location = new Point(16, 48);
            Controls.Add(grid);
        }
    }
}
