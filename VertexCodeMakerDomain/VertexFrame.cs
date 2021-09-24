using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertexCodeMakerDomain.Interfaces;

namespace VertexCodeMakerDomain
{
    public class VertexFrame
    {
        public string Name { get; }
        public string CabinName { get; }
        private List<VertexSection> _sectionsList;
        public VertexFrame(List<VertexSection> sections, string name, string cabinName = "empty")
        {
            Name = name;
            CabinName = cabinName;
            _sectionsList = sections;
        }
        public IReadOnlyList<VertexSection> GetSections()
        {
            return _sectionsList.AsReadOnly();
        }
        public void AddCommands(List<VertexCommand> commands)
        {
            foreach (var item in commands)
            {
                _sectionsList.Find(x => x.SectionName == item.ParentName).CommandsCollection.Add(item);
            }
        }
        public IDirectable GetSectionDirectionByName(string name)
        {
            try
            {
                return _sectionsList.Find(x => x.SectionName == name);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ISectionable GetSectionPointsByName(string name)
        {
            try
            {
                return _sectionsList.Find(x => x.SectionName == name);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IAngle GetSectionAnglesByName(string name)
        {
            try
            {
                return _sectionsList.Find(x => x.SectionName == name);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IMeasurable GetSectionMeasurementsBy(string name)
        {
            try
            {
                return _sectionsList.Find(x => x.SectionName == name);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
