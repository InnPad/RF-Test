using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing
{
    public class Interception
    {
        public static readonly Interception None = new Interception
        {
            Type = InterceptionTypes.None
        };

        private static Point[] _empty = new Point[0];
        private Point[] _points = null;

        public Point[] Points
        {
            get { return _points ?? _empty; }
            private set { _points = value; }
        }

        public InterceptionTypes Type
        {
            get;
            private set;
        }

        /*****
        * 
        *   Bezout
        *
        *   This code is based on MgcIntr2DElpElp.cpp written by David Eberly.  His
        *   code along with many other excellent examples are avaiable at his site:
        *   http://www.magic-software.com
        *
        *****/
        public static Polynomial Bezout(double[] e1, double[] e2)
        {
            var AB = e1[0] * e2[1] - e2[0] * e1[1];
            var AC = e1[0] * e2[2] - e2[0] * e1[2];
            var AD = e1[0] * e2[3] - e2[0] * e1[3];
            var AE = e1[0] * e2[4] - e2[0] * e1[4];
            var AF = e1[0] * e2[5] - e2[0] * e1[5];
            var BC = e1[1] * e2[2] - e2[1] * e1[2];
            var BE = e1[1] * e2[4] - e2[1] * e1[4];
            var BF = e1[1] * e2[5] - e2[1] * e1[5];
            var CD = e1[2] * e2[3] - e2[2] * e1[3];
            var DE = e1[3] * e2[4] - e2[3] * e1[4];
            var DF = e1[3] * e2[5] - e2[3] * e1[5];
            var BFpDE = BF + DE;
            var BEmCD = BE - CD;

            return new Polynomial(
                AB * BC - AC * AC,
                AB * BEmCD + AD * BC - 2 * AC * AE,
                AB * BFpDE + AD * BEmCD - AE * AE - 2 * AC * AF,
                AB * DF + AD * BFpDE - 2 * AE * AF,
                AD * DF - AF * AF
            );
        }

        public static Interception IntersectCircleCircle(Point c1, double r1, Point c2, double r2)
        {
            var points = new List<Point>();

            // Determine minimum and maximum radii where circles can intersect
            var r_max = r1 + r2;
            var r_min = Math.Abs(r1 - r2);

            // Determine actual distance between circle circles
            var c_dist = c1.DistanceFrom(c2);

            if (c_dist > r_max)
            {
                return new Interception
                {
                    Type = InterceptionTypes.Outside
                };
            }
            else if (c_dist < r_min)
            {
                return new Interception
                {
                    Type = InterceptionTypes.Inside
                };
            }
            else
            {
                var a = (r1 * r1 - r2 * r2 + c_dist * c_dist) / (2 * c_dist);
                var h = Math.Sqrt(r1 * r1 - a * a);
                var p = c1.Lerp(c2, a / c_dist);
                var b = h / c_dist;

                points.Add(
                    new Point
                        {
                            X = p.X - b * (c2.Y - c1.Y),
                            Y = p.Y + b * (c2.X - c1.X)
                        }
                );
                points.Add(
                    new Point
                    {
                        X = p.X + b * (c2.Y - c1.Y),
                        Y = p.Y - b * (c2.X - c1.X)
                    }
                );

                return new Interception
                {
                    Points = points.ToArray(),
                    Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
                };
            }
        }

        public static Interception IntersectCircleEllipse(Point cc, double r, Point ec, double rx, double ry)
        {
            return IntersectEllipseEllipse(cc, r, r, ec, rx, ry);
        }

        public static Interception IntersectCircleLine(Point c, double r, Point a1, Point a2)
        {
            var points = new List<Point>();
            var a = (a2.X - a1.X) * (a2.X - a1.X) +
                     (a2.Y - a1.Y) * (a2.Y - a1.Y);
            var b = 2 * ((a2.X - a1.X) * (a1.X - c.X) +
                           (a2.Y - a1.Y) * (a1.Y - c.Y));
            var cc = c.X * c.X + c.Y * c.Y + a1.X * a1.X + a1.Y * a1.Y -
                     2 * (c.X * a1.X + c.Y * a1.Y) - r * r;
            var deter = b * b - 4 * a * cc;

            if (deter < 0)
            {
                // Outside
            }
            else if (deter == 0)
            {
                // Tangent
                // NOTE: should calculate this point
            }
            else
            {
                var e = Math.Sqrt(deter);
                var u1 = (-b + e) / (2 * a);
                var u2 = (-b - e) / (2 * a);

                if ((u1 < 0 || u1 > 1) && (u2 < 0 || u2 > 1))
                {
                    if ((u1 < 0 && u2 < 0) || (u1 > 1 && u2 > 1))
                    {
                        // Outside
                    }
                    else
                    {
                        // Inside
                        points.Add(a1);
                        points.Add(a2);
                    }
                }
                else
                {
                    // Intersection

                    if (0 <= u1 && u1 <= 1)
                        points.Add(a1.Lerp(a2, u1));

                    if (0 <= u2 && u2 <= 1)
                        points.Add(a1.Lerp(a2, u2));
                }
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectCirclePolygon(Point c, double r, Point[] poly)
        {
            var points = new List<Point>();
            var length = poly.Length;

            for (var i = 0; i < length; i++)
            {
                var a1 = poly[i];
                var a2 = poly[(i + 1) % length];

                var inter = IntersectCircleLine(c, r, a1, a2);
                points.AddRange(inter.Points);
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectCircleRectangle(Point c, double r, Point r1, Point r2)
        {
            var min = r1.Min(r2);
            var max = r1.Max(r2);
            var topRight = new Point { X = max.X, Y = min.Y };
            var bottomLeft = new Point { X = min.X, Y = max.Y };

            var inter1 = IntersectCircleLine(c, r, min, topRight);
            var inter2 = IntersectCircleLine(c, r, topRight, max);
            var inter3 = IntersectCircleLine(c, r, max, bottomLeft);
            var inter4 = IntersectCircleLine(c, r, bottomLeft, min);

            var points = new List<Point>();

            points.AddRange(inter1.Points);
            points.AddRange(inter2.Points);
            points.AddRange(inter3.Points);
            points.AddRange(inter4.Points);

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        /*****
        *
        *   IntersectEllipseEllipse
        *   
        *   This code is based on MgcIntr2DElpElp.cpp written by David Eberly.  His
        *   code along with many other excellent examples are avaiable at his site:
        *   http://www.magic-software.com
        *
        *   NOTE: Rotation will need to be added to this function
        *
        *****/
        public static Interception IntersectEllipseEllipse(Point c1, double rx1, double ry1, Point c2, double rx2, double ry2)
        {
            var points = new List<Point>();

            var a = new[]
            {
                ry1*ry1, 0, rx1*rx1, -2*ry1*ry1*c1.X, -2*rx1*rx1*c1.Y,
                ry1*ry1*c1.X*c1.X + rx1*rx1*c1.Y*c1.Y - rx1*rx1*ry1*ry1
            };
            var b = new[]
            {
                ry2*ry2, 0, rx2*rx2, -2*ry2*ry2*c2.X, -2*rx2*rx2*c2.Y,
                ry2*ry2*c2.X*c2.X + rx2*rx2*c2.Y*c2.Y - rx2*rx2*ry2*ry2
            };
            var yPoly = Bezout(a, b);
            var yRoots = yPoly.getRoots();
            var epsilon = 1e-3;
            var norm0 = (a[0] * a[0] + 2 * a[1] * a[1] + a[2] * a[2]) * epsilon;
            var norm1 = (b[0] * b[0] + 2 * b[1] * b[1] + b[2] * b[2]) * epsilon;


            for (var y = 0; y < yRoots.Count; y++)
            {
                var xPoly = new Polynomial(
                    a[0],
                    a[3] + yRoots[y] * a[1],
                    a[5] + yRoots[y] * (a[4] + yRoots[y] * a[2])
                );
                var xRoots = xPoly.getRoots();

                for (var x = 0; x < xRoots.Count; x++)
                {
                    var test =
                        (a[0] * xRoots[x] + a[1] * yRoots[y] + a[3]) * xRoots[x] +
                        (a[2] * yRoots[y] + a[4]) * yRoots[y] + a[5];
                    if (Math.Abs(test) < norm0)
                    {
                        test =
                            (b[0] * xRoots[x] + b[1] * yRoots[y] + b[3]) * xRoots[x] +
                            (b[2] * yRoots[y] + b[4]) * yRoots[y] + b[5];
                        if (Math.Abs(test) < norm1)
                        {
                            points.Add(new Point { X = xRoots[x], Y = yRoots[y] });
                        }
                    }
                }
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectEllipseLine(Point center, double rx, double ry, Point a1, Point a2)
        {
            var type = InterceptionTypes.Interception;
            var points = new List<Point>();
            var origin = new Point { X = a1.X, Y = a1.Y };
            var dir = Vector.FromPoints(a1, a2);
            var diff = (Vector)origin.Subtract(center);
            var mDir = new Vector { X = dir.X / (rx * rx), Y = dir.Y / (ry * ry) };
            var mDiff = new Vector { X = diff.X / (rx * rx), Y = diff.Y / (ry * ry) };

            var a = dir.Dot(mDir);
            var b = dir.Dot(mDiff);
            var c = diff.Dot(mDiff) - 1.0;
            var d = b * b - a * c;

            if (d < 0)
            {
                type = InterceptionTypes.Outside;
            }
            else if (d > 0)
            {
                var root = Math.Sqrt(d);
                var t_a = (-b - root) / a;
                var t_b = (-b + root) / a;

                if ((t_a < 0 || 1 < t_a) && (t_b < 0 || 1 < t_b))
                {
                    if ((t_a < 0 && t_b < 0) || (t_a > 1 && t_b > 1))
                    {
                        type = InterceptionTypes.Outside;
                    }
                    else
                    {
                        type = InterceptionTypes.Inside;
                    }
                }
                else
                {
                    if (0 <= t_a && t_a <= 1)
                        points.Add(a1.Lerp(a2, t_a));
                    if (0 <= t_b && t_b <= 1)
                        points.Add(a1.Lerp(a2, t_b));
                }
            }
            else
            {
                var t = -b / a;
                if (0 <= t && t <= 1)
                {
                    points.Add(a1.Lerp(a2, t));
                }
                else
                {
                    type = InterceptionTypes.Outside;
                }
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = type.Equals(InterceptionTypes.Interception) && points.Count.Equals(0) ? InterceptionTypes.None : type
            };
        }

        public static Interception IntersectEllipsePolygon(Point c, double rx, double ry, Point[] poly)
        {
            var points = new List<Point>();
            var length = poly.Length;

            for (var i = 0; i < length; i++)
            {
                var b1 = poly[i];
                var b2 = poly[(i + 1) % length];
                var inter = IntersectEllipseLine(c, rx, ry, b1, b2);

                points.AddRange(inter.Points);
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }


        public static Interception IntersectEllipseRectangle(Point c, double rx, double ry, Point r1, Point r2)
        {
            var min = r1.Min(r2);
            var max = r1.Max(r2);
            var topRight = new Point { X = max.X, Y = min.Y };
            var bottomLeft = new Point { X = min.X, Y = max.Y };

            var inter1 = IntersectEllipseLine(c, rx, ry, min, topRight);
            var inter2 = IntersectEllipseLine(c, rx, ry, topRight, max);
            var inter3 = IntersectEllipseLine(c, rx, ry, max, bottomLeft);
            var inter4 = IntersectEllipseLine(c, rx, ry, bottomLeft, min);

            var points = new List<Point>();

            points.AddRange(inter1.Points);
            points.AddRange(inter2.Points);
            points.AddRange(inter3.Points);
            points.AddRange(inter4.Points);

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectLineLine(Point a1, Point a2, Point b1, Point b2)
        {
            var type = InterceptionTypes.Interception;
            var points = new List<Point>();
            var ua_t = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
            var ub_t = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
            var u_b = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

            if (u_b != 0)
            {
                var ua = ua_t / u_b;
                var ub = ub_t / u_b;

                if (0 <= ua && ua <= 1 && 0 <= ub && ub <= 1)
                {
                    points.Add(
                        new Point
                        {
                            X = a1.X + ua * (a2.X - a1.X),
                            Y = a1.Y + ua * (a2.Y - a1.Y)
                        }
                    );
                }
                else
                {
                    type = InterceptionTypes.None;
                }
            }
            else
            {
                if (ua_t == 0 || ub_t == 0)
                {
                    type = InterceptionTypes.Coincident;
                }
                else
                {
                    type = InterceptionTypes.Parallel;
                }
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = type.Equals(InterceptionTypes.Interception) && points.Count.Equals(0) ? InterceptionTypes.None : type
            };
        }

        public static Interception IntersectLinePolygon(Point a1, Point a2, Point[] poly)
        {
            var points = new List<Point>();
            var length = poly.Length;

            for (var i = 0; i < length; i++)
            {
                var b1 = poly[i];
                var b2 = poly[(i + 1) % length];
                var inter = IntersectLineLine(a1, a2, b1, b2);

                points.AddRange(inter.Points);
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectLineRectangle(Point a1, Point a2, Point r1, Point r2)
        {
            var min = r1.Min(r2);
            var max = r1.Max(r2);
            var topRight = new Point { X = max.X, Y = min.Y };
            var bottomLeft = new Point { X = min.X, Y = max.Y };

            var inter1 = IntersectLineLine(min, topRight, a1, a2);
            var inter2 = IntersectLineLine(topRight, max, a1, a2);
            var inter3 = IntersectLineLine(max, bottomLeft, a1, a2);
            var inter4 = IntersectLineLine(bottomLeft, min, a1, a2);

            var points = new List<Point>();

            points.AddRange(inter1.Points);
            points.AddRange(inter2.Points);
            points.AddRange(inter3.Points);
            points.AddRange(inter4.Points);

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectPolygonPolygon(Point[] points1, Point[] points2)
        {
            var points = new List<Point>();
            var length = points1.Length;

            for (var i = 0; i < length; i++)
            {
                var a1 = points1[i];
                var a2 = points1[(i + 1) % length];

                var inter = IntersectLinePolygon(a1, a2, points2);

                points.AddRange(inter.Points);
            }

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectPolygonRectangle(Point[] poly, Point r1, Point r2)
        {
            var min = r1.Min(r2);
            var max = r1.Max(r2);
            var topRight = new Point { X = max.X, Y = min.Y };
            var bottomLeft = new Point { X = min.X, Y = max.Y };

            var inter1 = IntersectLinePolygon(min, topRight, poly);
            var inter2 = IntersectLinePolygon(topRight, max, poly);
            var inter3 = IntersectLinePolygon(max, bottomLeft, poly);
            var inter4 = IntersectLinePolygon(bottomLeft, min, poly);

            var points = new List<Point>();

            points.AddRange(inter1.Points);
            points.AddRange(inter2.Points);
            points.AddRange(inter3.Points);
            points.AddRange(inter4.Points);

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }

        public static Interception IntersectRectangleRectangle(Point a1, Point a2, Point b1, Point b2)
        {
            var min = a1.Min(a2);
            var max = a1.Max(a2);
            var topRight = new Point { X = max.X, Y = min.Y };
            var bottomLeft = new Point { X = min.X, Y = max.Y };

            var inter1 = IntersectLineRectangle(min, topRight, b1, b2);
            var inter2 = IntersectLineRectangle(topRight, max, b1, b2);
            var inter3 = IntersectLineRectangle(max, bottomLeft, b1, b2);
            var inter4 = IntersectLineRectangle(bottomLeft, min, b1, b2);

            var points = new List<Point>();

            points.AddRange(inter1.Points);
            points.AddRange(inter2.Points);
            points.AddRange(inter3.Points);
            points.AddRange(inter4.Points);

            return new Interception
            {
                Points = points.ToArray(),
                Type = points.Count > 0 ? InterceptionTypes.Interception : InterceptionTypes.None
            };
        }
    }
}
