using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network
{
    public class PathMidPointDrawable : IconDrawable
    {
        public ConnectionZone.Path Path { get; private set; }
        public PathMidPointDrawable(int x, int y, ConnectionZone.Path path)
        {
            this.X = x;
            this.Y = y;
            this.Path = path;
        }

        public override int Height
        {
            get { return ConnectionZone.Path.DEFAULT_WIDTH * 2; }
        }

        public override int Width
        {
            get { return ConnectionZone.Path.DEFAULT_WIDTH * 2; }
        }

        public override System.Drawing.Point Location
        {
            get
            {
                System.Drawing.Point location = base.Location;
                location.X -= this.Width / 2;
                location.Y -= this.Height / 2;
                return location;
            }
            set
            {
                base.Location = value;
            }
        }

        protected override void OnDraw(System.Drawing.Graphics graphics, System.Drawing.Color backgroundColor)
        {
            this.DrawClear(graphics, backgroundColor);
            base.OnDraw(graphics, backgroundColor);
        }

        public override void OnlyDraw(System.Drawing.Graphics graphics, System.Drawing.Color backgroundColor)
        {
            base.OnDraw(graphics, backgroundColor);
        }
    }
}
