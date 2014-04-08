using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas.Drawing
{
    partial interface IShape
    {
        System.Windows.Shapes.Shape ToWindowsShape();
    }
}
