using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Flow_Network
{
    class Elem
    {
        public static List<Elem> AllElements { get { return new List<Elem>(); } }

        public Point A { get { return new Point(this.X, this.Y + this.Height); } }
        public Point B { get { return new Point(this.X + this.Width, this.Y + this.Height); } }
        public Point C { get { return new Point(this.X + this.Width, this.Y); } }
        public Point D { get { return new Point(this.X, this.Y); } }

        public int Width { get { return this.Size.X; } }
        public int Height { get { return this.Size.Y; } }

        public int Y { get { return this.Position.Y; } }
        public int X { get { return this.Position.X; } }

        public Point Position { get; private set; }
        public Point Size { get; private set; }
    }
}
