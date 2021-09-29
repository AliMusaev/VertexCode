using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain.Interfaces
{
    public interface IReadException
    {
        List<string> ExceptionsCollection { get; set; }
    }
}
