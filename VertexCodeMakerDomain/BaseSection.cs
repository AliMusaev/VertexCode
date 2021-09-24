using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertexCodeMakerDomain.Interfaces;

namespace VertexCodeMakerDomain
{
    public class BaseSection : IBaseSection, ICloneable
    {
        public string SectionName { get; }
        public List<VertexCommand> CommandsCollection { get; }
        public double Width { get; }
        public double Height { get; }
        public double Length { get; }
        public double Thickness { get; }
        public BaseSection(string name, List<VertexCommand> commands, double width, double height, double length, double thickness)
        {
            SectionName = name;
            CommandsCollection = commands;
            Width = width;
            Height = height;
            Length = length;
            Thickness = thickness;
        }

        public object Clone()
        {
            return new BaseSection(SectionName, CommandsCollection.Select(x => x.Clone() as VertexCommand).ToList(), Width, Height, Length, Thickness);
        }
    }
}
