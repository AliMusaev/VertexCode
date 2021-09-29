using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualControlApp
{
    interface IPresentVertex
    {
        string SectionName { get; set; }
        double Width { get; set; }
        double GA { get; set; }
        string CU { get; set; }
        int QT { get; set; }
        string DE { get; set; }
    }
}
