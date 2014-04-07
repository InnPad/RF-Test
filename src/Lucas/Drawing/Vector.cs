using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    public struct Vector
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Length() { return Math.Sqrt(this.X * this.X + this.Y * this.Y); }
        
        public double Dot(Vector that) { return this.X * that.X + this.Y * that.Y; }
        
        public double Cross(Vector that) { return this.X * that.Y - this.Y * that.X; }
        
        public Vector Unit() { return this.Divide(this.Length()); }
        
        public Vector UnitEquals() { this.DivideEquals(this.Length()); return this; }
        
        public Vector Add(Vector that) { return new Vector { X = this.X + that.X, Y = this.Y + that.Y }; }
        
        public Vector AddEquals(Vector that) { this.X += that.X; this.Y += that.Y; return this; }
        
        public Vector Subtract(Vector that) { return new Vector { X = this.X - that.X, Y = this.Y - that.Y }; }
        
        public Vector SubtractEquals(Vector that) { this.X -= that.X; this.Y -= that.Y; return this; }
        
        public Vector Multiply(double scalar) { return new Vector { X = this.X * scalar, Y = this.Y * scalar }; }
        
        public Vector MultiplyEquals(double scalar) { this.X *= scalar; this.Y *= scalar; return this; }
        
        public Vector Divide(double scalar) { return new Vector { X = this.X / scalar, Y = this.Y / scalar }; }
        
        public Vector DivideEquals(double scalar) { this.X /= scalar; this.Y /= scalar; return this; }
        
        public Vector Perp() { return new Vector { X = -this.Y, Y = this.X }; }
        
        public static Vector FromPoints(Point p1, Point p2) { return new Vector { X = p2.X - p1.X, Y = p2.Y - p1.Y }; }

        public static implicit operator Vector(Point point)
        {
            return new Vector { X = point.X, Y = point.Y };
        }
    }
}
