using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing
{
    public interface IHollowShape : IShape
    {
        IShape Internal { get; }
    }

    public static class HollowShapeHelper
    {
        public static bool Contains(this IHollowShape shape, Point point)
        {
            return shape.Contains(point) && !shape.Internal.Contains(point);
        }

        public static bool Intercepts(this IHollowShape shape, IShape other)
        {
            return false;
        }
    }
}
