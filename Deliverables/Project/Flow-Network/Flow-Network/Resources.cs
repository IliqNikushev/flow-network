using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network
{
    static class Resources
    {
        public static System.Drawing.Image PumpIcon;
        public static System.Drawing.Image SinkIcon;
        public static System.Drawing.Image MergerIcon;
        public static System.Drawing.Image SplitterIcon;
        public static System.Drawing.Image AdjSplitterIcon;

        public static System.Drawing.Image Icon(Element e)
        {
            if (e is Pump) return PumpIcon;
            else if (e is Sink) return SinkIcon;
            else if (e is Merger) return MergerIcon;
            else if (e is Splitter) return SplitterIcon;
            else if (e is AdjustableSplitter) return AdjSplitterIcon;
            throw new ArgumentException("Element not implemented, " + e.GetType().Name);
        }
    }
}
