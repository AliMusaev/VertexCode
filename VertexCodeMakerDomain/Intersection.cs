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
        public Point2d InterPoint { get; private set; }
        public VertexSection Section1 { get; private  set; }
        public VertexSection Section2 { get; private set; }
        public double [] Segments { get; private set; }
        public bool IsOrthogonal { get; private set; }
        public IntersectionType Type { get; private set; }
        public static Intersection Create(VertexSection sect1, VertexSection sect2)
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

            return new Intersection(new Point2d(line1.StartPoint.X + t * b.X, line1.StartPoint.Y + t * b.Y).Round(5), sect1, sect2);
        }
        private Intersection(Point2d intePoint, VertexSection section1, VertexSection section2)
        {
            InterPoint = intePoint;
            Section1 = section1;
            Section2 = section2;
            IsOrthogonal = DefineOrthogonalBetween();
            Segments = DefineSegments();
            Type = DefineType();
            
        }
        private IntersectionType DefineType()
        {
            int shortsCount = CalcShortsCount();
            if (shortsCount == 1)
            {
                return IntersectionType.Tee;
            }
            else if (shortsCount == 2)
            {
                return IntersectionType.Angular;
            }
            else if (shortsCount == 0)
            {
                return IntersectionType.Сruciform;
            }
            else
            {
                throw new Exception($"{shortsCount} segments have length <= {Section1.Height}(for {Section1.SectionName}) or <= {Section2.Height}(for {Section2.SectionName})\n" +
                    $"Cant defined intersection type");
            }
        }
        private int CalcShortsCount()
        {
            int shortsCount = 0;
            if (Segments[0] <= Section1.Height)
            {
                shortsCount++;
            }
            if (Segments[1] <= Section1.Height)
            {
                shortsCount++;
            }
            if (Segments[2] <= Section2.Height)
            {
                shortsCount++;
            }
            if (Segments[3] <= Section2.Height)
            {
                shortsCount++;
            }
            return shortsCount;
        }
        private double[] DefineSegments()
        {
            var values = new double[4];
            values[0] = Math.Round(CalcLength(Section1.StartPoint, InterPoint),3);
            values[1] = Math.Round(CalcLength(InterPoint, Section1.EndPoint),3);
            values[2] = Math.Round(CalcLength(Section2.StartPoint, InterPoint),3);
            values[3] = Math.Round(CalcLength(InterPoint, Section2.EndPoint),3);
            return values;
        }
        private bool DefineOrthogonalBetween()
        {
            if (Section1.IsOrthogonal && Section2.IsOrthogonal)
            {
                return true;
            }
            return false;
        }
        public void RotateSections()
        {
            var temp = Section1;
            Section1 = Section2;
            Section2 = temp;
        }
        private double CalcLength(Point2d first, Point2d second)
        {
            return Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
        }
    }
}
