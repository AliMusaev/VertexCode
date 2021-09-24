using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain.Interfaces
{
    public interface IBaseSection : IMeasurable
    {
        string SectionName { get; }
        List<VertexCommand> CommandsCollection { get; }
    }
}
