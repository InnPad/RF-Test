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
    public class Star : Polygon
    {
        public new static Star Create(params double[] args)
        {
            if (args.Length != 6)
                return new Star
                {
                    Points = Polygon.Create(args).Points
                };

            Point center = new Point { X = args[0], Y = args[1] }, start = new Point { X = args[2], Y = args[3] };
            var edges = new List<Point> { start };
            var ir = args[4];
            var n = args[5];
            var dx = start.X - center.X;
            var dy = start.Y - center.Y;
            var er = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(dy, 2));
            var a = Math.PI / n;
            var a0 = Math.Asin(dy / er);
            var ac = Math.Acos(dx / er);
            var at = Math.Atan(dy / dx);

            if (Math.Abs(a0 - ac) > 0.001 && Math.Abs(ac - at) < 0.001)
                a0 = ac;

            var ai = a0 + a;

            if (ai > (2 * Math.PI))
                ai -= (2 * Math.PI);

            edges.Add(new Point { X = Math.Round(center.X + ir * Math.Cos(ai), 2), Y = Math.Round(center.Y + ir * Math.Sin(ai)) });

            for (var i = 1; i < n; i++)
            {
                ai += a;

                if (ai > (2 * Math.PI))
                    ai -= (2 * Math.PI);

                edges.Add(new Point { X = Math.Round(center.X + er * Math.Cos(ai), 2), Y = Math.Round(center.Y + er * Math.Sin(ai)) });

                ai += a;

                if (ai > (2 * Math.PI))
                    ai -= (2 * Math.PI);

                edges.Add(new Point { X = Math.Round(center.X + ir * Math.Cos(ai), 2), Y = Math.Round(center.Y + ir * Math.Sin(ai)) });
            }

            return new Star
            {
                Points = edges.ToArray()
            };
        }
    }
}
