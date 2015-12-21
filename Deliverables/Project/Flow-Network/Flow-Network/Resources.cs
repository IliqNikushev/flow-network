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
        static Resources()
        {
            string notFound = "";
            foreach (var item in typeof(Resources).GetFields(System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Static))
            {
                if (item.FieldType == typeof(Dictionary<DrawState, System.Drawing.Image>))
                {
                    Dictionary<DrawState, System.Drawing.Image> collection = item.GetValue(null) as Dictionary<DrawState, System.Drawing.Image>;

                    foreach (var state in Enum.GetValues(typeof(DrawState)))
                    {
                        if ((DrawState)state == DrawState.None) continue;
                        if (!collection.ContainsKey((DrawState)state))
                            notFound += item.Name+"."+state+"\r\n";
                    }
                }
            }
            if (notFound != "")
                throw new Exception("Not found icons for:\r\n" + notFound);
        }
        public static Dictionary<DrawState, System.Drawing.Image> PumpIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.pump},
            {DrawState.Hovered, Properties.Resources.pump},
            {DrawState.Delete, Properties.Resources.pump},
            {DrawState.Blocking, Properties.Resources.pump},
            {DrawState.Active, Properties.Resources.pump}
        };

        public static Dictionary<DrawState, System.Drawing.Image> SinkIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.sink},
            {DrawState.Hovered, Properties.Resources.sink},
            {DrawState.Delete, Properties.Resources.sink},
            {DrawState.Blocking, Properties.Resources.sink},
            {DrawState.Active, Properties.Resources.sink}
        };

        public static Dictionary<DrawState, System.Drawing.Image> MergerIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.merger},
            {DrawState.Hovered, Properties.Resources.merger},
            {DrawState.Delete, Properties.Resources.merger},
            {DrawState.Blocking, Properties.Resources.merger},
            {DrawState.Active, Properties.Resources.merger}
        };

        public static Dictionary<DrawState, System.Drawing.Image> SplitterIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.splitter},
            {DrawState.Hovered, Properties.Resources.splitter},
            {DrawState.Delete, Properties.Resources.splitter},
            {DrawState.Blocking, Properties.Resources.splitter},
            {DrawState.Active, Properties.Resources.splitter}
        };

        public static Dictionary<DrawState, System.Drawing.Image> AdjSplitterIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.adjustableSplitter},
            {DrawState.Hovered, Properties.Resources.adjustableSplitter},
            {DrawState.Delete, Properties.Resources.adjustableSplitter},
            {DrawState.Blocking, Properties.Resources.adjustableSplitter},
            {DrawState.Active, Properties.Resources.adjustableSplitter}
        };

        public static Dictionary<DrawState, System.Drawing.Image> ConnectionZoneIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.connectionZone},
            {DrawState.Blocking, Properties.Resources.connectionZoneBlocking},
            {DrawState.Delete, Properties.Resources.connectionZoneDelete},
            {DrawState.Hovered, Properties.Resources.connectionZoneHovered},
            {DrawState.Active, Properties.Resources.connectionZoneActive}
        };

        /// <summary>Iterates the resources to find the specified element's proper icon</summary>
        public static System.Drawing.Image Icon(IconDrawable element)
        {
            if (element is PumpElement) return IconFromDictionary(PumpIcons, element);
            else if (element is SinkElement) return IconFromDictionary(SinkIcons, element);
            else if (element is MergerElement) return IconFromDictionary(MergerIcons, element);
            else if (element is SplitterElement) return IconFromDictionary(SplitterIcons, element);
            else if (element is AdjustableSplitter) return IconFromDictionary(AdjSplitterIcons, element);
            else if (element is ConnectionZone) return Icon(element as ConnectionZone);
            throw new ArgumentException("Element not implemented, " + element.GetType().Name);
        }

        public static System.Drawing.Image Icon(ConnectionZone zone)
        {
            return IconFromDictionary(ConnectionZoneIcons, zone);
        }

        private static System.Drawing.Image IconFromDictionary(Dictionary<DrawState, System.Drawing.Image> dictionary, IconDrawable e)
        {
            if (!dictionary.ContainsKey(e.DrawState))
                throw new NotImplementedException("Missing icon for:" + e.GetType().Name + "." + e.DrawState);
            return dictionary[e.DrawState];
        }
    }
}
