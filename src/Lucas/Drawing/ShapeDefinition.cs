using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing
{
    using Lucas.Drawing.Shapes;

    /// <summary>
    /// Shape descrition. Static information about a shape class.
    /// </summary>
    public sealed class ShapeDefinition
    {
        /// <summary>
        /// Shape constructor, takes an array of double values as arguments.
        /// </summary>
        public Delegate Constructor { get; set; }

        /// <summary>
        /// Function that returns an string that describes this shape type.
        /// </summary>
        public Func<string> Summary { get; set; }

        /// <summary>
        /// Function that returns an string with an exmaple for this shape type.
        /// </summary>
        public Func<string> Example { get; set; }
    }
}
