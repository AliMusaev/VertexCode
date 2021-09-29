using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualControlApp
{
    class PresentVertexSectionEmpty : IPresentVertex
    {
        public string SectionName { get; set; }
        public double Width { get; set; }
        public double GA { get; set; }
        public string CU { get; set; }
        public int QT { get; set; }
        public string DE { get; set; }
        public PresentVertexSectionEmpty(PresentVertexSection master)
        {
            SectionName = master.SectionName;
            Width = master.Width;
            GA = master.GA;
            CU = master.CU;
            QT = master.QT;
            DE = "FIL";
        }
    }
}
