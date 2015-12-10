using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Flow_Network
{
    /// <summary>Helper class for ConnectionZone.Path to find a collision with an element</summary>
    class Collision
    {
        public Element Element{get; private set;}

        public bool SenderIsUp { get; private set; }
        public bool SenderIsOnLeft { get; private set; }
        public bool TargetIsOnLeft { get; private set; }
        public bool TargetIsUp { get; private set; }

        public Collision(Element element, Point from, Point to)
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

                bool interects = Intersects(element.A, element.B, from, to);
                if(!interects)
                    interects = Intersects(element.A, element.D, from, to);
                if (!interects)
                    interects = Intersects(element.C, element.D, from, to);
                if (!interects)
                   interects = Intersects(element.C, element.B, from, to);
                if (interects)
                {
                    double deltaX = element.Center.X - fromEl.Parent.Center.X;
                    double deltaY = element.Center.Y - fromEl.Parent.Center.Y;
                    double delta = deltaX * deltaX + deltaY * deltaY;
                    delta = Math.Sqrt(delta);
                    Collision collision = new Collision(element, from, to);
                    if (lastCollision.FirstOrDefault(x => x.Element == element) != null) continue;
                    
                    //find closest collision
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

        static bool Intersects(Point a1, Point a2, Point b1, Point b2)
        {
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

            //intersection = new Point((int)(a1.X + t * b.X), (int)(a1.Y + t * b.Y));

            return true;
        }
    }
}