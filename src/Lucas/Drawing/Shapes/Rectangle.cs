using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    public class Rectangle : IShape, IConvexPolygon
    {
        public static Rectangle Create(params double[] args)
        {
            args.Length
                // Evaluate precondition. A rectangle must be contructed from 4 arguments
                .Assert(4, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_COUNT, "rectangle", 4, args.Length));

            return new Rectangle
            {
                TopLeft = new Point { X = args.First(), Y = args.Skip(1).First() },
                Height = args.Skip(2).First(),
                Width = args.Skip(2).First()
            };
        }

        public double Area
        {
            get { return Height * Width; }
        }

        double[] IShape.Arguments { get { return new[] { TopLeft.X, TopLeft.Y, Width, Height }; } }

        public Point[] Points
        {
            get { return new[] { TopLeft, TopLeft.Offset(0, Height), TopLeft.Offset(Width, Height), TopLeft.Offset(Width, 0) }; }
        }

        public Point TopLeft { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public bool Contains(Point point)
        {
            return point.X >= TopLeft.X && point.X <= TopLeft.X + Width
                && point.Y >= TopLeft.Y && point.Y <= TopLeft.Y + Height;
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
                return Interception.IntersectEllipseRectangle(other.Centre, other.XRadius, other.YRadius, TopLeft, TopLeft.Offset(Width, Height));
            }
            else if (that is IConvexPolygon)
            {
                var other = that as IConvexPolygon;
                return Interception.IntersectPolygonRectangle(other.Points, TopLeft, TopLeft.Offset(Width, Height));
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
            return string.Format(Strings.SHAPE_RECTANGLE_FORMAT, TopLeft.X, TopLeft.Y, Height, Width);
        }
    }
}
