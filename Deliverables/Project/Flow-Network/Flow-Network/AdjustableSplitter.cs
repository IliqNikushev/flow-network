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
        private int upPercent;
        /// <summary>range : 0-100</summary>
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
                float previous = upPercent;
                upPercent = value;

                if (upPercent != previous) OnAdjusted(this, previous, upPercent);
            }
        }

        /// <summary>range : 0-100</summary>
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

        /// <summary>Called when the up flow percent has been changed, left value is previous, right value is current and Adjustable splitter is the current splitter</summary>
        public event Action<AdjustableSplitter, float, float> OnAdjusted = (e, x, y) => { };
    }
}