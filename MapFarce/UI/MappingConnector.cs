using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFarce.DataModel;
using System.Drawing;

namespace MapFarce.UI
{
    public class MappingConnector : UserControl
    {
        public MappingConnector(MappingControl mapping, DataSource.Mode mode)
        {
            Mapping = mapping;
            Mode = mode;

            if (Mode == DataSource.Mode.Input)
                Connections = Mapping.Element.Inputs;
            else if (Mode == DataSource.Mode.Output)
                Connections = Mapping.Element.Outputs;

            Recalculate();
        }

        public MappingControl Mapping { get; private set; }
        public DataSource.Mode Mode { get; private set; }
        public List<Mapping.Connection> Connections { get; set; }

        private List<Point> Points = new List<Point>();
        private Point MappingConnection;

        public void Recalculate()
        {
            Invalidate();
            Points.Clear();

            int height = 64;
            if (Connections.Count > 4)
                height = (int)(height + height * (Connections.Count - 4) * 0.1f);

            int width = (int)(height * 0.625f + 0.5f);
            Visible = Connections.Count > 1;

            var bounds = Mapping.Bounds;
            if (Mode == DataSource.Mode.Input)
            {
                MappingConnection = new Point(bounds.X, bounds.Y + bounds.Height / 2);
                Bounds = new Rectangle(MappingConnection.X - width, MappingConnection.Y - height / 2, width, height);
            }
            else if (Mode == DataSource.Mode.Output)
            {
                MappingConnection = new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height / 2);
                Bounds = new Rectangle(MappingConnection.X, MappingConnection.Y - height / 2, width, height);
            }
            MappingConnection.Offset(-Location.X, -Location.Y);

            if (Connections.Count <= 1)
                return;

            float maxAngle;
            switch (Connections.Count)
            {
                case 2:
                    maxAngle = (float)Math.PI / 3; break; // 60 degree spread
                case 3:
                    maxAngle = (float)Math.PI / 2; break; // 90 degree spread
                default:
                    maxAngle = (float)Math.PI * 2 / 3; break; // 120 degree spread
            }
            
            float startAngle = -maxAngle / 2f;
            float dir = Mode == DataSource.Mode.Input ? -1 : 1;
            float dist = height / 2f;

            for (int i = 0; i < Connections.Count; i++)
            {
                float angleIncrement = maxAngle * i / (float)(Connections.Count - 1);

                Point p = new Point(
                    MappingConnection.X + (int)(Math.Cos(startAngle + angleIncrement) * dist * dir + 0.5f),
                    MappingConnection.Y + (int)(Math.Sin(startAngle + angleIncrement) * dist + 0.5f)
                );

                Points.Add(p);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(0, 0, Width - 1, Height - 1));

            var pointPen = new Pen(Color.Blue, 10);
            var linePen = new Pen(Color.Green);

            foreach (var point in Points)
            {
                e.Graphics.DrawLine(linePen, MappingConnection, point);
                e.Graphics.DrawLine(pointPen, point, new Point(point.X + 1, point.Y + 1));
            }
        }

        public void DrawLinks(Graphics g)
        {
            var myBounds = Bounds;

            Point pos;
            switch (Connections.Count)
            {
                case 0:
                    break;
                case 1:
                    pos = Location;
                    pos.Offset(MappingConnection);
                    DrawLinks(g, Connections[0], pos);
                    break;
                default:
                    for (int i = 0; i < Connections.Count; i++)
                    {
                        pos = Location;
                        pos.Offset(Points[i]);
                        DrawLinks(g, Connections[i], pos);
                    }
                    break;
            }
        }

        static readonly Pen linePen = new Pen(Color.Green);
        private void DrawLinks(Graphics g, Mapping.Connection connection, Point mappingPoint)
        {
            foreach (var dt in connection.DataTypes)
                if (dt.SourceBase.ProjectControl != null)
                {
                    var bounds = dt.SourceBase.ProjectControl.Bounds;
                    g.DrawLine(linePen, new Point(Mode == DataSource.Mode.Input ? bounds.X + Bounds.Width : bounds.X, bounds.Y + bounds.Height / 2), mappingPoint);
                }
        }
    }
}
