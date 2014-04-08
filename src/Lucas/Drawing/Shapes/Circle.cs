using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    public partial class Circle : IShape, IEllipse
    {
        public static Circle Create(params double[] args)
        {
            args.Length
                // Evaluate precondition. A circle must be contructed from 3 arguments
                .Assert(3, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "circle", 3, args.Length));

            return new Circle
            {
                Centre = new Point { X = args[0], Y = args[1] },
                Radius = args[2]
            };
        }

        double[] IShape.Arguments { get { return new [] { Centre.X, Centre.Y, Radius }; } }

        public double Area
        {
            get { return 2 * Math.PI * Radius; }
        }

        public Point Centre { get; set; }

        public double Radius { get; set; }

        double IEllipse.XRadius
        {
            get { return Radius; }
        }

        double IEllipse.YRadius
        {
            get { return Radius; }
        }

        public bool Contains(Point point)
        {
            return Math.Pow(point.X - Centre.X, 2) + Math.Pow(point.Y - Centre.Y, 2) <= Math.Pow(Radius, 2);
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
                return Interception.IntersectCircleEllipse(Centre, Radius, other.Centre, other.XRadius, other.YRadius); 
            }
            else if (that is IPolygon)
            {
                var other = that as IPolygon;
                return Interception.IntersectCirclePolygon(Centre, Radius, other.Points);
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
            return string.Format(Strings.SHAPE_CIRCLE_FORMAT, Centre.X, Centre.Y, Radius);
        }
    }
}
