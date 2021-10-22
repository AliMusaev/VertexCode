using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VertexCodeMakerDomain;
using Point = netDxf.Entities.Point;

namespace VisualControlApp
{
    class DxfBuilder
    {
        private ProfileMarker _marker;
        private DxfDocument _doc; 
        public DxfBuilder()
        {
            _marker = new ProfileMarker();
        }

        public void BuildOxyModel(List<PresentVertexSection> sections)
        {

            _doc = new DxfDocument();
            foreach (var section in sections)
            {
                if (section.X1 == 0 && section.X2 == 0 && section.Y1 == 0 && section.Y2 == 0)
                    continue;
                section.Mark = _marker.GetNextMark().ToString();
                // Find bounding rectangle
                var sectionPoints = GenerateBoundingPoints(section);
               
                //var sectionPoints = GenerateBoundingPoints(p1, p2, section.Height, section.CosY, section.SinY);
                GnerateLine(sectionPoints, section.SectionName);
                Vector2 init = new Vector2(section.X1, section.Y1);
                CalculateAnnotPoint(section.CosX, section.SinX, init, section.SectionName);
                //CalculateAnnotPoint(section.CommandsCollection, section.Height, section.CosX, section.SinX, section.CosY, section.SinY, init, section.SectionName);
                
            }
            _doc.Save("C:\\Users\\musaev.ae\\source\\repos\\VertexCodeMaker\\NxAssemblyReaderLib\\bin\\x64\\Debug\\test.dxf");
        }

        private Vector2[] GenerateBoundingPoints(PresentVertexSection sect)
        {
            double halH = sect.Height / 2;
            // p2  *       * p3
            //
            //
            //
            // p1  *       * p4
            Vector2 point1 = new Vector2(sect.X2 - halH * sect.CosY, sect.Y2 - halH * sect.SinY);
            Vector2 point2 = new Vector2(sect.X1 - halH * sect.CosY, sect.Y1 - halH * sect.SinY);
            Vector2 point3 = new Vector2(sect.X1 + halH * sect.CosY, sect.Y1 + halH * sect.SinY);
            Vector2 point4 = new Vector2(sect.X2 + halH * sect.CosY, sect.Y2 + halH * sect.SinY);
            if (sect.IsOrthogonal)
            {
                return new Vector2[] { point1, point2, point3, point4, point1 };
            }
            else
            {
                double delta = 10;
                // Start
                Vector2 p1 = new Vector2(point2.X + delta * sect.CosX, point2.Y + delta * sect.SinX);
                // Top
                Vector2 p2 = new Vector2(point2.X + delta * sect.CosY, point2.Y + delta * sect.SinY);
                Vector2 p3 = new Vector2(point3.X - delta * sect.CosY, point3.Y - delta * sect.SinY);
                // Right
                Vector2 p4 = new Vector2(point3.X + delta * sect.CosX, point3.Y + delta * sect.SinX);
                Vector2 p5 = new Vector2(point4.X - delta * sect.CosX, point4.Y - delta * sect.SinX);
                // Bot
                Vector2 p6 = new Vector2(point4.X - delta * sect.CosY, point4.Y - delta * sect.SinY);
                Vector2 p7 = new Vector2(point1.X + delta * sect.CosY, point1.Y + delta * sect.SinY);
                //Left
                Vector2 p8 = new Vector2(point1.X - delta * sect.CosX, point1.Y - delta * sect.SinX);
                return new Vector2[] { p1, p2, p3, p4, p5, p6, p7, p8, p1 };
            }
        }
        private void GnerateLine(Vector2[] points, string name)
        {
            netDxf.Tables.Layer layer = new netDxf.Tables.Layer(name);
            List<Line> lines = new List<Line>();
            for (int i = 1; i < points.Length; i++)
            {
                var line = new Line(points[i - 1], points[i]);
                line.Layer = layer;
                lines.Add(line);
            }
            _doc.AddEntity(lines);
        }
        private void CalculateAnnotPoint(double cosX, double sinX, Vector2 init, string name)
        {
            var x = init.X + 50 * cosX;
            var y = init.Y + 50 * sinX;
            var circle = new Circle(new Vector2(x, y),10);
            circle.Layer = _doc.Layers.First(z=>z.Name == name);
            circle.Color = AciColor.Red;
            _doc.AddEntity(circle);


        }
    }
}
