using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain.Interfaces
{
    public interface IMeasurable
    {
        double Height { get; }
        double Width { get; }
        double Length { get; }
        double Thickness { get; }
    }
}
