using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    public class Pump : Element
    {
        public Pump ()
        {
            this.Out = new ConnectionZone(new System.Drawing.Point(32,32),this);
        }
        public ConnectionZone Out { get; private set; }
        
    }
}
