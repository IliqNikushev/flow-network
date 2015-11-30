using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using   System.Drawing;

namespace Flow_Network
{
    class Conn
    {
        public class Path
        {
            public Conn From{get; private set;}
            public Conn To{get; private set;}

            public List<Point> Path
            {
                get
                {
                    List<Point> result = new List<Point>();
                    result.Add(From.Position);
                    result.AddRange(MidPoints);
                    result.Add(To.Position);
                    return result;
                }
            }
            public List<Point> MidPoints = new List<Point>();

            public Path(Conn from, Conn to)
            {
                this.From = from;
                this.To = to;
                Adjust();
            }

            public event Action OnAdjusted = () => { };

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
                        bool connected = false;
                        while (!connected)
                        {
                            Collision collision = Collision.FindBetween(start, end);
                            if (!collision) connected = true;
                            else
                                this.MidPoints.Add(collision.CloserMargin);
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

        public Elem Parent { get; private set; }

        public Point Margin { get; private set; }

        public Point Position
        {
            get
            {
                return new Point(this.Margin.X + this.Parent.Position.X, this.Margin.Y + this.Parent.Position.Y);
            }
        }

        public Path Connection { get; private set; }

        public Conn(Point margin, Elem parent)
        {
            this.Margin = margin;
            this.Parent = parent;
        }

        public void ConnectTo(Conn connection)
        {
            this.Connection = new Path(this, connection);
        }

        public static Path PathFromTo(Conn from, Conn to)
        {
            return new Path(from, to);
        }

        public Path GetPathTo(Conn to)
        {
            return PathFromTo(this, to);
        }

        public static implicit operator Point(Conn connection)
        {
            return connection.Position;
        }
    }
}
