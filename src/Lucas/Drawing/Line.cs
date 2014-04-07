using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    public struct Line
    {
        public Point A { get; set; }

        public Point B { get; set; }

        public bool Contains(Point point)
        {
            double
                x = point.X,
                y = point.Y,
                x1 = A.X,
                x2 = B.X,
                y1 = A.Y,
                y2 = B.Y;

            if (x1 == x2 && x != x1)
                return false;
            else if (x1 < x2 && (x < x1 || x > x2))
                return false;
            else if (x < x2 || x > x1)
                return false;

            if (y1 == y2 && y != y1)
                return false;
            else if (y1 < y2 && (y < y1 || y > y2))
                return false;
            else if (y < y2 || y > y1)
                return false;

            // round up the error
            return Math.Abs(x * (y2 - y1) - y * (x2 - x1) - (x1 * y2 - x2 * y1)) < 0.01;
        }

        public Point? Intersection(Line line)
        {
            double
                x1 = A.X,
                x2 = B.X,
                x3 = line.A.X,
                x4 = line.B.X,
                y1 = A.Y,
                y2 = B.Y,
                y3 = line.A.Y,
                y4 = line.B.Y;

            var denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (denom != 0.0)
            {
                var point = new Point
                {
                    X = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denom,
                    Y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denom,
                };

                if (Contains(point))
                    return point;
            }
            
            if (Contains(line.A))
                return line.A;

            if (Contains(line.B))
                return B;

            return null;
        }
    }
}
