using NXOpen;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public PlotModel BuildOxyModel(List<PresentVertexSection> sections)
        {
            
            var oxyChart = new PlotModel();
            oxyChart.PlotType = PlotType.Cartesian;
            foreach (var section in sections)
            {
                Point2d p1 = new Point2d(section.X1, section.Y1);
                Point2d p2 = new Point2d(section.X2, section.Y2);
                if (p1.X == 0 && p2.X == 0 && p1.Y == 0 && p2.Y == 0)
                    continue;

                section.Mark = _marker.GetNextMark().ToString();
                // Find bounding rectangle
               
                var sectionPoints = GenerateBoundingPoints(p1, p2, section.Height, section.CosY, section.SinY);
                LineSeries ln = GnerateLine(sectionPoints, section.SectionName, section.Direction);

                
                oxyChart.Series.Add(ln);
               
                   
                foreach (var command in section.CommandsCollection)
                {   
                    oxyChart.Series.Add(CreateAnnotationsBy(command, section.SectionName, p1, section.Height, section.CosX, section.SinX, section.CosY, section.SinY));
                }
                
            }

            return oxyChart;
        }


        public class CustomDataPoint : IDataPointProvider
        {
            public double X { get; set; }
            public double Y { get; set; }
            public string Description { get; set; }
            public DataPoint GetDataPoint() => new DataPoint(X, Y);

            public CustomDataPoint(double x, double y, string description)
            {
                X = x;
                Y = y;
                Description = description;
            }
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

        private LineSeries GnerateLine(Point2d[] points, string sectionName, string direction)
        {
            LineSeries ln = new LineSeries();

            var a = points.Select(x => new DataPoint(x.X, x.Y)).ToList();
            a.Add(new DataPoint(points[0].X, points[0].Y));
            ln.ItemsSource = a;
            ln.LineStyle = LineStyle.Solid;
            ln.Title = $"{sectionName} {direction}";
            ln.StrokeThickness = 2;
            return ln;
        }
        private LineSeries CreateAnnotationsBy(PresentVertexCommand command, string name, Point2d init, double height, double cosX, double sinX, double cosY, double sinY)
        {
            var points = new List<DataPoint>();
          
            var halH = height / 2;
            double add = 0;
            if (command.Operation == "Notch")
                add = halH * -1;
            if (command.Operation == "Lip")
                add = halH;
            if (command.Operation == "Service")
                add = halH * -1;

            var x = init.X + command.Ordinate * cosX + add * cosY;
            var y = init.Y + command.Ordinate * sinX + add * sinY;

            var value = 1;
            points.Add( new DataPoint(x, y - value));
            points.Add(new DataPoint(x, y + value));
            points.Add(new DataPoint(x, y));
            points.Add(new DataPoint(x + value, y));
            points.Add(new DataPoint(x, y));
            points.Add(new DataPoint(x - value, y));

            var lineSeries = new LineSeries
            {
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Black,
                StrokeThickness = 3,
                ItemsSource = points,
                TrackerFormatString = $"{name} {command.Operation}\n{Math.Round(x, 1)}\n{Math.Round(y, 1)}",

            };

            return lineSeries;

        }
       

        

    }
}
