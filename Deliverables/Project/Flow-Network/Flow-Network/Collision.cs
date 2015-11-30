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
        public static Collision LastCollision;

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

        public static Collision FindBetween(Point from, Point to, Connection fromEl, Connection toEl, List<Element> elements = null)
        {
            if (elements == null) elements = Element.AllElements;

            foreach (Element element in elements)
            {
                if (element == fromEl.Parent) continue;
                if (element == toEl.Parent) continue;

                Point intersection;
                bool interects = Intersects(element.A, element.B, from, to, out intersection);
                if(!interects)
                    interects = Intersects(element.A, element.D, from, to, out intersection);
                if (!interects)
                    interects = Intersects(element.C, element.D, from, to, out intersection);
                if (!interects)
                    interects = Intersects(element.C, element.B, from, to, out intersection);
                if (interects)
                {
                    Collision previous = LastCollision;

                    Collision collision = new Collision(intersection, element, from, to);
                    LastCollision = collision;
                    if (previous != null)
                        if (collision.Element == previous.Element) continue;
                    return collision;
                }
            }

            return null;
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