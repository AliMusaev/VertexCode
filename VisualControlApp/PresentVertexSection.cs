using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualControlApp
{
    class PresentVertexSection : IPresentVertex
    {
        public List<PresentVertexCommand> CommandsCollection { get; set; }
        public string SectionName { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Thickness { get; set; }
        public double GA { get; set; }
        public string CU { get; set; }
        public int QT { get; set; }
        public string DE { get; set; }
        public double HI { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double CosX { get; set; }
        public double CosY { get; set; }
        public double SinX { get; set; }
        public double SinY { get; set; }
        public string Direction { get; set; }
        public string Mark { get; set; }
        public string Type { get; set; }
        public bool IsEmpty { get; set; }
        public List<string> ExceptionsCollection { get; set; }
        public PresentVertexSection()
        {
            IsEmpty = false;
            DE = $"{Mark}-{Direction}";
        }
        public PresentVertexSection(PresentVertexSection section)
        {
            SectionName = section.SectionName;
            QT = section.QT;
            CU = section.CU;
            Width = 50;
            IsEmpty = true;
            DE = $"FIL";
        }

    }
}
