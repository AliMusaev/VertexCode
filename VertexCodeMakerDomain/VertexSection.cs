using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VertexCodeMakerDomain.Interfaces;

namespace VertexCodeMakerDomain
{
    public class VertexSection : IBaseSection, IAngle, IDirectable, ISectionable
    {
        public string SectionName { get; }
        public List<VertexCommand> CommandsCollection { get; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public double Length { get; private set; }
        public double Thickness { get; private set; }
        public Point2d StartPoint { get; private set; }
        public Point2d EndPoint { get; private set; }
        public double CosX { get; private set; }
        public double CosY { get; private set; }
        public double SinX { get; private set; }
        public double SinY { get; private set; }
        public bool IsOrthogonal { get; }
        public ShelvsDirection Direction { get; private set; }
        public bool IsSwapped { get; set; }
        public string Mark { get; set; }
        
        public VertexSection(IBaseSection baseSection, Point2d starPoint, Point2d endPoint, double[] angles, ShelvsDirection direction)
        {
            this.SectionName = $"{baseSection.SectionName} {Math.Round(starPoint.X),1}:{Math.Round(starPoint.Y),1} {Math.Round(endPoint.X),1}:{Math.Round(endPoint.Y),1}";
            this.Width = baseSection.Width;
            this.Height = baseSection.Height;
            this.Length = baseSection.Length;
            this.Thickness = baseSection.Thickness;
            this.CommandsCollection = baseSection.CommandsCollection.Select(x => x.Clone() as VertexCommand).ToList();
            CosX = angles[0];
            SinX = angles[1];
            CosY = angles[2];
            SinY = angles[3];
            StartPoint = starPoint;
            EndPoint = endPoint;
            Direction = direction;
            IsOrthogonal = DefineOrthogonal(angles[0], angles[1]);
        }
        private bool DefineOrthogonal(double angle1, double angle2)
        {
            if (angle1 == 0 || angle2 == 0)
            {
                return true;
            }
            return false;
        }
    }
}
