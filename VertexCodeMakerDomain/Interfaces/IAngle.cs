using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain.Interfaces
{
    public interface IAngle
    {
        double CosX { get; }
        double SinX { get; }
        double CosY { get; }
        double SinY { get; }
        bool IsOrthogonal { get; }
    }
}
