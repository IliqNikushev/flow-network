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
        public Point CloserMargin;

        public Collision(Point intersection, Point closerMargin)
        {
        }

        public static implicit operator bool(Collision collision)
        {
            return collision != null;
        }

        public static Collision FindBetween(Point from, Point to, List<Elem> elements = null)
        {
            if (elements == null) elements = Elem.AllElements;

            foreach (Elem element in elements)
            {
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
                    Point closerMargin = new Point();

                    bool left = from.X < element.X;;

                    bool up = from.Y < element.Y;

                    if (left)
                        if (up)
                            closerMargin = element.D;
                        else
                            closerMargin = element.A;
                    else
                        if (up)
                            closerMargin = element.C;
                        else
                            closerMargin = element.B;

                    Collision collision = new Collision(intersection, closerMargin);

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