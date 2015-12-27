using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    /// <summary>
    /// Element that has 2 IN and 1 OUT flow points. The out flow is the sum of the two in going flows
    /// </summary>
    public class MergerElement : Element
    {
        public ConnectionZone Up { get; private set; }

        public ConnectionZone Down { get; private set; }

        public ConnectionZone Out { get; private set; }

        public MergerElement()
        {
            this.Up = new ConnectionZone(-10, -6, this, true);
            this.Down = new ConnectionZone(-10, 30, this, true);
            this.Out = new ConnectionZone(34, 12, this, false);
        }
    }
}
