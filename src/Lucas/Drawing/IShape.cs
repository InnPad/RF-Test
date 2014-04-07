using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing
{
    public interface IShape
    {
        /// <summary>
        /// Surface area
        /// </summary>
        double Area { get; }

        /// <summary>
        /// Arguments required to clone this shape.
        /// </summary>
        double[] Arguments { get; }

        /// <summary>
        /// Returns true if the shape contains the point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>true if the file contains the point.</returns>
        bool Contains(Point point);

        /// <summary>
        /// Returns true if the shape fully contains the other.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        bool Contains(IShape that);

        /// <summary>
        /// Returns true if the shape intersects another shape
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        bool Intersects(IShape that);
    }
}
