﻿using System;
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
            this.In = new ConnectionZone(0, this.Height / 2, this, true);
        }
    }
}