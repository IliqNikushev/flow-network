using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Flow_Network
{
    class Collision
    {
        public Point CloserMargin { get; private set; }
        public Point Intersection { get; private set; }

        public Element Element{get; private set;}

        public Point LeftCloserMargin { get; private set; }
        public Point RightCloserMargin { get; private set; }
        public Point InverseCloserMargin { get; private set; }

        public bool SenderIsUp { get; private set; }
        public bool SenderIsOnLeft { get; private set; }
        public bool TargetIsOnLeft { get; private set; }
        public bool TargetIsUp { get; private set; }

        public Collision(Point intersection, Element element, Point from, Point to)//, Element element, Point end)
        {

            bool left = from.X < element.Center.X;
            bool up = from.Y < element.Center.Y;

            bool targetLeft = to.X < element.Center.X;
            bool targetUp = to.Y < element.Center.Y;

            this.SenderIsOnLeft = left;
            this.SenderIsUp = up;

            this.TargetIsUp = targetUp;
            this.TargetIsOnLeft = targetLeft;

            this.Element = element;
            this.Intersection = intersection;
            Point closerMargin = new Point();
            if (left)
                if (up)
                {
                    closerMargin = element.D;
                    LeftCloserMargin = element.A;
                    RightCloserMargin = element.C;
                    InverseCloserMargin = element.B;
                }
                else
                {
                    closerMargin = element.A;
                    this.LeftCloserMargin = element.D;
                    this.RightCloserMargin = element.B;
                    this.InverseCloserMargin = element.C;
                }
            else
                if (up)
                {
                    closerMargin = element.C;
                    this.LeftCloserMargin = element.D;
                    this.RightCloserMargin = element.B;
                    this.InverseCloserMargin = element.A;
                }
                else
                {
                    closerMargin = element.B;
                    this.LeftCloserMargin = element.A;
                    this.RightCloserMargin = element.D;
                    this.InverseCloserMargin = element.C;
                }
            this.CloserMargin = closerMargin;

            //this.CloserMargin = Offset(CloserMargin, element, targetLeft, targetUp);
            //this.LeftCloserMargin = Offset(LeftCloserMargin, element, targetLeft, targetUp);
            //this.RightCloserMargin = Offset(RightCloserMargin, element, targetLeft, targetUp);
            //this.InverseCloserMargin = Offset(InverseCloserMargin, element, targetLeft, targetUp);
        }

        private Point Offset(Point closerMargin, Element element, bool targetLeft, bool targetUp)
        {
            int xD = targetLeft ? -1 : 1;
            int yD = targetUp ? -1 : 1;

            if (xD < 0) xD = -1;
            else xD = 1;
            if (yD < 0) yD = -1;
            else yD = 1;

            xD *= 5;
            yD *= 5;
            closerMargin.X += xD;
            closerMargin.Y += yD;

            return closerMargin;
        }

        public static implicit operator bool(Collision collision)
        {
            return collision != null;
        }

        public static Collision FindBetween(Point from, Point to, ConnectionZone fromEl, ConnectionZone toEl,ref HashSet<Collision> lastCollision, List<Element> elements = null)
        {
            if (elements == null) elements = Element.AllElements;

            double deltaMin = double.MaxValue;
            Collision minimum = null;

            foreach (Element element in elements)
            {
                if (element == fromEl.Parent) continue;
                if (element == toEl.Parent) continue;

                Point intersection = new Point();
                bool interects = Intersects(element.A, element.B, from, to, out intersection);
                if(!interects)
                    interects = Intersects(element.A, element.D, from, to, out intersection);
                if (!interects)
                    interects = Intersects(element.C, element.D, from, to, out intersection);
                if (!interects)
                   interects = Intersects(element.C, element.B, from, to, out intersection);
                if (interects)
                //bool interects = Intersects(from, to, element);
                if (interects)
                {
                    double deltaX = element.Center.X - fromEl.Parent.Center.X;
                    double deltaY = element.Center.Y - fromEl.Parent.Center.Y;
                    double delta = deltaX * deltaX + deltaY * deltaY;
                    delta = Math.Sqrt(delta);
                    Collision collision = new Collision(intersection, element, from, to);
                    if (lastCollision.FirstOrDefault(x => x.Element == element) != null) continue;
                    
                    if (deltaMin > delta)
                    {
                        deltaMin = delta;
                        minimum = collision;
                    }
                }
            }
            if(minimum != null)
                lastCollision.Add(minimum);
            return minimum;
        }

        static bool Intersects(Point p1, Point p2, Element r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

        static bool Intersects(Point a1, Point a2, Point b1, Point b2, out Point intersection)
        {
            intersection = Point.Empty;

            Point b = new Point(a2.X - a1.X, a2.Y - a1.Y);
            Point d = new Point(b2.X - b1.X, b2.Y - b1.Y);
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Point c = new Point(b1.X - a1.X, b1.Y - a1.Y);
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = new Point((int)(a1.X + t * b.X), (int)(a1.Y + t * b.Y));

            return true;
        }
    }
}