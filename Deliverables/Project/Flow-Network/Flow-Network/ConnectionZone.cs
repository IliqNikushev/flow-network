using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Flow_Network
{
    public class ConnectionZone
    {
        private bool isInFlow;
        
        public Element Parent { get; private set; }

        public Point Margin { get; private set; }

        public int Width { get { return 20; } }
        public int Height { get { return 20; } }

        public Point Position
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

        public ConnectionZone Previous { get; private set; }

        public float Flow
        {
            get
            {
                if (Previous != null)
                {
                    float flow = Previous.Flow;
                        
                    if (this.Parent is Splitter)
                        flow *= 0.5f;
                    else if(this.Parent is AdjustableSplitter)
                    {
                        AdjustableSplitter splitter = this.Parent as AdjustableSplitter;
                        float percent = 1;
                        if (this == splitter.Up) 
                            percent = ((100 - splitter.UpPercent) / 100);
                        else
                            percent = ((100 - splitter.DownPercent) / 100);
                            flow *= percent;
                    }
                    else if(this.Parent is Merger)
                    {
                        Merger merger = this.Parent as Merger;
                        flow = merger.Up.Flow + merger.Down.Flow;
                    }
                    return flow;
                }
                if (this.Parent is Pump)
                    return (this.Parent as Pump).Flow;
                return 0;
            }
        }

        public ConnectionZone(Point margin, Element parent, bool isInflow)
        {
            this.Margin = margin;
            this.Parent = parent;
            this.isInFlow = isInflow;
        }

        public static Path PathFromTo(ConnectionZone from, ConnectionZone to)
        {
            return new Path(from, to);
        }

        public Path GetPathTo(ConnectionZone to)
        {
            return PathFromTo(this, to);
        }

        public static implicit operator Point(ConnectionZone connection)
        {
            return new Point(connection.Position.X + connection.Width / 2, connection.Position.Y + connection.Height / 2);
        }


        public class Path
        {
            public static List<Path> All { get { return Main.AllPaths; } }
            public ConnectionZone From { get; private set; }
            public ConnectionZone To { get; private set; }

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
            public List<Point> MidPoints = new List<Point>();

            public Path(ConnectionZone from, ConnectionZone to)
            {
                this.From = from;
                this.To = to;
            }

            private bool isAdded = false;

            public void Add()
            {
                if (isAdded) return;
                ConnectionZone.Path.All.Add(this);
                this.isAdded = true;
                this.To.Previous = this.From;
            }

            public void Remove()
            {
                if (!isAdded) return;
                ConnectionZone.Path.All.Remove(this);
                this.isAdded = false;
                this.To.Previous = null;
            }

            public event System.Action OnCreated = () => { };
            public event System.Action OnAdjusted = () => { };

            private System.Threading.Thread activeAdjuster;
            private bool IsAdjusting { get { return activeAdjuster != null && activeAdjuster.ThreadState == System.Threading.ThreadState.Running; } }

            bool isNew = true;

            object threadLock = new object();

            public void Adjust(bool refresh = false)
            {
                lock (threadLock)
                {
                    if (IsAdjusting) activeAdjuster.Abort();
                }

                activeAdjuster = new System.Threading.Thread(() =>
                {
                    try
                    {
                        Point start = this.From;
                        Point end = this.To;

                        lock (threadLock)
                        {
                            this.MidPoints.Clear();
                        }
                        HashSet<Collision> lastCollisions = new HashSet<Collision>();
                        while (true)
                        {
                            Collision collision = Collision.FindBetween(start, end, this.From, this.To, ref lastCollisions, Element.AllElements);
                            if (!collision) break;
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
