using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network
{
    /// <summary>Static class for resources</summary>
    static class Resources
    {
        public static System.Drawing.Image PumpIcon;
        public static System.Drawing.Image SinkIcon;
        public static System.Drawing.Image MergerIcon;
        public static System.Drawing.Image SplitterIcon;
        public static System.Drawing.Image AdjSplitterIcon;

        public static System.Drawing.Image Icon(Element e)
        {
            if (e is PumpElement) return PumpIcon;
            else if (e is SinkElement) return SinkIcon;
            else if (e is MergerElement) return MergerIcon;
            else if (e is SplitterElement) return SplitterIcon;
            else if (e is AdjustableSplitter) return AdjSplitterIcon;
            throw new ArgumentException("Element not implemented, " + e.GetType().Name);
        }
    }
}
