using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Flow_Network
{
    /// <summary>Zone in which an element can be connected with a path</summary>
    public class ConnectionZone : IconDrawable
    {
        /// <summary>Constant Default size for width and height when drawn</summary>
        public static readonly Point DefaultSize = new Point(20, 20);
        private bool isInFlow;

        public bool IsConnected { get { return this.ConnectedZone != null; } }
        public bool IsFree { get { return !this.IsConnected; } }

        /// <summary>Element that the zone belongs to</summary>
        public Element Parent { get; private set; }

        /// <summary>Distance from the parent's X,Y</summary>
        public Point Margin { get; set; }

        /// <summary>returns DefaultSize.X</summary>
        public override int Width { get { return DefaultSize.X; } }
        /// <summary>returns DefaultSize.Y</summary>
        public override int Height { get { return DefaultSize.Y; } }

        /// <summary>Returns the parent's location + current (X,Y)</summary>
        public override Point Location
        {
            get
            {
                return new Point(this.Margin.X + this.Parent.X, this.Margin.Y + this.Parent.Y);
            }
        }

        public bool IsInFlow
        {
            get { return isInFlow; }
        }

        public bool IsOutFlow
        {
            get { return !isInFlow; }
        }

        public bool FlowIsSameAs(ConnectionZone other)
        {
            if (other == null) return false;
            return this.isInFlow == other.isInFlow;
        }

        /// <summary>If connected, returns the zone prior to the current on the path they are connected</summary>
        public ConnectionZone ConnectedZone { get; set; }

        /// <summary>returns the current flow that is passing through the zone, by going up the connected path it lays on</summary>
        public float Flow
        {
            get
            {
                if (this.Parent is PumpElement)
                    return (this.Parent as PumpElement).Flow;

                if (ConnectedZone != null)
                {
                    float flow = ConnectedZone.Flow;

                    if (this.Parent is SplitterElement)
                        flow *= 0.5f;
                    else if (this.Parent is AdjustableSplitter)
                    {
                        AdjustableSplitter splitter = this.Parent as AdjustableSplitter;
                        float percent = 1;
                        if (this == splitter.Up)
                            percent = ((100 - splitter.UpFlowPercent) / 100);
                        else
                            percent = ((100 - splitter.DownFlowPercent) / 100);
                        flow *= percent;
                    }
                    else if (this.Parent is MergerElement)
                    {
                        MergerElement merger = this.Parent as MergerElement;
                        flow = merger.Up.Flow + merger.Down.Flow;
                    }
                    return flow;
                }
                return 0;
            }
        }

        public ConnectionZone(int x, int y, Element parent, bool isInFlow) : this(new Point(x, y), parent, isInFlow) { }

        public ConnectionZone(Point margin, Element parent, bool isInflow)
        {
            this.Parent = parent;
            this.Margin = margin;
            this.isInFlow = isInflow;
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
        public class Path : Drawable
        {
            public int Width { get { return DEFAULT_WIDTH; } }
            public const int DEFAULT_WIDTH = 6;
            public delegate void FlowAlteredEvent(Path path, float previous, float current);
            /// <summary>Returns all paths defineed in the Main form</summary>
            public static List<Path> All { get { return Main.AllPaths; } }
            public ConnectionZone From { get; private set; }
            public ConnectionZone To { get; private set; }

            public bool PointsHaveChanged { get; private set; }

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

            public float Flow { get { return this.From.Flow; } }

            /// <summary>Called when the max flow has been altered</summary>
            public event FlowAlteredEvent OnMaxFlowChanged = (x, y, z) => { };

            /// <summary>Returns a path starting at the FROM zone to the TO zone, with all midpoints inbetween</summary>
            public List<Point> PathPoints
            {
                get
                {
                    List<Point> result = new List<Point>();
                    result.Add(From);
                    lock (adjusterThreadLock)
                    {
                        result.AddRange(MidPoints);
                    }
                    result.Add(To);
                    return result;
                }
            }

            /// <summary>Points the path HAS to go to before reaching the TO zone</summary>
            public List<PathMidPointDrawable> UserDefinedMidPoints = new List<PathMidPointDrawable>();

            public void AddUserMidPoint(Point pt)
            {
                AddUserMidPoint(pt.X, pt.Y);
            }

            public void AddUserMidPoint(int x, int y)
            {
                this.UserDefinedMidPoints.Add(new PathMidPointDrawable(x, y, this));
            }

            public List<Point> PreviousPointsToGoThrough = new List<Point>();

            /// <summary>Points the path will go through before reaching the TO zone</summary>
            private List<Point> MidPoints = new List<Point>();

            public Path(ConnectionZone from, ConnectionZone to)
            {
                this.IsNew = true;
                if (from.isInFlow == to.isInFlow) throw new ArgumentException("Two connection zones with same flow type connected");
                if (from.IsOutFlow)
                {
                    this.From = from;
                    this.To = to;
                }
                else
                {
                    this.From = to;
                    this.To = from;
                }
            }

            private bool isAdded = false;

            /// <summary>Adds to the All collection of paths if not added</summary>
            public void AddToSystem()
            {
                if (isAdded) return;
                this.To.DrawState = Flow_Network.DrawState.Blocking;
                this.From.DrawState = Flow_Network.DrawState.Blocking;
                ConnectionZone.Path.All.Add(this);
                this.isAdded = true;
                this.To.ConnectedZone = this.From;
                this.From.ConnectedZone = this.To;
            }

            /// <summary>Removes from the All collection of paths if present</summary>
            public void RemoveFromSystem()
            {
                if (!isAdded) return;
                this.To.DrawState = Flow_Network.DrawState.Normal;
                this.From.DrawState = Flow_Network.DrawState.Normal;
                ConnectionZone.Path.All.Remove(this);
                this.isAdded = false;
                this.To.ConnectedZone = null;
                this.From.ConnectedZone = null;
            }

            public PathMidPointDrawable GetClosestMidPointTo(Point position)
            {
                PathMidPointDrawable p = null;

                foreach (PathMidPointDrawable point in this.UserDefinedMidPoints)
                {
                    float deltaX = point.X - position.X;
                    float deltaY = point.Y - position.Y;
                    deltaX *= deltaX;
                    deltaY *= deltaY;
                    float distance = (float)Math.Sqrt(deltaX + deltaY);
                    if (distance <= this.Width)
                    {
                        p = point;
                        break;
                    }
                }

                return p;
            }

            /// <summary>Called when the path is adjusted for the first time</summary>
            public event System.Action OnCreated = () => { };
            /// <summary>Called when the path has been adjusted</summary>
            public event System.Action OnAdjusted = () => { };

            /// <summary>Called when the path has been adjusted</summary>
            public event System.Action OnBeforeAdjusted = () => { };

            private System.Threading.Thread activeAdjuster;
            private bool IsAdjusting { get { return activeAdjuster != null && activeAdjuster.ThreadState == System.Threading.ThreadState.Running; } }

            public bool IsNew { get; private set; }

            private object adjusterThreadLock = new object();

            public void LockForRender(Action a)
            {
                lock (adjusterThreadLock)
                {
                    a();
                }
            }

            /// <summary>Calculates the new midpoints based on the position of the Elements.All and the user defined mid points
            /// The process is executed on a different thread to not hold up the program</summary>
            /// <param name="refresh">If set to true, will call OnAdjusted</param>
            public void Adjust(bool refresh = false, Action onDone = null)
            {
                lock (adjusterThreadLock)
                {
                    if (IsAdjusting) activeAdjuster.Abort();
                }

                List<Point> pointsToGoThrough = new List<Point>();
                pointsToGoThrough.Add(this.From);
                pointsToGoThrough.AddRange(UserDefinedMidPoints.Select(x=>x.Location));
                pointsToGoThrough.Add(this.To);

                if (!IsNew)
                    if (refresh)
                        lock(adjusterThreadLock)
                            OnBeforeAdjusted();

                List<Point> currentMidPoints = new List<Point>(this.MidPoints);

                activeAdjuster = new System.Threading.Thread(() =>
                {
                    try
                    {
                        lock (adjusterThreadLock)
                        {
                            this.MidPoints.Clear();

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
                                    Point location = start;
                                    location.X += this.UserDefinedMidPoints[currentPointStartIndex - 1].Width / 2;
                                    location.Y += this.UserDefinedMidPoints[currentPointStartIndex - 1].Height / 2;
                                    this.MidPoints.Add(location);
                                    elementsAlreadyCollided.Clear();
                                }
                                else
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

                            PointsHaveChanged = false;
                            List<Point> currentPoints = this.PathPoints;
                            if (PreviousPointsToGoThrough.Count == currentPoints.Count)
                                for (int i = 0; i < currentPoints.Count; i++)
                                {
                                    if (currentPoints[i] != PreviousPointsToGoThrough[i])
                                    {
                                        PointsHaveChanged = true;
                                        break;
                                    }
                                }
                            else
                                PointsHaveChanged = true;

                            this.PreviousPointsToGoThrough.Clear();
                            this.PreviousPointsToGoThrough.AddRange(this.PathPoints);

                            if (IsNew)
                            {
                                OnCreated();
                                IsNew = false;
                            }
                            else
                                if (refresh)
                                    OnAdjusted();
                        }
                        if (onDone != null) onDone();

                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                    }
                });
                activeAdjuster.Start();
            }

            static Pen onHoveredPen = new Pen(Color.Orange, Path.DEFAULT_WIDTH);
            static Pen onDeletePen = new Pen(Color.Red, Path.DEFAULT_WIDTH);
            static Pen onNormalPen = new Pen(Color.Black, Path.DEFAULT_WIDTH);

            protected override void OnDraw(Graphics graphics, Color backgroundColor)
            {
                lock (adjusterThreadLock)
                {
                    Pen currentPen = onNormalPen;
                    switch (this.DrawState)
                    {
                        case DrawState.Normal:
                            currentPen = onNormalPen;
                            break;
                        case DrawState.Hovered:
                            currentPen = onHoveredPen;
                            break;
                        case DrawState.Delete:
                            currentPen = onDeletePen;
                            break;
                        case DrawState.Blocking:
                            currentPen = onDeletePen;
                            break;
                        default:
                            break;
                    }
                    Point previousPoint = this.From;
                    foreach (Point currentPoint in this.PathPoints)
                    {
                        if (previousPoint == currentPoint) continue;
                        graphics.DrawLine(currentPen, previousPoint, currentPoint);
                        previousPoint = currentPoint;
                    }

                    foreach (PathMidPointDrawable point in this.UserDefinedMidPoints)
                    {
                        point.Draw(graphics, backgroundColor);
                    }
                }
            }

            protected override void OnDrawClear(Graphics g, Color backgroundColor)
            {
                lock (adjusterThreadLock)
                {
                    if (this.PreviousPointsToGoThrough.Count == 0) this.PreviousPointsToGoThrough.AddRange(this.PathPoints);
                    Pen onClearPen = new Pen(backgroundColor, Path.DEFAULT_WIDTH);
                    Brush onClearBrush = new SolidBrush(backgroundColor);
                    Point previousPoint = this.PreviousPointsToGoThrough[0];
                    foreach (Point currentPoint in this.PreviousPointsToGoThrough)
                    {
                        if (previousPoint == currentPoint) continue;
                        g.DrawLine(onClearPen, previousPoint, currentPoint);
                        previousPoint = currentPoint;
                    }

                    foreach (PathMidPointDrawable point in this.UserDefinedMidPoints)
                    {
                        point.DrawClear(g, backgroundColor);
                    }
                }
            }
        }
    }
}