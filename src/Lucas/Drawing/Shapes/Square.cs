using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    public class Square : IShape, IConvexPolygon
    {
        public static Square Create(params double[] args)
        {
            args.Length
                // Evaluate precondition. A square must be contructed from 3 arguments
                .Assert(3, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "square", 3, args.Length));

            return new Square
            {
                TopLeft = new Point { X = args.First(), Y = args.Skip(1).First() },
                Length = args.Skip(2).First()
            };
        }

        public Point TopLeft { get; set; }

        public double Length { get; set; }

        public double Area
        {
            get { return Math.Pow(Length, 2); }
        }

        double[] IShape.Arguments { get { return new [] { TopLeft.X, TopLeft.Y, Length }; } }

        public Point[] Points
        {
            get { return new[] { TopLeft, TopLeft.Offset(0, Length), TopLeft.Offset(Length, Length), TopLeft.Offset(Length, 0) }; }
        }

        public bool Contains(Point point)
        {
            return point.X >= TopLeft.X && point.X <= TopLeft.X + Length
                && point.Y >= TopLeft.Y && point.Y <= TopLeft.Y + Length;
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
                return Interception.IntersectEllipseRectangle(other.Centre, other.XRadius, other.YRadius, TopLeft, TopLeft.Offset(Length, Length));
            }
            else if (that is IConvexPolygon)
            {
                var other = that as IConvexPolygon;
                return Interception.IntersectPolygonRectangle(other.Points, TopLeft, TopLeft.Offset(Length, Length));
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
            return string.Format(Strings.SHAPE_SQUARE_FORMAT, TopLeft.X, TopLeft.Y, Length);
        }
    }
}
