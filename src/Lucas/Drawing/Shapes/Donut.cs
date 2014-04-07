using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    public class Donut : IEllipse, IHollowShape
    {
        public static Donut Create(params double[] args)
        {
            args.Length
                // Evaluate precondition. A donut must be contructed from 4 arguments
                .Assert(4, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "donut", 4, args.Length));

            return new Donut
            {
                Centre = new Point { X = args[0], Y = args[1] },
                InternalRadius = args[2],
                ExternalRadius = args[3]
            };
        }

        double[] IShape.Arguments { get { return new [] { Centre.X, Centre.Y, ExternalRadius, InternalRadius }; } }

        public double Area
        {
            get { return Math.PI * (Math.Pow(ExternalRadius, 2) - Math.Pow(InternalRadius, 2)); }
        }

        public Point Centre { get; set; }

        public double InternalRadius { get; set; }

        public double ExternalRadius { get; set; }

        double IEllipse.XRadius
        {
            get { return ExternalRadius; }
        }

        double IEllipse.YRadius
        {
            get { return ExternalRadius; }
        }

        IShape IHollowShape.Internal
        {
            get { return new Circle { Centre = Centre, Radius = InternalRadius }; }
        }

        public bool Contains(Point point)
        {
            var sqr = Math.Pow(point.X - Centre.X, 2) + Math.Pow(point.Y - Centre.Y, 2);
            return sqr <= Math.Pow(ExternalRadius, 2) && sqr >= Math.Pow(InternalRadius, 2);
        }

        public bool Contains(IShape that)
        {
            var inter = GetInterceptionWith(that);
            return inter.Type.Equals(InterceptionTypes.Inside);
        }

        public Interception GetInterceptionWith(IShape that)
        {
            var inter = Interception.None;

            if (that is IEllipse)
            {
                var other = that as IEllipse;
                inter = Interception.IntersectCircleEllipse(Centre, ExternalRadius, other.Centre, other.XRadius, other.YRadius);
            }
            else if (that is IConvexPolygon)
            {
                var other = that as IConvexPolygon;
                inter = Interception.IntersectCirclePolygon(Centre, ExternalRadius, other.Points);
            }

            if (inter.Type != InterceptionTypes.Inside)
            {
                if (InterceptionTypes.Inside == new Circle { Centre = Centre, Radius = InternalRadius }.GetInterceptionWith(that).Type)
                    inter = Interception.None; 
            }

            return inter;
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
            return string.Format(Strings.SHAPE_DONUT_FORMAT, Centre.X, Centre.Y, InternalRadius, ExternalRadius);
        }
    }
}
