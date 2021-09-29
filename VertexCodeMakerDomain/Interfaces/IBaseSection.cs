using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain.Interfaces
{
    public interface IBaseSection : IMeasurable, IReadException
    {
        string SectionName { get; }
        int SectionId { get; set; }
        IReadOnlyList<VertexCommand> CommandsCollection { get; }
    }
}
