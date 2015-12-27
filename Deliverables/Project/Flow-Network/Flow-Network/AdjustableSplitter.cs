using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    /// <summary>
    /// Splitter that can have the out flow adjusted in percentage
    /// </summary>
    public class AdjustableSplitter : SplitterElement
    {
        public delegate void AdjustEventArgs(AdjustableSplitter splitter, int previous, int current);
        private int upPercent=100;
        /// <summary>
        /// Value of the % flow of the up stream
        /// range : 0-100
        /// </summary>
        public int UpFlowPercent
        {
            get
            { 
                return upPercent; 
            }
            set
            {
                if (value < 0) value = 0;
                else if (value > 100) value = 100;
                int previous = upPercent;
                upPercent = value;

                if (previous != value) 
                    OnAdjusted(this, previous, value);
            }
        }

        /// <summary>
        /// Value of the % flow of the down stream
        /// range : 0-100
        /// </summary>
        public int DownFlowPercent
        {
            get
            { 
                return 100 - upPercent; 
            }
            set
            {
                if (value < 0) value = 0;
                else if (value > 100) value = 100;
                UpFlowPercent = 100 - value;
            }
        }

        /// <summary>Called when the up flow percent has been changed</summary>
        public event AdjustEventArgs OnAdjusted = (e, x, y) => { };
    }
}