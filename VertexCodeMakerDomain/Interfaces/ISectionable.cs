using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain.Interfaces
{
    public interface ISectionable
    {
        Point2d StartPoint { get; }
        Point2d EndPoint { get; }
    }
}
