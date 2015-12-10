using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    /// <summary>Element that has 1 OUT flow point and can specify the out flow's amount </summary>
    public class PumpElement : Element
    {
        public PumpElement ()
        {
            this.Out = new ConnectionZone(32,32,this, false);
        }
        public ConnectionZone Out { get; private set; }

        ///<summary>Flow of the pump </summary>
        public float Flow { get; set; }
    }
}
