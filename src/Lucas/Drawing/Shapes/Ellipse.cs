using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    public class Ellipse : IShape, IEllipse
    {
        public static Ellipse Create(params double[] args)
        {
            args.Length
                // Evaluate precondition. A donut must be contructed from 4 arguments
                .Assert(4, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "ellipse", 4, args.Length));

            return new Ellipse
            {
                Centre = new Point { X = args[0], Y = args[1] },
                YRadius = args[2],
                XRadius = args[3]
            };
        }

        public Point Centre { get; set; }

        public double Area
        {
            get { return Math.PI * YRadius * XRadius; }
        }

        double[] IShape.Arguments { get { return new [] { Centre.X, Centre.Y, XRadius, YRadius }; } }

        public double YRadius { get; set; }

        public double XRadius { get; set; }

        public bool Contains(Point point)
        {
            return (Math.Pow(point.X - Centre.X, 2) / Math.Pow(YRadius, 2) + Math.Pow(point.Y - Centre.Y, 2) / Math.Pow(XRadius, 2)) <= 1;
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
                return Interception.IntersectEllipseEllipse(Centre, XRadius, YRadius, other.Centre, other.XRadius, other.YRadius);
            }
            else if (that is IConvexPolygon)
            {
                var other = that as IConvexPolygon;
                return Interception.IntersectEllipsePolygon(Centre, XRadius, YRadius, other.Points);
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
            return string.Format(Strings.SHAPE_ELLIPSE_FORMAT, Centre.X, Centre.Y, YRadius, XRadius);
        }
    }
}
