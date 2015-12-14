using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Flow_Network
{
    /// <summary>Zone in which an element can be connected with a path</summary>
    public class ConnectionZone
    {
        /// <summary>Constant Default size for width and height when drawn</summary>
        public static readonly Point DefaultSize = new Point(20, 20);
        private bool isInFlow;
        
        /// <summary>Element that the zone belongs to</summary>
        public Element Parent { get; private set; }

        /// <summary>Distance from the parent's X,Y</summary>
        public Point Margin { get; private set; }

        /// <summary>returns DefaultSize.X</summary>
        public int Width { get { return DefaultSize.X; } }
        /// <summary>returns DefaultSize.Y</summary>
        public int Height { get { return DefaultSize.Y; } }

        /// <summary>Returns the parent's location + current (X,Y)</summary>
        public Point Location
        {
            get
            {
                return new Point(this.Margin.X + this.Parent.X, this.Margin.Y + this.Parent.Y);
            }
        }

        public bool IsInFlow
        {
            get
            {
                return isInFlow;
            }
        }

        public bool IsOutFlow
        {
            get
            {
                return !isInFlow;
            }
        }

        /// <summary>If connected, returns the element prior to the current on the path they are connected</summary>
        public ConnectionZone Previous { get; private set; }

        /// <summary>returns the current flow that is passing through the zone, by going up the connected path it lays on</summary>
        public float Flow
        {
            get
            {
                if (Previous != null)
                {
                    float flow = Previous.Flow;
                        
                    if (this.Parent is SplitterElement)
                        flow *= 0.5f;
                    else if(this.Parent is AdjustableSplitter)
                    {
                        AdjustableSplitter splitter = this.Parent as AdjustableSplitter;
                        float percent = 1;
                        if (this == splitter.Up) 
                            percent = ((100 - splitter.UpFlowPercent) / 100);
                        else
                            percent = ((100 - splitter.DownFlowPercent) / 100);
                            flow *= percent;
                    }
                    else if(this.Parent is MergerElement)
                    {
                        MergerElement merger = this.Parent as MergerElement;
                        flow = merger.Up.Flow + merger.Down.Flow;
                    }
                    return flow;
                }
                if (this.Parent is PumpElement)
                    return (this.Parent as PumpElement).Flow;
                return 0;
            }
        }
        public int State { get; set; }
        public ConnectionZone(int x, int y, Element parent, bool isInFlow) : this(new Point(x,y), parent, isInFlow) { }

        public ConnectionZone(Point margin, Element parent, bool isInflow)
        {
            this.Margin = margin;
            this.Parent = parent;
            this.isInFlow = isInflow;
            this.State = 0;
        }

        public static Path GetPathFromTo(ConnectionZone from, ConnectionZone to)
        {
            return new Path(from, to);
        }

        public Path GetPathTo(ConnectionZone to)
        {
            return GetPathFromTo(this, to);
        }

        public static implicit operator Point(ConnectionZone connection)
        {
            return new Point(connection.Location.X + connection.Width / 2, connection.Location.Y + connection.Height / 2);
        }

        /// <summary>Path from 2 connection zones.</summary>
        public class Path
        {
            public delegate void FlowAlteredEvent(Path path, float previous, float current);
            /// <summary>Returns all paths defineed in the Main form</summary>
            public static List<Path> All { get { return Main.AllPaths; } }
            public ConnectionZone From { get; private set; }
            public ConnectionZone To { get; private set; }

            private float maxFlow;
            /// <summary>
            /// Maximum flow that can go through this path
            /// Minimum : 0
            /// </summary>
            public float MaxFlow
            {
                get { return maxFlow; }
                set
                {
                    if (value < 0) value = 0;

                    float previous = this.maxFlow;
                    this.maxFlow = value;

                    if (previous != value)
                        OnMaxFlowChanged(this, previous, value);
                }
            }

            /// <summary>Called when the max flow has been altered</summary>
            public event FlowAlteredEvent OnMaxFlowChanged = (x,y,z) => { };

            /// <summary>Returns a path starting at the FROM zone to the TO zone, with all midpoints inbetween</summary>
            public List<Point> PathPoints
            {
                get
                {
                    List<Point> result = new List<Point>();
                    result.Add(From);
                    lock (threadLock)
                    {
                        result.AddRange(MidPoints);
                    }
                    result.Add(To);
                    return result;
                }
            }

            /// <summary>Points the path HAS to go to before reaching the TO zone</summary>
            public List<Point> UserDefinedMidPoints = new List<Point>();
            /// <summary>Points the path will go through before reaching the TO zone</summary>
            private List<Point> MidPoints = new List<Point>();

            public Path(ConnectionZone from, ConnectionZone to)
            {
                if (from.isInFlow == to.isInFlow) throw new ArgumentException("Two connection zones with same flow type connected");
                if (from.IsOutFlow)
                {
                    this.From = to;
                    this.To = from;
                }
                else
                {
                    this.From = from;
                    this.To = to;
                }
            }

            private bool isAdded = false;

            /// <summary>Adds to the All collection of paths if not added</summary>
            public void Add()
            {
                if (isAdded) return;
                this.To.State = 1;
                this.From.State = 1;
                ConnectionZone.Path.All.Add(this);
                this.isAdded = true;
                this.To.Previous = this.From;
            }

            /// <summary>Removes from the All collection of paths if present</summary>
            public void Remove()
            {
                if (!isAdded) return;
                this.From.State = 0;
                this.To.State = 0;
                ConnectionZone.Path.All.Remove(this);
                this.isAdded = false;
                this.To.Previous = null;
            }

            /// <summary>Called when the path is adjusted for the first time</summary>
            public event System.Action OnCreated = () => { };
            /// <summary>Called when the path has been adjusted</summary>
            public event System.Action OnAdjusted = () => { };

            private System.Threading.Thread activeAdjuster;
            private bool IsAdjusting { get { return activeAdjuster != null && activeAdjuster.ThreadState == System.Threading.ThreadState.Running; } }

            private bool isNew = true;

            private object threadLock = new object();

            /// <summary>Calculates the new midpoints based on the position of the Elements.All and the user defined mid points
            /// The process is executed on a different thread to not hold up the program</summary>
            /// <param name="refresh">If set to true, will call OnAdjusted</param>
            public void Adjust(bool refresh = false)
            {
                lock (threadLock)
                {
                    if (IsAdjusting) activeAdjuster.Abort();
                }

                List<Point> pointsToGoThrough = new List<Point>();
                pointsToGoThrough.Add(this.From);
                pointsToGoThrough.AddRange(UserDefinedMidPoints);
                pointsToGoThrough.Add(this.To);

                activeAdjuster = new System.Threading.Thread(() =>
                {
                    try
                    {
                        lock (threadLock)
                        {
                            this.MidPoints.Clear();
                        }


                        HashSet<Element> elementsAlreadyCollided = new HashSet<Element>();

                        int currentPointStartIndex = 0;
                        Point start = pointsToGoThrough[currentPointStartIndex];
                        Point end = pointsToGoThrough[currentPointStartIndex + 1];

                        while (true)
                        {
                            Collision collision = Collision.FindBetween(start, end, this.From, this.To, ref elementsAlreadyCollided, Element.AllElements);
                            if (!collision)
                            {
                                currentPointStartIndex += 1;
                                if (currentPointStartIndex + 1 >= pointsToGoThrough.Count) break;
                                start = pointsToGoThrough[currentPointStartIndex];
                                end = pointsToGoThrough[currentPointStartIndex + 1];
                            }
                            else
                            {
                                lock (threadLock)
                                {
                                    //1 3;2 4
                                    if (collision.SenderIsOnLeft && !collision.TargetIsOnLeft)
                                    {
                                        // 1;2
                                        if (collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.D);
                                            this.MidPoints.Add(collision.Element.C);
                                            start = collision.Element.C;
                                        }//1; 4
                                        else if (collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.D);
                                            this.MidPoints.Add(collision.Element.C);
                                            this.MidPoints.Add(collision.Element.B);
                                            start = collision.Element.B;
                                        }//3;2
                                        if (!collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.A);
                                            this.MidPoints.Add(collision.Element.B);
                                            this.MidPoints.Add(collision.Element.C);
                                            start = collision.Element.C;
                                        }//3;4
                                        else if (!collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.A);
                                            this.MidPoints.Add(collision.Element.B);
                                            start = collision.Element.B;
                                        }
                                    }//1 3;1 3
                                    else if (collision.SenderIsOnLeft && collision.TargetIsOnLeft)
                                    {
                                        //1;1
                                        if (collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.D);
                                            start = collision.Element.D;
                                        }//1;3
                                        else if (collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.D);
                                            this.MidPoints.Add(collision.Element.A);
                                            start = collision.Element.A;
                                        }//3;1
                                        else if (!collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.A);
                                            this.MidPoints.Add(collision.Element.D);
                                            start = collision.Element.D;
                                        }//3;3
                                        else if (!collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.A);
                                            start = collision.Element.A;
                                        }
                                    }
                                    //2 4; 2 4
                                    else if (!collision.SenderIsOnLeft && !collision.TargetIsOnLeft)
                                    {
                                        //2;2
                                        if (collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.C);
                                            start = collision.Element.C;
                                        }//2;4
                                        else if (collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.C);
                                            this.MidPoints.Add(collision.Element.B);
                                            start = collision.Element.B;
                                        }//4;2
                                        else if (!collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.B);
                                            this.MidPoints.Add(collision.Element.C);
                                            start = collision.Element.C;
                                        }//4;4
                                        else if (!collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.B);
                                            start = collision.Element.B;
                                        }
                                    }//2 4; 1 3
                                    else if (!collision.SenderIsOnLeft && collision.TargetIsOnLeft)
                                    {
                                        // 2;1
                                        if (collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.C);
                                            this.MidPoints.Add(collision.Element.D);
                                            start = collision.Element.D;
                                        }//2; 3
                                        else if (collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.C);
                                            this.MidPoints.Add(collision.Element.B);
                                            this.MidPoints.Add(collision.Element.A);
                                            start = collision.Element.A;
                                        }//4;1
                                        if (!collision.SenderIsUp && collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.B);
                                            this.MidPoints.Add(collision.Element.C);
                                            this.MidPoints.Add(collision.Element.D);
                                            start = collision.Element.D;
                                        }//4;3
                                        else if (!collision.SenderIsUp && !collision.TargetIsUp)
                                        {
                                            this.MidPoints.Add(collision.Element.B);
                                            this.MidPoints.Add(collision.Element.A);
                                            start = collision.Element.A;
                                        }
                                    }
                                }
                            }
                        }

                        if (isNew)
                        {
                            OnCreated();
                            isNew = false;
                        }
                        else
                            if (refresh)
                                OnAdjusted();
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                    }
                });
                activeAdjuster.Start();
            }
        }
    }
}
