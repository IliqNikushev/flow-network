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
        static Dictionary<DrawState, System.Drawing.Color> ImageColors = new Dictionary<DrawState, System.Drawing.Color>()
        {
            {DrawState.Normal, System.Drawing.Color.Black},
            {DrawState.Hovered, System.Drawing.Color.Gold},
            {DrawState.Delete, System.Drawing.Color.Red},
            {DrawState.Blocking, System.Drawing.Color.DarkRed},
            {DrawState.Active, System.Drawing.Color.Blue},
            {DrawState.Clear, System.Drawing.Color.OldLace}
        };

        static Dictionary<DrawState, System.Drawing.Color> ConnectionZoneColors = new Dictionary<DrawState, System.Drawing.Color>()
        {
            {DrawState.Normal, System.Drawing.Color.Green},
            {DrawState.Hovered, System.Drawing.Color.Blue},
            {DrawState.Delete, System.Drawing.Color.Red},
            {DrawState.Blocking, System.Drawing.Color.DarkRed},
            {DrawState.Active, System.Drawing.Color.Purple},
            {DrawState.Clear, System.Drawing.Color.OldLace}
        };

        public static void Initialize()
        {
        }

        static Resources()
        {
            string notFound = "";
            foreach (var item in typeof(Resources).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if (item.FieldType == typeof(Dictionary<DrawState, System.Drawing.Image>))
                {
                    Dictionary<DrawState, System.Drawing.Image> collection = item.GetValue(null) as Dictionary<DrawState, System.Drawing.Image>;

                    foreach (var s in Enum.GetValues(typeof(DrawState)))
                    {
                        DrawState state = (DrawState)s;
                        if (state == DrawState.None) continue;
                        if (!collection.ContainsKey(state))
                            notFound += item.Name + "." + state + "\r\n";
                        System.Drawing.Color color = ImageColors[state];
                        if (collection == ConnectionZoneIcons)
                            color = ConnectionZoneColors[state];
                        else
                            color = ImageColors[state];
                        System.Drawing.Bitmap b = new System.Drawing.Bitmap(collection[state]);
                        for (int x = 0; x < b.Width; x++)
                        {
                            for (int y = 0; y < b.Height; y++)
                            {
                                System.Drawing.Color pixel = b.GetPixel(x, y);
                                System.Drawing.Color bg = color;
                                if (pixel.A != 0)
                                {
                                    //todo if A < 255 -> DRAW 100% or 100% - A%
                                    if (state == DrawState.Clear)
                                        pixel = System.Drawing.Color.FromArgb(255, bg.R, bg.G, bg.B);
                                    else
                                        pixel = System.Drawing.Color.FromArgb(pixel.A > 128 ? 255 : 0, bg.R, bg.G, bg.B);
                                    b.SetPixel(x, y, pixel);
                                }
                            }
                        }
                        collection[state] = b;
                    }
                }
            }
            if (notFound != "")
                throw new Exception("Not found icons for:\r\n" + notFound);
        }

        public static Dictionary<DrawState, System.Drawing.Image> MidPointIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.pathMidPoint},
            {DrawState.Hovered, Properties.Resources.pathMidPoint},
            {DrawState.Delete, Properties.Resources.pathMidPoint},
            {DrawState.Blocking, Properties.Resources.pathMidPoint},
            {DrawState.Active, Properties.Resources.pathMidPoint},
            {DrawState.Clear, Properties.Resources.pathMidPoint}
        };

        public static Dictionary<DrawState, System.Drawing.Image> PumpIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.pump},
            {DrawState.Hovered, Properties.Resources.pump},
            {DrawState.Delete, Properties.Resources.pump},
            {DrawState.Blocking, Properties.Resources.pump},
            {DrawState.Active, Properties.Resources.pump},
            {DrawState.Clear, Properties.Resources.pump}
        };

        public static Dictionary<DrawState, System.Drawing.Image> SinkIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.sink},
            {DrawState.Hovered, Properties.Resources.sink},
            {DrawState.Delete, Properties.Resources.sink},
            {DrawState.Blocking, Properties.Resources.sink},
            {DrawState.Active, Properties.Resources.sink},
            {DrawState.Clear, Properties.Resources.sink}
        };

        public static Dictionary<DrawState, System.Drawing.Image> MergerIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.merger},
            {DrawState.Hovered, Properties.Resources.merger},
            {DrawState.Delete, Properties.Resources.merger},
            {DrawState.Blocking, Properties.Resources.merger},
            {DrawState.Active, Properties.Resources.merger},
            {DrawState.Clear, Properties.Resources.merger}
        };

        public static Dictionary<DrawState, System.Drawing.Image> SplitterIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.splitter},
            {DrawState.Hovered, Properties.Resources.splitter},
            {DrawState.Delete, Properties.Resources.splitter},
            {DrawState.Blocking, Properties.Resources.splitter},
            {DrawState.Active, Properties.Resources.splitter},
            {DrawState.Clear, Properties.Resources.splitter}
        };

        public static Dictionary<DrawState, System.Drawing.Image> AdjSplitterIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.adjustableSplitter},
            {DrawState.Hovered, Properties.Resources.adjustableSplitter},
            {DrawState.Delete, Properties.Resources.adjustableSplitter},
            {DrawState.Blocking, Properties.Resources.adjustableSplitter},
            {DrawState.Active, Properties.Resources.adjustableSplitter},
            {DrawState.Clear, Properties.Resources.adjustableSplitter}
        };

        public static Dictionary<DrawState, System.Drawing.Image> ConnectionZoneIcons = new Dictionary<DrawState, System.Drawing.Image>()
        {
            {DrawState.Normal, Properties.Resources.connectionZone},
            {DrawState.Blocking, Properties.Resources.connectionZone},
            {DrawState.Delete, Properties.Resources.connectionZone},
            {DrawState.Hovered, Properties.Resources.connectionZone},
            {DrawState.Active, Properties.Resources.connectionZone},
            {DrawState.Clear, Properties.Resources.connectionZone}
        };

        /// <summary>Iterates the resources to find the specified element's proper icon</summary>
        public static System.Drawing.Image Icon(IconDrawable element)
        {
            if (element is PumpElement) return IconFromDictionary(PumpIcons, element);
            else if (element is SinkElement) return IconFromDictionary(SinkIcons, element);
            else if (element is MergerElement) return IconFromDictionary(MergerIcons, element);
            else if (element is AdjustableSplitter) return IconFromDictionary(AdjSplitterIcons, element);
            else if (element is SplitterElement) return IconFromDictionary(SplitterIcons, element);
            else if (element is ConnectionZone) return Icon(element as ConnectionZone);
            else if (element is PathMidPointDrawable) return IconFromDictionary(MidPointIcons, element);
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
