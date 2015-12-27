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
            this.Up = new ConnectionZone(32, -10, this, false);
            this.Down = new ConnectionZone(32, 30, this, false);
            this.In = new ConnectionZone(-12, 12, this, true);
            if(this is AdjustableSplitter)
            {
                this.Up = new ConnectionZone(32, 5, this, false);
                this.Down = new ConnectionZone(32, 29, this, false);
                this.In = new ConnectionZone(-10, 17, this, true);
            }
        }
    }
}
