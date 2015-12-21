using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Flow_Network
{
    public enum DrawState
    {
        Normal,
        Hovered,
        Delete,
        Blocking
    }

    public abstract class IconDrawable : Drawable
    {
        /// <summary>Gets the icon found in the Resources assosiacted to the current element</summary>
        public System.Drawing.Image Icon { get { return Resources.Icon(this); } }

        /// <summary>X coordinate in the 4-th sector of the coordinate system</summary>
        public int X { get; set; }
        /// <summary>Y coordinate in the 4-th sector of the coordinate system, where 4-th sector is positive, 2-nd is negative</summary>
        public int Y { get; set; }

        public abstract int Width { get; }
        public abstract int Height { get; }

        /// <summary>Location based on the X and Y</summary>
        public virtual Point Location { get { return new Point(X, Y); } set { this.X = value.X; this.Y = value.Y; } }

        public override void Draw(System.Drawing.Graphics graphics)
        {
            graphics.DrawImage(this.Icon, this.X, this.Y, this.Width, this.Height);
        }
    }

    public abstract class Drawable
    {
        public DrawState DrawState { get; set; }

        public abstract void Draw(System.Drawing.Graphics graphics);
    }
}
