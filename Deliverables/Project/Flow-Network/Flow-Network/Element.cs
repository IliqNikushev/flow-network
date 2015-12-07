using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace Flow_Network
{
    public abstract class Element
    {
        public static List<Element> AllElements { get { return Main.AllElements; } }

        public ConnectionZone ParentConnection { get; private set; }

        public bool Contains(Point point)
        {
            return this.X <= point.X && this.X >= point.X + this.Width &&
                    this.Y <= point.Y && this.Y >= point.Y + this.Height;
        }

        public void RefreshConnections(bool refresh = false)
        {
            foreach (ConnectionZone.Path connection in this.Connections)
            {
                connection.Adjust(refresh);
            }
        }
        public IEnumerable<ConnectionZone> ConnectionZones
        {
            get
            {
                var prop =
                this.GetType().GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance).
                Where(x => x.PropertyType == typeof(ConnectionZone) && x.CanRead && x.CanWrite);
                return prop.Select(x => x.GetValue(this) as ConnectionZone);

            }
        }
        public IEnumerable<ConnectionZone.Path> Connections { get { return ConnectionZone.Path.All.Where(x => x.From.Parent == this); } }

        public Point Center { get { return new Point(this.X + this.Width / 2, this.Y + this.Height / 2); } }
        public Point A { get { return new Point(this.X - 1, this.Y + this.Height + 2); } }
        public Point B { get { return new Point(this.X + this.Width + 1, this.Y + this.Height + 1); } }
        public Point C { get { return new Point(this.X + this.Width, this.Y - 2); } }
        public Point D { get { return new Point(this.X, this.Y - 2); } }

        public int Width { get { return 42; } }
        public int Height { get { return 42; } }

        public System.Drawing.Image Icon { get { return Resources.Icon(this); } }


        public int X { get; set; }

        public int Y { get; set; }
        public Point Location { get { return new Point(X, Y); } set { X = value.X; Y = value.Y; } }

    }
}
