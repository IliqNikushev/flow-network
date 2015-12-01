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

        public IEnumerable<ConnectionZone.Path> Connections { get { return ConnectionZone.Path.All.Where(x => x.From.Parent == this); } }

        public Point Center { get { return new Point(this.X + this.Width / 2, this.Y + this.Height / 2); } }
        public Point A { get { return new Point(this.X-1, this.Y + this.Height+2); } }
        public Point B { get { return new Point(this.X + this.Width+1, this.Y + this.Height+1); } }
        public Point C { get { return new Point(this.X + this.Width, this.Y-2); } }
        public Point D { get { return new Point(this.X, this.Y-2); } }

        public int Width { get { return this.PictureBox.Width; } }
        public int Height { get { return this.PictureBox.Height; } }

        public System.Drawing.Image Icon { get { return Resources.Icon(this); } }
        public System.Windows.Forms.PictureBox PictureBox { get; private set; }

        public Element()
        {
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.PictureBox.Image = this.Icon;
            this.PictureBox.Width = 32;
            this.PictureBox.Height = 32;
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            
        }
        public int X
        {
            get
            {
                return this.PictureBox.Left;
            }
            set
            {
                this.PictureBox.Left = value;
            }
        }

        public int Y
        {
            get
            {
                return this.PictureBox.Top;
            }
            set
            {
                this.PictureBox.Top = value;
            }
        }
    }
}
