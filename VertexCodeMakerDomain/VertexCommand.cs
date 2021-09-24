using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertexCodeMakerDomain.Interfaces;

namespace VertexCodeMakerDomain
{
    public class VertexCommand : ICommand , ICloneable
    {
        public string ParentName { get; set; }
        public Operations Operation { get; }
        public double Ordinate { get; }


        public object Clone()
        {
            return new VertexCommand(ParentName, Operation, Ordinate);
        }

        public VertexCommand(string name, Operations operation, double value)
        {
            ParentName = name;
            Operation = operation;
            Ordinate = Math.Round(value,5);
        }
    }
}
