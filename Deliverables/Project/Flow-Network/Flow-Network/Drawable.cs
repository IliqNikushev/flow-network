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
        None = 0,
        Normal,
        Hovered,
        Delete,
        Blocking,
        Active,
        Clear
    }

    public abstract class IconDrawable : Drawable
    {
        /// <summary>Gets the icon found in the Resources assosiacted to the current element</summary>
        public System.Drawing.Image Icon { get { return Resources.Icon(this); } }
        public System.Drawing.Image ClearIcon
        {
            get
            {
                this.DrawState = Flow_Network.DrawState.Clear;
                System.Drawing.Image image = this.Icon;
                this.DrawState = this.LastState;
                return image;
            }
        }
        /// <summary>X coordinate in the 4-th sector of the coordinate system</summary>
        public int X { get; set; }
        /// <summary>Y coordinate in the 4-th sector of the coordinate system, where 4-th sector is positive, 2-nd is negative</summary>
        public int Y { get; set; }

        public abstract int Width { get; }
        public abstract int Height { get; }

        /// <summary>Location based on the X and Y</summary>
        public virtual Point Location { get { return new Point(X, Y); } set { this.X = value.X; this.Y = value.Y; } }

        public override void Draw(System.Drawing.Graphics graphics, Color backgroundColor)
        {
            if (this is Element)
                graphics.FillRectangle(new SolidBrush(backgroundColor), this.Location.X, this.Location.Y, this.Width, this.Height);
            graphics.DrawImage(this.Icon, this.Location.X, this.Location.Y, this.Width, this.Height);
        }
    }

    public abstract class Drawable
    {
        public Drawable()
        {
            this.LastState = Flow_Network.DrawState.Normal;
            this.DrawState = Flow_Network.DrawState.Normal;
        }

        private DrawState state;
        public DrawState LastState { get; set; }
        public DrawState DrawState
        {
            get { return state; }
            set
            {
                if (value == Flow_Network.DrawState.None) value = Flow_Network.DrawState.Normal;
                if (this.state == value) return;
                this.LastState = this.state;
                this.state = value;
            }
        }

        public abstract void Draw(System.Drawing.Graphics graphics, Color backgroundColor);
    }
}
