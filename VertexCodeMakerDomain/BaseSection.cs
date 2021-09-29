using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertexCodeMakerDomain.Interfaces;

namespace VertexCodeMakerDomain
{
    public class BaseSection : IBaseSection, ICloneable, IReadException
    {
        private List<VertexCommand> _commandsCollection;
        public string SectionName { get; }
        public IReadOnlyList<VertexCommand> CommandsCollection { get { return _commandsCollection.AsReadOnly(); } }
        public double Width { get; }
        public double Height { get; }
        public double Length { get; }
        public double Thickness { get; }
        public int SectionId { get; set; }
        public List<string> ExceptionsCollection { get; set; }

        public BaseSection(string name, List<VertexCommand> commands, double width, double height, double length, double thickness)
        {
            SectionName = name;
            _commandsCollection = commands;
            Width = width;
            Height = height;
            Length = length;
            Thickness = thickness;
            ExceptionsCollection = new List<string>();
        }
        public BaseSection(string name, List<VertexCommand> commands, double width, double height, double length, double thickness,  List<string> coll)
        {
            SectionName = name;
            _commandsCollection = commands;
            Width = width;
            Height = height;
            Length = length;
            Thickness = thickness;
            ExceptionsCollection = coll;
        }
        public object Clone()
        {
            return new BaseSection(SectionName, CommandsCollection.Select(x => x.Clone() as VertexCommand).ToList(), Width, Height, Length, Thickness, new List<string>(ExceptionsCollection));
        }
    }
}
