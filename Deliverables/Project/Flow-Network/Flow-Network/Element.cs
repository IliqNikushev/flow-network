using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace Flow_Network
{
    public abstract class Element
    {
        /// <summary>Constant Default size for width and height when drawn</summary>
        public static readonly Point DefaultSize = new Point(42, 42);
        /// <summary>Returns all elements used in the Main form</summary>
        public static List<Element> AllElements { get { return Main.AllElements; } }

        /// <summary>Checks if a specific point is within the element's boundries</summary>
        public bool Contains(Point point)
        {
            return this.X <= point.X && this.X >= point.X + this.Width &&
                    this.Y <= point.Y && this.Y >= point.Y + this.Height;
        }

        /// <summary>Recalculates all connections of this element</summary>
        /// <param name="refresh">Passed to the ConnectionZone.Path.Adjust method</param>
        public void RefreshConnections(bool refresh = false)
        {
            foreach (ConnectionZone.Path connection in this.Connections)
            {
                connection.Adjust(refresh);
            }
        }

        private IEnumerable<ConnectionZone> connectionZones;

        /// <summary>Gets all Properties that are a connection zone within the class</summary>
        public IEnumerable<ConnectionZone> ConnectionZones
        {
            get
            {
                if (connectionZones == null)
                {
                    List<ConnectionZone> connectionZoneProps = new List<ConnectionZone>();
                    Type type = this.GetType();
                    while(type!=typeof(Element))
                    {
                        connectionZoneProps.AddRange(type.GetProperties(
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.Instance).
                        Where(x => x.PropertyType == typeof(ConnectionZone) && x.CanRead && x.CanWrite).
                        Select(x => x.GetValue(this) as ConnectionZone));
                        type = type.BaseType;
                    }
                    this.connectionZones = connectionZoneProps;
                    if (this.connectionZones.Where(x => x == null).Any()) 
                        throw new NotImplementedException("A connection zone is not implemented in " + this.GetType().Name);
                }
                return connectionZones;
            }
        }
        /// <summary>Gets all connections that are defined in Path.All and are from this element to any other</summary>
        public IEnumerable<ConnectionZone.Path> Connections { get { return ConnectionZone.Path.All.Where(x => x.From.Parent == this); } }

        /// <summary>Center of the bounding box of the element</summary>
        public Point Center { get { return new Point(this.X + this.Width / 2, this.Y + this.Height / 2); } }
        /// <summary>Bottom left point of the bounding box of the element</summary>
        public Point A { get { return new Point(this.X - 1, this.Y + this.Height + 2); } }
        /// <summary>Bottom right point of the bounding box of the element</summary>
        public Point B { get { return new Point(this.X + this.Width + 1, this.Y + this.Height + 1); } }
        /// <summary>Top right point of the bounding box of the element</summary>
        public Point C { get { return new Point(this.X + this.Width, this.Y - 2); } }
        /// <summary>Top left point of the bounding box of the element</summary>
        public Point D { get { return new Point(this.X, this.Y - 2); } }

        /// <summary>Returns DefaultSize.X</summary>
        public int Width { get { return DefaultSize.X; } }
        /// <summary>Returns DefaultSize.Y</summary>
        public int Height { get { return DefaultSize.Y; } }

        /// <summary>Gets the icon found in the Resources assosiacted to the current element</summary>
        public System.Drawing.Image Icon { get { return Resources.Icon(this); } }

        /// <summary>X coordinate in the 4-th sector of the coordinate system</summary>
        public int X { get; set; }
        /// <summary>Y coordinate in the 4-th sector of the coordinate system, where 4-th sector is positive, 2-nd is negative</summary>
        public int Y { get; set; }
        /// <summary>Location based on the X and Y</summary>
        public Point Location { get { return new Point(X, Y); } set { this.X = value.X; this.Y = value.Y; } }

    }
}
