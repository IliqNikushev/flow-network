using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    public class Splitter : Element
    {
        public ConnectionZone Up { get; private set; }

        /// <summary>
        /// range : 0-100
        /// </summary>
        public int UpPercent { get; private set; }

        public ConnectionZone Down { get; private set; }

        /// <summary>
        /// range : 0-100
        /// </summary>
        public int DownPercent { get; private set; }

        public ConnectionZone In { get; private set; }

        public Splitter()
        {
            this.Up = new ConnectionZone(new System.Drawing.Point(42, 0), this, false);
            this.Down = new ConnectionZone(new System.Drawing.Point(42, 42), this, false);
            this.In = new ConnectionZone(new System.Drawing.Point(0, 26), this, true);
        }
    }
}
