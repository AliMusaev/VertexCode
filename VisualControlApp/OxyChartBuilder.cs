using NXOpen;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VertexCodeMakerDomain;

namespace VisualControlApp
{
    class OxyChartBuilder
    {
        ProfileMarker _marker;
        public OxyChartBuilder()
        {
            _marker = new ProfileMarker();
        }

        public PlotModel BuildOxyModel(VertexFrame frame)
        {
            
            var oxyChart = new PlotModel();
            oxyChart.PlotType = PlotType.Cartesian;
            foreach (var section in frame.GetSections())
            {
                if (section.StartPoint.X == 0 && section.EndPoint.X == 0 && section.StartPoint.Y == 0 && section.EndPoint.Y == 0)
                    continue;

                section.Mark = _marker.GetNextMark().ToString();
                // Find bounding rectangle
                var sectionPoints = GenerateBoundingPoints(section.StartPoint, section.EndPoint, section.Height, section.CosY, section.SinY);
                LineSeries ln = GnerateLine(sectionPoints, section.SectionName, section.Direction);

                foreach (var command in section.CommandsCollection)
                {
                    oxyChart.Annotations.Add(GenerateAnnotationsBy(command, section.SectionName, section.StartPoint, section.Height, section.CosX, section.SinX, section.CosY, section.SinY));
                }
                
                oxyChart.Series.Add(ln);
            }
            return oxyChart;
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="startP">Init point in section</param>
        /// <param name="endP">End point in section</param>
        /// <param name="height">Hight of section</param>
        /// <param name="cosY">CosY of section</param>
        /// <param name="sinY">SinY of section</param>
        /// <returns>4 points - bounding rectangle of profile</returns>
        private Point2d[] GenerateBoundingPoints(Point2d startP, Point2d endP, double height, double cosY, double sinY)
        {
            double halH = height / 2;
            // p2  *       * p3
            //
            //
            //
            // p1  *       * p4
            Point2d point1 = new Point2d(Math.Round(endP.X - halH * cosY, 2), Math.Round(endP.Y - halH * sinY, 2));
            Point2d point2 = new Point2d(Math.Round(startP.X - halH * cosY, 2), Math.Round(startP.Y - halH * sinY, 2));
            Point2d point3 = new Point2d(Math.Round(startP.X + halH * cosY, 2), Math.Round(startP.Y + halH * sinY, 2));
            Point2d point4 = new Point2d(Math.Round(endP.X + halH * cosY, 2), Math.Round(endP.Y + halH * sinY, 2));
            return new Point2d[] { point1, point2, point3, point4 };
        }
        private LineSeries GnerateLine(Point2d[] points, string sectionName, ShelvsDirection direction)
        {
            LineSeries ln = new LineSeries();

            var a  = points.Select(x => new DataPoint(x.X, x.Y)).ToList();
            a.Add(new DataPoint(points[0].X, points[0].Y));
            ln.ItemsSource = a;
            ln.LineStyle = LineStyle.Solid;
            ln.Title = $"{sectionName} {direction}";
            ln.StrokeThickness = 2;
            return ln;
        }
        private PointAnnotation GenerateAnnotationsBy(VertexCommand command, string name,Point2d init, double height, double cosX, double sinX, double cosY, double sinY)
        {
            var halH = height / 2;
            double add = 0;
            if (command.Operation == Operations.Notch)
                add = halH * -1;
            if (command.Operation == Operations.Lip)
                add = halH;
            if (command.Operation == Operations.Service)
                add = halH * -1;

            var x = init.X + command.Ordinate * cosX + add * cosY;
            var y = init.Y + command.Ordinate * sinX + add * sinY;
           
            return new PointAnnotation
            {
                X = x,
                Y = y,
                Size = 3,
                Text = $"{command.Operation} {Math.Round(x, 2)}|{Math.Round(y, 2)}\n" +
                $"{name}",
                Fill = OxyColors.Black,
            };
        }
    }
}
