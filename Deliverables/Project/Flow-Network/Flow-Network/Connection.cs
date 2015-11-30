﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Flow_Network
{
    public class ConnectionZone
    {
        public class Path
        {
            public static List<Path> All { get { return Main.AllPaths; } }
            public ConnectionZone From{get; private set;}
            public ConnectionZone To{get; private set;}

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

            public Path(ConnectionZone from, ConnectionZone to)
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

                activeAdjuster = new System.Threading.Thread(() =>
               {
                    try
                    {
                        Collision lastCollision = null;
                        while (true)
                        {
                            
                            Collision collision = Collision.FindBetween(start, end, this.From, this.To, ref lastCollision, Element.AllElements);
                            if (!collision) break;
                            else
                            {
                                Point margin = collision.CloserMargin;

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
                });
                activeAdjuster.Start();
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

        public ConnectionZone(Point margin, Element parent)
        {
            this.Margin = margin;
            this.Parent = parent;
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
            return connection.Parent.Center;
        }
    }
}
