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
        /// <summary>X coordinate in the 4-th sector of the coordinate system</summary>
        public int X { get; set; }
        /// <summary>Y coordinate in the 4-th sector of the coordinate system, where 4-th sector is positive, 2-nd is negative</summary>
        public int Y { get; set; }

        public abstract int Width { get; }
        public abstract int Height { get; }

        /// <summary>Location based on the X and Y</summary>
        public virtual Point Location { get { return new Point(X, Y); } set { this.X = value.X; this.Y = value.Y; } }

        protected override void OnDraw(System.Drawing.Graphics graphics, Color backgroundColor)
        {
            graphics.DrawImage(this.Icon, this.Location.X, this.Location.Y, this.Width, this.Height);
        }

        public virtual void OnlyDraw(System.Drawing.Graphics graphics, Color backgroundColor) { }

        protected override void OnDrawClear(Graphics g, Color backgroundColor)
        {
            DrawState previous = this.LastState;
            this.DrawState = Flow_Network.DrawState.Clear;
            g.DrawImage(this.Icon, this.Location.X, this.Location.Y, this.Width, this.Height);
            this.DrawState = this.LastState;
            this.LastState = previous;
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

        static Dictionary<System.Drawing.Graphics, object> singletonGraphics = new Dictionary<Graphics, object>();
        static object singletonLock = new object();

        public static void LockGraphicsForDraw(Graphics graphics, Action draw)
        {
            if (draw == null) return;
            lock (singletonLock)
                if (!singletonGraphics.ContainsKey(graphics))
                    singletonGraphics.Add(graphics, new object());

            lock (singletonGraphics[graphics])
                draw();
        }

        public void Draw(System.Drawing.Graphics graphics, Color backgroundColor)
        {
            LockGraphicsForDraw(graphics, () => this.OnDraw(graphics, backgroundColor));
        }
        public void DrawClear(System.Drawing.Graphics g, Color backgroundColor)
        {
            LockGraphicsForDraw(g, () => this.OnDrawClear(g, backgroundColor));
        }

        protected abstract void OnDraw(System.Drawing.Graphics graphics, Color backgroundColor);
        protected abstract void OnDrawClear(System.Drawing.Graphics g, Color backgroundColor);
    }
}
