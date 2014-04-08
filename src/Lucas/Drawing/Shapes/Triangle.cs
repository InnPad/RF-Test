using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    public partial class Triangle : IShape, IPolygon
    {
        public static Triangle Create(params double[] args)
        {
            args.Length
                // Evaluate precondition. A triangle must be contructed from 6 arguments
                .Assert(6, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "triangle", 6, args.Length));

            return new Triangle
            {
                A = new Point { X = args[0], Y = args[1] },
                B = new Point { X = args[2], Y = args[3] },
                C = new Point { X = args[4], Y = args[5] },
            };
        }

        public Point A { get; set; }

        public Point B { get; set; }

        public Point C { get; set; }

        public double Area
        {
            get { return Math.Abs(A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2; }
        }

        double[] IShape.Arguments { get { return new[] { A.X, A.Y, B.X, B.Y, C.X, C.Y }; } }

        public Point[] Points
        {
            get { return new[] { A, B, C }; }
        }

        public bool Contains(Point point)
        {
            return Interception.IntersectPolygonPoint(Points, point);

            /*var v0 = new Point { X = C.X - A.X, Y = C.Y - A.Y };
            var v1 = new Point { X = B.X - A.X, Y = B.Y - A.Y };
            var v2 = new Point { X = point.X - A.X, Y = point.Y - A.Y };

            // dot product
            Func<Point, Point, double> dot = (Point a, Point b) => a.X * b.X + a.Y * b.Y;

            var dot00 = dot(v0, v0);
            var dot01 = dot(v0, v1);
            var dot02 = dot(v0, v2);
            var dot11 = dot(v1, v1);
            var dot12 = dot(v1, v2);

            var invDenom = 1 / (dot00 * dot01 - dot01 * dot01);
            var u = (dot11 * dot01 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return (u >= 0) && (v >= 0) && (u + v < 1);*/
        }

        public bool Contains(IShape that)
        {
            var inter = GetInterceptionWith(that);
            return inter.Type.Equals(InterceptionTypes.Inside);
        }

        public Interception GetInterceptionWith(IShape that)
        {
            if (that is IEllipse)
            {
                var other = that as IEllipse;
                return Interception.IntersectEllipsePolygon(other.Centre, other.XRadius, other.YRadius, Points);
            }
            else if (that is IPolygon)
            {
                var other = that as IPolygon;
                return Interception.IntersectPolygonPolygon(other.Points, Points);
            }

            return Interception.None;
        }

        public bool Intersects(IShape that)
        {
            var inter = GetInterceptionWith(that);

            if (inter.Type.Equals(InterceptionTypes.Outside) && that is IHollowShape)
            {
                var hole = (that as IHollowShape).Internal;
                if (hole.Contains(this))
                    return false;
            }

            return inter.Type != InterceptionTypes.None;
        }
        
        public override string ToString()
        {
            return string.Format(Strings.SHAPE_TRIANGLE_FORMAT, A.X, A.Y, B.X, B.Y, C.X, C.Y);
        }
    }
}
