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

        /// <summary>Iterates the resources to find the specified element's proper icon</summary>
        public static System.Drawing.Image Icon(Element element)
        {
            if (element is PumpElement) return PumpIcon;
            else if (element is SinkElement) return SinkIcon;
            else if (element is MergerElement) return MergerIcon;
            else if (element is SplitterElement) return SplitterIcon;
            else if (element is AdjustableSplitter) return AdjSplitterIcon;
            throw new ArgumentException("Element not implemented, " + element.GetType().Name);
        }
    }
}
