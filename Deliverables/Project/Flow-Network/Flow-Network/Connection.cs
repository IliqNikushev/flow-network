using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Flow_Network
{
    public class Connection
    {
        #region old
        /*
        public Connection()
        {
            throw new System.NotImplementedException();
        }

        public int X
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Y
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Element Owner
        {
            get
            {
                throw new System.NotImplementedException();
            }
            private set
            {
            }
        }

        public Element To
        {
            get
            {
                throw new System.NotImplementedException();
            }
            private set
            {
            }
        }

        public int Flow
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public bool CanConnectTo(Element e)
        {
            throw new System.NotImplementedException();
        }

        public void ConnectTo(Element e)
        {
            throw new System.NotImplementedException();
        }
        */
        #endregion
        public class Path
        {
            public Connection From{get; private set;}
            public Connection To{get; private set;}

            public List<Point> PathPoints
            {
                get
                {
                    List<Point> result = new List<Point>();
                    result.Add(From.Parent.Center);
                    result.AddRange(MidPoints);
                    result.Add(To.Parent.Center);
                    return result;
                }
            }
            public List<Point> MidPoints = new List<Point>();

            public Path(Connection from, Connection to)
            {
                this.From = from;
                this.To = to;
            }

            public event System.Action OnAdjusted = () => { };

            private System.Threading.Thread activeAdjuster;
            private bool IsAdjusting { get { return activeAdjuster != null && activeAdjuster.ThreadState == System.Threading.ThreadState.Running; } }

            public void Adjust()
            {
                if (IsAdjusting) activeAdjuster.Abort();
                Point start = this.From;
                Point end = this.To;

               // activeAdjuster = new System.Threading.Thread(() =>
               // {
                    try
                    {
                        Collision.LastCollision = null;
                        bool connected = false;
                        while (!connected)
                        {
                            Collision collision = Collision.FindBetween(start, end, this.From, this.To);
                            if (!collision) connected = true;
                            else
                            {

                                //{
                                //    //bool left = start.X < end.X;
                                //    //bool up = start.Y < end.Y;

                                //    if (!this.MidPoints.Contains(collision.LeftCloserMargin))
                                //        margin = collision.LeftCloserMargin;
                                //    else if (!this.MidPoints.Contains(collision.RightCloserMargin))
                                //        margin = collision.RightCloserMargin;
                                //    else if (!this.MidPoints.Contains(collision.InverseCloserMargin))
                                //        margin = collision.InverseCloserMargin;
                                //}

                                Point margin = collision.CloserMargin;

                                //todo                                        
                                //check if between width WHENEVER 3x midpoints

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
                        
                        OnAdjusted();
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                    }
                //});
                //activeAdjuster.Start();
            }
        }

        public Element Parent { get; private set; }

        public Point Margin { get; private set; }

        public Point Position
        {
            get
            {
                return new Point(this.Margin.X + this.Parent.X, this.Margin.Y + this.Parent.Y);
            }
        }

        public Path PathWay { get; private set; }

        public Connection(Point margin, Element parent)
        {
            this.Margin = margin;
            this.Parent = parent;
        }

        public void ConnectTo(Connection connection)
        {
            this.PathWay = new Path(this, connection);
        }

        public static Path PathFromTo(Connection from, Connection to)
        {
            return new Path(from, to);
        }

        public Path GetPathTo(Connection to)
        {
            return PathFromTo(this, to);
        }

        public static implicit operator Point(Connection connection)
        {
            return connection.Parent.Center;
        }
    }
}
