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
            ControlAdded += (o, e) => { MakeDraggable(e.Control); };
        }
        
        public void Reset()
        {
            Controls.Clear();

            foreach (var source in Project.Instance.Sources)
            {

            }
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

            var tag = draggingControl.Tag;
            if (tag != null && tag is ProjectElement)
            {
                var element = tag as ProjectElement;
                element.Location = location;
                element.HasChanges = true;
            }
        }

        public void AddSource(DataSource.Mode mode, DataSourceDescriptorAttribute attrib, Type type)
        {
            var source = Activator.CreateInstance(type) as DataSource;
            source.DataMode = mode;
            if (!source.InitializeNew()) // but these might not be NEW sources. We might be loading a file!
                return;

            Project.Instance.AddSource(source);
            AddControlFor(source);
        }

        private void AddControlFor(DataSource source)
        {
            var sourceControl = new DataSourceControl();
            source.Location = sourceControl.Location = PlaceNewControl(sourceControl.Size);
            sourceControl.Tag = source;
            Controls.Add(sourceControl);
            sourceControl.Populate(source);
        }

        public void AddMapping()
        {
            Mapping mapping = new Mapping();
            Project.Instance.AddMapping(mapping);
            AddControlFor(mapping);
        }

        private void AddControlFor(Mapping mapping)
        {
            MappingControl mappingControl = new MappingControl();
            mapping.Location = mappingControl.Location = PlaceNewControl(mappingControl.Size);
            mappingControl.Tag = mapping;
            Controls.Add(mappingControl);
            mappingControl.Populate(mapping);
        }

        private Point PlaceNewControl(Size size)
        {
            return new Point(40, 40);
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

        public ContextMenu inputDataTypeRightClick, outputDataTypeRightClick;
    }
}
