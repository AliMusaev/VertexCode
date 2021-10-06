using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VertexCodeMakerDomain.Interfaces;

namespace VertexCodeMakerDomain
{
    public class VertexFrame
    {
        private List<VertexSection> _sectionsList;
        public string FrameName { get; }
        public string FrameId { get; set; }
        public string CabinName { get; }
        public IReadOnlyList<VertexSection> SectionsCollection { get { return _sectionsList.AsReadOnly(); } }
        public VertexFrame(List<VertexSection> sections, string name, string cabinName = "empty")
        {
            FrameName = name;
            CabinName = cabinName;
            _sectionsList = sections;
        }
        public void AddCommands(List<VertexCommand> commands)
        {
            
            foreach (var item in _sectionsList)
            {
                var comms = commands.FindAll(x => x.ParentName == item.SectionName).ToList();
                var b = comms.Where(x => x.Operation == Operations.Chamfer).GroupBy(y => y.Ordinate).Select(z => z.First()).ToList();
                comms.RemoveAll(x => x.Operation == Operations.Chamfer);
                comms.AddRange(b);
                comms = comms.OrderBy(x => x.Ordinate).ToList();
                item.AddCommands(comms);
            }
        }

    }
}
