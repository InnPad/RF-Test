using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    /// <summary>
    /// Convex Polygon
    /// </summary>
    public partial class Polygon : IShape, IPolygon
    {
        public static Polygon Create(params double[] args)
        {
            if (args.Length.Equals(5))
            {
                // Fast regrulary poliedral initialization

                Point center = new Point { X = args[0], Y = args[1] }, start = new Point { X = args[2], Y = args[3] };
                var edges = new List<Point> { start };
                var n = args[4];
                var dx = start.X - center.X;
                var dy = start.Y - center.Y;
                var r = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(dy, 2));
                var a = 2 * Math.PI / n;
                var a0 = Math.Asin(dx / r);
               
                for (var i = 1; i < n; i++) 
                {
                    var ai = a0 + i * a;

                    if (ai > 2 * Math.PI)
                        ai -= 2 * Math.PI;

                    edges.Add(new Point { X = Math.Round(center.X + r * Math.Cos(ai), 2), Y = Math.Round(center.Y + r * Math.Sin(ai)) });
                }

                return new Polygon
                {
                    Points = edges.ToArray()
                };
            }
            else
            {
                (args.Length > 5)
                    // Evaluate precondition. A polygon must be contructed from at least 6 arguments
                    .Assert(true, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "polygon", 6, args.Length));

                (args.Length % 2)
                    // Evaluate precondition. A polygon must be contructed from an even number arguments
                    .Assert(0, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "polygon", 2 * (1 + (int)(args.Length / 2)), args.Length));

                return new Polygon
                {
                    Points = args.Select((value, index) => new { Index = index, Value = value })
                        .GroupBy(pair => pair.Index / 2)
                        .Select(group => new Point { X = group.First().Value, Y = group.Last().Value })
                        .ToArray()
                };
            }
        }

        private Point[] _points;

        public double Area
        {
            get { return ToTriangles().Sum(triangle => triangle.Area); }
        }

        double[] IShape.Arguments
        {
            get { return Points.SelectMany(point => new [] { point.X, point.Y }).ToArray(); }
        }

        public Point[] Points
        {
            get { return _points ?? (_points = new Point[0]); }
            set { _points = value; }
        }

        public bool Contains(Point point)
        {
            return Interception.IntersectPolygonPoint(Points, point);
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
                return Interception.IntersectPolygonPolygon(Points, other.Points);
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

        /// <summary>
        /// Split a convex polygon into triangles
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public Triangle[] ToTriangles()
        {
            var points = Points;

            var triangles = new Triangle[points.Length - 3];

            for (var i = 2; i < points.Length - 1; i++)
            {
                triangles[i] = new Triangle { A = points[0], B = points[i - 1], C = points[i] };
            }

            return triangles;
        }

        public override string ToString()
        {
            return string.Format(Strings.SHAPE_POLYGON_FORMAT, string.Join(", ", Points.Select(point => string.Format("({0}, {1})", point.X, point.Y))));
        }
    }
}
