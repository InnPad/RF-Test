using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    public struct Point
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Point Offset(double dx, double dy)
        {
            return new Point { X = X + dx, Y = Y + dy };
        }

        public Point multiply(double scalar)
        {
            return new Point { X = scalar * X, Y = scalar * Y };
        }

        public Point Add(Point that)
        {
            return new Point { X = X + that.X, Y = Y + that.Y };
        }

        public Point AddEquals(Point that) { this.X += that.X; this.Y += that.Y; return this; }

        public Point ScalarAdd(double scalar) { return new Point { X = this.X + scalar, Y = this.Y + scalar }; }

        public Point AddEquals(double scalar) { this.X += scalar; this.Y += scalar; return this; }

        public Point Subtract(Point that) { return new Point { X = this.X - that.X, Y = this.Y - that.Y }; }
        public Point SubtractEquals(Point that) { this.X -= that.X; this.Y -= that.Y; return this; }
        public Point ScalarSubtract(double scalar) { return new Point { X = this.X - scalar, Y = this.Y - scalar }; }
        public Point ScalarSubtractEquals(double scalar) { this.X -= scalar; this.Y -= scalar; return this; }
        public Point Multiply(double scalar) { return new Point { X = this.X * scalar, Y = this.Y * scalar }; }
        public Point MultiplyEquals(double scalar) { this.X *= scalar; this.Y *= scalar; return this; }
        public Point Divide(double scalar) { return new Point { X = this.X / scalar, Y = this.Y / scalar }; }
        public Point DivideEquals(double scalar) { this.X /= scalar; this.Y /= scalar; return this; }
        public bool eq(Point that) { return (this.X == that.X && this.Y == that.Y); }
        public bool lt(Point that) { return (this.X < that.X && this.Y < that.Y); }
        public bool lte(Point that) { return (this.X <= that.X && this.Y <= that.Y); }
        public bool gt(Point that) { return (this.X > that.X && this.Y > that.Y); }
        public bool gte(Point that) { return (this.X >= that.X && this.Y >= that.Y); }
        public Point Lerp(Point that, double t) { return new Point { X = this.X + (that.X - this.X) * t, Y = this.Y + (that.Y - this.Y) * t }; }
        public double DistanceFrom(Point that) { var dx = this.X - that.X; var dy = this.Y - that.Y; return Math.Sqrt(dx * dx + dy * dy); }
        public Point Min(Point that) { return new Point { X = Math.Min(this.X, that.X), Y = Math.Min(this.Y, that.Y) }; }
        public Point Max(Point that) { return new Point { X = Math.Max(this.X, that.X), Y = Math.Max(this.Y, that.Y) }; }

        public override string ToString()
        {
            return string.Join(" ", X, Y);
        }

        public static bool TryParse(string s, out Point point)
        {
            bool pass = true;
            double x = 0, y = 0;

            var tokens = s.Split(new [] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length > 0)
                pass &= double.TryParse(tokens[0], out x);

            if (tokens.Length > 1)
                pass &= double.TryParse(tokens[1], out y);

            point = new Point
                {
                    X = x,
                    Y = y
                };

            return pass && tokens.Length == 2;
        }
    }
}
