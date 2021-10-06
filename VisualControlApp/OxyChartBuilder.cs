using NXOpen;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var sectionPoints = GenerateBoundingPoints(section);
                //var sectionPoints = GenerateBoundingPoints(p1, p2, section.Height, section.CosY, section.SinY);
                LineSeries ln = GnerateLine(sectionPoints, section.SectionName, section.Direction);

                
                oxyChart.Series.Add(ln);
            }
            foreach (var section in sections)
            {
                Point2d p1 = new Point2d(section.X1, section.Y1);
                Point2d p2 = new Point2d(section.X2, section.Y2);
                if (section.CommandsCollection == null || section.CommandsCollection.Count == 0)
                {
                    continue;
                }
                foreach (var command in section.CommandsCollection)
                {
                    Point2d point = CalculateAnnotPoint(command, section.Height, section.CosX, section.SinX, section.CosY, section.SinY, p1);
                    oxyChart.Series.Add(CreateAnnotationsBy(section.SectionName, command.Operation, command.Ordinate, point, section.CosY, section.SinY));
                    oxyChart.Annotations.Add(CreatePointAnnotation(section.SectionName, command.Operation, point));
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
        private Point2d[] GenerateBoundingPoints1(PresentVertexSection sect)
        {
            double halH = sect.Height / 2;
            // p2  *       * p3
            //
            //
            //
            // p1  *       * p4
            Point2d point1 = new Point2d(Math.Round(sect.X2 - halH * sect.CosY, 2), Math.Round(sect.Y2 - halH * sect.SinY, 2));
            Point2d point2 = new Point2d(Math.Round(sect.X1 - halH * sect.CosY, 2), Math.Round(sect.Y1 - halH * sect.SinY, 2));
            Point2d point3 = new Point2d(Math.Round(sect.X1 + halH * sect.CosY, 2), Math.Round(sect.Y1 + halH * sect.SinY, 2));
            Point2d point4 = new Point2d(Math.Round(sect.X2 + halH * sect.CosY, 2), Math.Round(sect.Y2 + halH * sect.SinY, 2));
            if (sect.IsOrthogonal)
            {
                return new Point2d[] { point1, point2, point3, point4, point1 };
            }
            else
            {
                double delta = 10;
                // Start
                Point2d p1 = new Point2d(Math.Round(point2.X + delta * sect.CosX, 2), Math.Round(point2.Y + delta * sect.SinX, 2));
                // Top
                Point2d p2 = new Point2d(Math.Round(point2.X + delta * sect.CosY, 2), Math.Round(point2.Y + delta * sect.SinY, 2));
                Point2d p3 = new Point2d(Math.Round(point3.X - delta * sect.CosY, 2), Math.Round(point3.Y - delta * sect.SinY, 2));
                // Right
                Point2d p4 = new Point2d(Math.Round(point3.X + delta * sect.CosX, 2), Math.Round(point3.Y + delta * sect.SinX, 2));
                Point2d p5 = new Point2d(Math.Round(point4.X - delta * sect.CosX, 2), Math.Round(point4.Y - delta * sect.SinX, 2));
                // Bot
                Point2d p6 = new Point2d(Math.Round(point4.X - delta * sect.CosY, 2), Math.Round(point4.Y - delta * sect.SinY, 2));
                Point2d p7 = new Point2d(Math.Round(point1.X + delta * sect.CosY, 2), Math.Round(point1.Y + delta * sect.SinY, 2));
                //Left
                Point2d p8 = new Point2d(Math.Round(point1.X - delta * sect.CosX, 2), Math.Round(point1.Y - delta * sect.SinX, 2));
                return new Point2d[] { p1,p2,p3,p4,p5,p6,p7,p8,p1 };
            }
        }
        private Point2d[] GenerateBoundingPoints(PresentVertexSection sect)
        {
            double halH = sect.Height / 2;
            // p2  *       * p3
            //
            //
            //
            // p1  *       * p4
            Point2d point1 = new Point2d(sect.X2 - halH * sect.CosY, sect.Y2 - halH * sect.SinY);
            Point2d point2 = new Point2d(sect.X1 - halH * sect.CosY, sect.Y1 - halH * sect.SinY);
            Point2d point3 = new Point2d(sect.X1 + halH * sect.CosY, sect.Y1 + halH * sect.SinY);
            Point2d point4 = new Point2d(sect.X2 + halH * sect.CosY, sect.Y2 + halH * sect.SinY);
            if (sect.IsOrthogonal)
            {
                return new Point2d[] { point1, point2, point3, point4, point1 };
            }
            else
            {
                double delta = 10;
                // Start
                Point2d p1 = new Point2d(point2.X + delta * sect.CosX, point2.Y + delta * sect.SinX);
                // Top
                Point2d p2 = new Point2d(point2.X + delta * sect.CosY, point2.Y + delta * sect.SinY);
                Point2d p3 = new Point2d(point3.X - delta * sect.CosY, point3.Y - delta * sect.SinY);
                // Right
                Point2d p4 = new Point2d(point3.X + delta * sect.CosX, point3.Y + delta * sect.SinX);
                Point2d p5 = new Point2d(point4.X - delta * sect.CosX, point4.Y - delta * sect.SinX);
                // Bot
                Point2d p6 = new Point2d(point4.X - delta * sect.CosY, point4.Y - delta * sect.SinY);
                Point2d p7 = new Point2d(point1.X + delta * sect.CosY, point1.Y + delta * sect.SinY);
                //Left
                Point2d p8 = new Point2d(point1.X - delta * sect.CosX, point1.Y - delta * sect.SinX);
                return new Point2d[] { p1, p2, p3, p4, p5, p6, p7, p8, p1 };
            }
        }
        private LineSeries GnerateLine(Point2d[] points, string sectionName, string direction)
        {
            LineSeries ln = new LineSeries();

            var a = points.Select(x => new DataPoint(x.X, x.Y)).ToList();
            ln.ItemsSource = a;
            ln.LineStyle = LineStyle.Solid;
            ln.Title = $"{sectionName} {direction}";
            ln.StrokeThickness = 2;
            ln.TrackerFormatString = "{0}\n{1}: {2:2.#}\n{3}: {4:0.#}";
            return ln;
        }
        private Point2d CalculateAnnotPoint(PresentVertexCommand command, double height, double cosX, double sinX, double cosY, double sinY, Point2d init)
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
            return new Point2d(x, y);

        }
        private LineSeries CreateAnnotationsBy(PresentVertexCommand command, string name, Point2d init, double height, double cosX, double sinX, double cosY, double sinY)
        {
            var points = new List<DataPoint>();

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

            var value = 1;
            points.Add(new DataPoint(x, y - value));
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
        private LineSeries CreateAnnotationsBy(string name, Operations operation, double ordinate, Point2d point, double cosY, double sinY)
        {
            var points = new List<DataPoint>();
            var value = 0.05;
            points.Add(new DataPoint(point.X - value * cosY, point.Y - value * sinY));
            points.Add(new DataPoint(point.X + value * cosY, point.Y + value * sinY));
            var lineSeries = new LineSeries
            {
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Black,
                StrokeThickness = 3,
                ItemsSource = points,
                TrackerFormatString = $"Section: {name}\n" +
                $"Operation: {operation}\n" +
                $"Ordinate: {Math.Round(ordinate, 1)}",
            };

            return lineSeries;

        }
        private PointAnnotation CreatePointAnnotation(string name, Operations operation, Point2d point)
        {
            var annot =  new PointAnnotation
            {
                X = point.X,
                Y = point.Y,
                StrokeThickness = 1,
                Size = 5,
            };
            if (operation == Operations.Dimple)
            {
                annot.Fill = OxyColors.Gray;
                annot.Shape = MarkerType.Circle;
                return annot;
            }
            if (operation == Operations.Service)
            {
                annot.Shape = MarkerType.Circle;
                return annot;
            }
            if (operation == Operations.Notch)
            {
                annot.Fill = OxyColors.Green;
                annot.Shape = MarkerType.Square;
                return annot;
            }
            if (operation == Operations.Swage)
            {
                annot.Shape = MarkerType.Square;
                return annot;
            }
            if (operation == Operations.Chamfer)
            {
                annot.Fill = OxyColors.Blue;
                annot.Shape = MarkerType.Triangle;
                return annot;
            }
            if (operation == Operations.Lip)
            {
                annot.Shape = MarkerType.Triangle;
                return annot;
            }
            return annot;
        }
    }
}
