using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    using Lucas.Drawing;
    using Lucas.Drawing.Shapes;

    partial class Program
    {
        void InitializeShapes()
        {
            _canvas.Register<Circle>(
                constructor: Circle.Create,
                summary: () => Strings.SHAPE_CIRCLE_SUMMARY,
                example: () => Strings.SHAPE_CIRCLE_EXAMPLE);

            _canvas.Register<Donut>(
                constructor: Donut.Create,
                summary: () => Strings.SHAPE_DONUT_SUMMARY,
                example: () => Strings.SHAPE_DONUT_EXAMPLE);

            _canvas.Register<Ellipse>(
                constructor: Ellipse.Create,
                summary: () => Strings.SHAPE_ELLIPSE_SUMMARY,
                example: () => Strings.SHAPE_ELLIPSE_EXAMPLE);

            _canvas.Register<Polygon>(
                constructor: Polygon.Create,
                summary: () => Strings.SHAPE_POLYGON_SUMMARY,
                example: () => Strings.SHAPE_POLYGON_EXAMPLE);

            _canvas.Register<Rectangle>(
                constructor: Rectangle.Create,
                summary: () => Strings.SHAPE_RECTANGLE_SUMMARY,
                example: () => Strings.SHAPE_RECTANGLE_EXAMPLE);

            _canvas.Register<Square>(
                constructor: Square.Create,
                summary: () => Strings.SHAPE_SQUARE_SUMMARY,
                example: () => Strings.SHAPE_SQUARE_EXAMPLE);

            _canvas.Register<Star>(
                constructor: Star.Create,
                summary: () => Strings.SHAPE_STAR_SUMMARY,
                example: () => Strings.SHAPE_STAR_EXAMPLE);

            _canvas.Register<Triangle>(
                constructor: Triangle.Create,
                summary: () => Strings.SHAPE_TRIANGLE_SUMMARY,
                example: () => Strings.SHAPE_TRIANGLE_EXAMPLE);
        }
    }
}
