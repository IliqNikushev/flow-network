using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    /// <summary>Element that has an 2 OUT 1 IN flow points. The flow is 50%</summary>
    public class SplitterElement : Element
    {
        public ConnectionZone Up { get; private set; }

        public ConnectionZone Down { get; private set; }

        public ConnectionZone In { get; private set; }

        public SplitterElement()
        {
            this.Up = new ConnectionZone(this.Width, 0, this, false);
            this.Down = new ConnectionZone(this.Width, this.Height, this, false);
            this.In = new ConnectionZone(0, this.Height / 2, this, true);
        }
    }
}
