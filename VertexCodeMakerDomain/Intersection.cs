using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertexCodeMakerDomain.Interfaces;

namespace VertexCodeMakerDomain
{
    public class Intersection
    {
        public Point2d InterPoint { get; }
        public string Section1Name { get; private  set; }
        public string Section2Name { get; private set; }
        public bool IsOrthogonal { get;  }
       
        public static Intersection Create(ISectionable sect1, ISectionable sect2)
        {
            var line1 = sect1;
            var line2 = sect2;
            Point2d b = new Point2d(line1.EndPoint.X - line1.StartPoint.X, line1.EndPoint.Y - line1.StartPoint.Y);
            Point2d d = new Point2d(line2.EndPoint.X - line2.StartPoint.X, line2.EndPoint.Y - line2.StartPoint.Y);
            var bDotDPerp = b.X * d.Y - b.Y * d.X;

            if (bDotDPerp == 0)
                return null;

            Point2d c = new Point2d(line2.StartPoint.X - line1.StartPoint.X, line2.StartPoint.Y - line1.StartPoint.Y);
            var t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return null;

            var u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return null;

            return new Intersection(new Point2d(line1.StartPoint.X + t * b.X, line1.StartPoint.Y + t * b.Y).Round(5), sect1 as VertexSection, sect2 as VertexSection);
        }
        private Intersection(Point2d intePoint, VertexSection section1, VertexSection section2)
        {
            InterPoint = intePoint;
            Section1Name = section1.SectionName;
            Section2Name = section2.SectionName;
            IsOrthogonal = DefineOrthogonalBetween(section1, section2);
        }
        private bool DefineOrthogonalBetween(IAngle sect1Angles, IAngle sect2Angles)
        {
            if (sect1Angles.IsOrthogonal && sect2Angles.IsOrthogonal)
            {
                return true;
            }
            return false;
        }
        public void RotateSections()
        {
            var temp = Section1Name;
            Section1Name = Section2Name;
            Section2Name = temp;
        }
    }
}
