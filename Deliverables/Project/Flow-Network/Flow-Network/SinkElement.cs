using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    /// <summary>Element that has 1 IN flow point</summary>
    public class SinkElement : Element
    {
        public ConnectionZone In { get; private set; }

        public SinkElement()
        {
            this.In = new ConnectionZone(11, 12, this, true);
        }

        public float Flow { get { return this.In.Flow; } }

        private float lastFlow;

        private System.Drawing.Font labelFont = System.Drawing.SystemFonts.MessageBoxFont;

        protected override void OnDraw(System.Drawing.Graphics graphics, System.Drawing.Color backgroundColor)
        {
            System.Drawing.SizeF size = graphics.MeasureString(lastFlow.ToString(),labelFont);
            graphics.FillRectangle(new System.Drawing.SolidBrush(backgroundColor), this.Location.X, this.Location.Y, size.Width, size.Height);
            lastFlow = Flow;

            base.OnDraw(graphics, backgroundColor);

            size = graphics.MeasureString(lastFlow.ToString(), labelFont);
            graphics.FillRectangle(new System.Drawing.SolidBrush(backgroundColor), this.Location.X, this.Location.Y, size.Width, size.Height);
            graphics.DrawRectangle(System.Drawing.Pens.Black, this.Location.X, this.Location.Y, size.Width, size.Height);
            graphics.DrawString(this.Flow.ToString(), labelFont, System.Drawing.Brushes.Black, this.Location);
        }
    }
}
