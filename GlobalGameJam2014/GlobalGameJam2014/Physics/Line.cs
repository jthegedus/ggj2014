// -----------------------------------------------------------------------
// <copyright file="Line.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace GGJ2014.Physics
{
    /// <summary>
    /// Line data for collision.
    /// </summary>
    public struct Line : IEquatable<Line>
    {
        public Vector2 P1;
        public Vector2 P2;

        public static bool operator ==(Line lhs, Line rhs)
        {
            return ((Vector2.Distance(lhs.P1, rhs.P1) < 0.1 && Vector2.Distance(lhs.P2, rhs.P2) < 0.1) ||
                   (Vector2.Distance(lhs.P1, rhs.P2) < 0.1 && Vector2.Distance(lhs.P2, rhs.P1) < 0.1));
        }

        public static bool operator !=(Line lhs, Line rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(Line other)
        {
            return this == other;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return this.P1.ApproxEquals(point) || this.P2.ApproxEquals(point);
        }

        public Vector2? SharedPoint(Line l)
        {
            if (this.P1.ApproxEquals(l.P1))
            {
                return this.P1;
            }
            else if (this.P1.ApproxEquals(l.P1))
            {
                return this.P1;
            }
            else if (this.P2.ApproxEquals(l.P1))
            {
                return this.P2;
            }
            else if (this.P2.ApproxEquals(l.P2))
            {
                return this.P2;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Line segment intersection
        /// http://paulbourke.net/geometry/lineline2d/.
        /// </summary>
        /// <param name="l">The line to intersect with.</param>
        /// <returns>The intersection point of hte lines, possibly null.</returns>
        public Vector2? IntersectsLineSegment(Line l)
        {
            float denom = ((l.P2.Y - l.P1.Y) * (this.P2.X - this.P1.X) - (l.P2.X - l.P1.X) * (this.P2.Y - this.P1.Y));

            if (denom == 0)
            {
                //if (((l.P2.X - l.P1.X) * (this.P1.Y - l.P1.Y) - (l.P2.Y - l.P1.Y) * (this.P1.X - l.P1.X)) == 0)
                //{

                //    return new Vector2(float.NaN, float.NaN);
                //}
                return null;
            }

            float ua = ((l.P2.X - l.P1.X) * (this.P1.Y - l.P1.Y) - (l.P2.Y - l.P1.Y) * (this.P1.X - l.P1.X)) / denom;
            float ub = ((this.P2.X - this.P1.X) * (this.P1.Y - l.P1.Y) - (this.P2.Y - this.P1.Y) * (this.P1.X - l.P1.X)) / denom;

            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            {
                float x = this.P1.X + ua * (this.P2.X - this.P1.X);
                float y = this.P1.Y + ua * (this.P2.Y - this.P1.Y);

                return new Vector2(x, y);
            }
            else
            {
                return null;
            }
        }

        public Vector2? IntersectsInfiniteLine(Line l)
        {
            float denom = ((l.P2.Y - l.P1.Y) * (this.P2.X - this.P1.X) - (l.P2.X - l.P1.X) * (this.P2.Y - this.P1.Y));

            if (denom == 0)
            {
                return null;
            }

            float ua = ((l.P2.X - l.P1.X) * (this.P1.Y - l.P1.Y) - (l.P2.Y - l.P1.Y) * (this.P1.X - l.P1.X)) / denom;
            float ub = ((this.P2.X - this.P1.X) * (this.P1.Y - l.P1.Y) - (this.P2.Y - this.P1.Y) * (this.P1.X - l.P1.X)) / denom;

            float x = this.P1.X + ua * (this.P2.X - this.P1.X);
            float y = this.P1.Y + ua * (this.P2.Y - this.P1.Y);

            return new Vector2(x, y);
        }

        public bool PointOnLineSegment(Vector2 point)
        {
            float t0 = (point.X - this.P1.X) / (this.P2.X - this.P1.X);
            float t1 = (point.Y - this.P1.Y) / (this.P2.Y - this.P1.Y);

            if (Math.Abs(t0 - t1) < 0.01)
            {
                if (t0 > 0 && t0 < 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// http://www.t3hprogrammer.com/research/line-circle-collision/tutorial#TOC-Line-Line-Intersection
        /// </summary>
        /// <param name="otherPoint"></param>
        /// <returns></returns>
        public Vector2 ClosestPointOnInfiniteLine(Vector2 otherPoint)
        {
            float A1 = this.P2.Y - this.P1.Y;
            float B1 = this.P1.X - this.P2.X;

            float C1 = (this.P2.Y - this.P1.Y) * this.P1.X + (this.P1.X - this.P2.X) * this.P1.Y;
            float C2 = -B1 * otherPoint.X + A1 * otherPoint.Y;

            float det = A1 * A1 + B1 * B1;

            float cx, cy;

            if (det != 0)
            {
                cx = (A1 * C1 - B1 * C2) / det;
                cy = (A1 * C2 - -B1 * C1) / det;
            }
            else
            {
                cx = otherPoint.X;
                cy = otherPoint.Y;
            }

            return new Vector2(cx, cy);
        }

        /*
        public Line? JoinLine(Line l, Vector2 sharedPoint)
        {
            Vector2 first;
            Vector2 second;

            if (sharedPoint.ApproxEquals(this.P1))
            {
                first = this.P2;
            }
            else
            {
                first = this.P1;
            }

            if (sharedPoint.ApproxEquals(l.P1))
            {
                second = l.P2;
            }
            else
            {
                second = l.P1;
            }

            if (Math.Acos(Vector2.Dot(first - sharedPoint, second - sharedPoint)) <= MathHelper.PiOver2)
            {
#warning This normal is probably not in the right direction
                return new Line() { P1 = first, P2 = second };
            }

            return null;
        }
        */

        /*
        public bool PointOnLeft(Vector2 point)
        {
            return Math.Acos(Vector2.Dot(Vector2.Normalize(point - this.P1), this.Normal)) > MathHelper.PiOver2;
        }
        */
    }
}
