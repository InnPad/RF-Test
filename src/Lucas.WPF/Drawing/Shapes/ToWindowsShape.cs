using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing.Shapes
{
    static class ColorHelper
    {
        public static System.Windows.Media.Color Color(this IShape shape, byte alpha)
        {
            int color = 0;
            foreach (var arg in shape.Arguments)
                color ^= arg.GetHashCode();

            return System.Windows.Media.Color.FromArgb(alpha, (byte)(color & 0xFF), (byte)((color >> 8) & 0xFF), (byte)((color >> 16) & 0xFF)); 
        }
    }

    partial class Circle
    {
        public System.Windows.Shapes.Shape ToWindowsShape()
        {
            var shape = new System.Windows.Shapes.Ellipse();

            shape.SetValue(System.Windows.Controls.Canvas.LeftProperty, Centre.X - Radius);
            shape.SetValue(System.Windows.Controls.Canvas.TopProperty, Centre.Y - Radius);
            shape.Width = 2 * Radius;
            shape.Height = 2 * Radius;

            var color = this.Color(192);
            var fill = new System.Windows.Media.SolidColorBrush(color);
            shape.Fill = fill;

            return shape;
        }
    }

    partial class Donut
    {
        public System.Windows.Shapes.Shape ToWindowsShape()
        {
            var shape = new System.Windows.Shapes.Ellipse();

            shape.SetValue(System.Windows.Controls.Canvas.LeftProperty, Centre.X - ExternalRadius);
            shape.SetValue(System.Windows.Controls.Canvas.TopProperty, Centre.Y - ExternalRadius);
            shape.Width = 2 * ExternalRadius;
            shape.Height = 2 * ExternalRadius;
            
            var color = this.Color(192);
            var fill = new System.Windows.Media.SolidColorBrush(color);
            shape.Fill = fill;

            var offset = InternalRadius / ExternalRadius;
            var opacityMask = new System.Windows.Media.RadialGradientBrush();
            opacityMask.GradientStops.Add(new System.Windows.Media.GradientStop { Color = color, Offset = offset });
            opacityMask.GradientStops.Add(new System.Windows.Media.GradientStop { Color = System.Windows.Media.Colors.Transparent, Offset = offset - 0.000001 });
            shape.OpacityMask = opacityMask;

            return shape;
        }
    }

    partial class Ellipse
    {
        public System.Windows.Shapes.Shape ToWindowsShape()
        {
            var shape = new System.Windows.Shapes.Ellipse();

            shape.SetValue(System.Windows.Controls.Canvas.LeftProperty, Centre.X - XRadius);
            shape.SetValue(System.Windows.Controls.Canvas.TopProperty, Centre.Y - YRadius);
            shape.Width = 2 * XRadius;
            shape.Height = 2 * YRadius;

            var color = this.Color(192);
            var fill = new System.Windows.Media.SolidColorBrush(color);
            shape.Fill = fill;

            return shape;
        }
    }

    partial class Polygon
    {
        public System.Windows.Shapes.Shape ToWindowsShape()
        {
            var shape = new System.Windows.Shapes.Polygon();

            //shape.SetValue(System.Windows.Controls.Canvas.LeftProperty, this.Points[0].X);
            //shape.SetValue(System.Windows.Controls.Canvas.TopProperty, this.Points[0].Y);

            foreach (var point in Points)
            {
                shape.Points.Add(new System.Windows.Point { X = point.X, Y = point.Y });
            }

            var color = this.Color(192);
            var fill = new System.Windows.Media.SolidColorBrush(color);
            shape.Fill = fill;

            return shape;
        }
    }

    partial class Rectangle
    {
        public System.Windows.Shapes.Shape ToWindowsShape()
        {
            var shape = new System.Windows.Shapes.Rectangle();

            shape.SetValue(System.Windows.Controls.Canvas.LeftProperty, this.TopLeft.X);
            shape.SetValue(System.Windows.Controls.Canvas.TopProperty, this.TopLeft.Y);

            shape.Width = Width;
            shape.Height = Height;

            var color = this.Color(192);
            var fill = new System.Windows.Media.SolidColorBrush(color);
            shape.Fill = fill;

            return shape;
        }
    }

    partial class Square
    {
        public System.Windows.Shapes.Shape ToWindowsShape()
        {
            var shape = new System.Windows.Shapes.Rectangle();

            shape.SetValue(System.Windows.Controls.Canvas.LeftProperty, this.TopLeft.X);
            shape.SetValue(System.Windows.Controls.Canvas.TopProperty, this.TopLeft.Y);

            shape.Width = Length;
            shape.Height = Length;

            var color = this.Color(192);
            var fill = new System.Windows.Media.SolidColorBrush(color);
            shape.Fill = fill;

            return shape;
        }
    }

    partial class Triangle
    {
        public System.Windows.Shapes.Shape ToWindowsShape()
        {
            var shape = new System.Windows.Shapes.Polygon();

            //shape.SetValue(System.Windows.Controls.Canvas.LeftProperty, this.Points[0].X);
            //shape.SetValue(System.Windows.Controls.Canvas.TopProperty, this.Points[0].Y);

            foreach (var point in Points)
            {
                shape.Points.Add(new System.Windows.Point { X = point.X, Y = point.Y });
            }

            var color = this.Color(192);
            var fill = new System.Windows.Media.SolidColorBrush(color);
            shape.Fill = fill;

            return shape;
        }
    }
}
