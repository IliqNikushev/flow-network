using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace Flow_Network
{
    public abstract class Element : IconDrawable
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

        public bool Contains(PathMidPointDrawable midpoint)
        {
            bool state = Contains(midpoint.Location);
            if (state) return true;

            return new Rectangle(this.X, this.Y, this.Width, this.Height).IntersectsWith(new Rectangle(midpoint.Location.X, midpoint.Location.Y, midpoint.Width, midpoint.Height));
        }

        private IEnumerable<ConnectionZone> connectionZones;

        public IEnumerable<Element> ConnectedElements
        {
            get
            {
                return this.ConnectionZones.Where(x=>x.IsInFlow).Select(x => x.ConnectedZone == null ? null : x.ConnectedZone.Parent).Where(x=>x!=null);
            }
        }

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
        public IEnumerable<ConnectionZone.Path> Connections { get { return ConnectionZone.Path.All.Where(x => (x.From.Parent == this || x.To.Parent == this) && x.To != null); } }

        const int POINT_DELTA = 0;//ConnectionZone.Path.DEFAULT_WIDTH / 2;

        /// <summary>Center of the bounding box of the element</summary>
        public Point Center { get { return new Point(this.X + this.Width / 2, this.Y + this.Height / 2); } }
        /// <summary>Bottom left point of the bounding box of the element</summary>
        public Point A { get { return new Point(this.X - POINT_DELTA, this.Y + this.Height + POINT_DELTA); } }
        /// <summary>Bottom right point of the bounding box of the element</summary>
        public Point B { get { return new Point(this.X + this.Width + POINT_DELTA, this.Y + this.Height + POINT_DELTA); } }
        /// <summary>Top right point of the bounding box of the element</summary>
        public Point C { get { return new Point(this.X + this.Width, this.Y - POINT_DELTA); } }
        /// <summary>Top left point of the bounding box of the element</summary>
        public Point D { get { return new Point(this.X, this.Y - POINT_DELTA); } }

        /// <summary>Returns DefaultSize.X</summary>
        public override int Width { get { return DefaultSize.X; } }
        /// <summary>Returns DefaultSize.Y</summary>
        public override int Height { get { return DefaultSize.Y; } }

        protected override void OnDrawClear(Graphics graphics, Color backgroundColor)
        {
            base.OnDrawClear(graphics,backgroundColor);
            //graphics.FillRectangle(new SolidBrush(backgroundColor), this.Location.X, this.Location.Y, this.Width, this.Height);
        }

        protected override void OnDraw(Graphics graphics, Color backgroundColor)
        {
            this.OnDrawClear(graphics, backgroundColor);
            base.OnDraw(graphics, backgroundColor);
        }

        public override void OnlyDraw(Graphics graphics, Color backgroundColor)
        {
            base.OnDraw(graphics, backgroundColor);
        }
    }
}
