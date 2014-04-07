using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing
{
    public interface IEllipse
    {
        Point Centre { get; }

        double XRadius { get; }

        double YRadius { get; }
    }
}
