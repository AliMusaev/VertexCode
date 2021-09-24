using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VertexCodeMakerDomain;
using VertexCodeMakerDomain.Interfaces;

namespace NxAssemblyReaderLib.FrameBuilder
{
    class StandartFeaturesFinder
    {
      
        public List<VertexCommand> FindFeaturesIn(Intersection intersection, VertexFrame frame)
        {
            List<VertexCommand> commands = new List<VertexCommand>();
            double H = DefineH(intersection,
                               frame.GetSectionMeasurementsBy(intersection.Section1Name),
                               frame.GetSectionAnglesByName(intersection.Section1Name),
                               frame.GetSectionDirectionByName(intersection.Section2Name)
                               );

            double[] values;
            string result;

            do
            {
                result = DefineType(H,
                                    intersection,
                                    frame.GetSectionPointsByName(intersection.Section1Name),
                                    frame.GetSectionPointsByName(intersection.Section2Name),
                                    frame.GetSectionDirectionByName(intersection.Section1Name),
                                    frame.GetSectionDirectionByName(intersection.Section2Name),
                                    out values
                                    );
            } while (result == "RepeatLip");
           


            if (result == "Lip")
            {
                commands.AddRange(AddLip(intersection,
                                    H,
                                    values,
                                    frame.GetSectionAnglesByName(intersection.Section1Name),
                                    frame.GetSectionPointsByName(intersection.Section2Name),
                                    frame.GetSectionMeasurementsBy(intersection.Section1Name).Height
                                    ));
            }
            else if (result == "Notch")
            {
                commands.AddRange(AddNotch(intersection, values, H, false));
            }
            else if (result == "Notch1")
            {
                commands.AddRange(AddNotch(intersection, values, H, true));
            }
            else
            {
                //throw new Exception();
            }
            // Dimples
            commands.Add(new VertexCommand(intersection.Section1Name, Operations.Dimple, values[0]));
            commands.Add(new VertexCommand(intersection.Section2Name, Operations.Dimple, values[2]));
            commands.AddRange(AddChamfer(intersection,
                       frame.GetSectionAnglesByName(intersection.Section1Name),
                       frame.GetSectionAnglesByName(intersection.Section2Name),
                       frame.GetSectionMeasurementsBy(intersection.Section1Name),
                       frame.GetSectionMeasurementsBy(intersection.Section2Name)
                       ));

            return commands;
        }
        private double DefineH(Intersection intersect, IMeasurable sect1Measurements, IAngle sect1Angles, IDirectable sect2Directable )
        {
            if (intersect.IsOrthogonal)
            {
                return sect1Measurements.Height / 2;
            }
            else
            {
                return CalcHypotenuse(sect2Directable.Direction, sect1Angles.CosX, sect1Angles.SinX, sect1Measurements.Height / 2);
            }
        }
        private string DefineType(double H, Intersection intersection, ISectionable sect1Points, ISectionable sect2Points, IDirectable d1, IDirectable d2, out double[] values)
        {
            values = GetValues(intersection.InterPoint, sect1Points, sect2Points);
            if (values[0] >= H && values[1] >= H && values[2] >= H && values[3] >= H)
            {
                if (intersection.IsOrthogonal)
                {
                    double horMax = 0;
                    double verMax = 0;
                    double horMin = 0;
                    double verMin = 0;
                    double delta = H / 2;
                    ShelvsDirection verDirect = ShelvsDirection.Undefined;
                    if ((d1.Direction == ShelvsDirection.D || d1.Direction == ShelvsDirection.U) && (d2.Direction == ShelvsDirection.L || d2.Direction == ShelvsDirection.R))
                    {
                        horMax = Math.Max(sect1Points.StartPoint.X, sect1Points.EndPoint.X);
                        verMax = Math.Max(sect2Points.StartPoint.X, sect2Points.EndPoint.X);
                        horMin = Math.Min(sect1Points.StartPoint.X, sect1Points.EndPoint.X);
                        verMin = Math.Min(sect2Points.StartPoint.X, sect2Points.EndPoint.X);
                        verDirect = d2.Direction;
                    }
                    else if ((d2.Direction == ShelvsDirection.D || d2.Direction == ShelvsDirection.U) && (d1.Direction == ShelvsDirection.L || d1.Direction == ShelvsDirection.R))
                    {

                        horMax = Math.Max(sect2Points.StartPoint.X, sect2Points.EndPoint.X);
                        verMax = Math.Max(sect1Points.StartPoint.X, sect1Points.EndPoint.X);
                        horMin = Math.Min(sect2Points.StartPoint.X, sect2Points.EndPoint.X);
                        verMin = Math.Min(sect1Points.StartPoint.X, sect1Points.EndPoint.X);
                        verDirect = d1.Direction;
                    }
                    if (horMax <= (verMax + H) && horMax > (verMax + delta) && verDirect == ShelvsDirection.R)
                    {
                        return "Notch1";
                    }
                    else if (horMin >= (verMin - H) && horMin < (verMin - delta) && verDirect == ShelvsDirection.L)
                    {
                        return "Notch1";
                    }


                }

                return "Notch";
            }
            

            if (values[0] < H && values[0] < values[1] && values[0] < values[2] && values[0] < values[3])
            {
                return "Lip";
            }
            if (values[1] < H && values[1] < values[0] && values[1] < values[2] && values[1] < values[3])
            {
                return "Lip";
            }
            if (values[2] < H && values[2] < values[1] && values[2] < values[0] && values[2] < values[3])
            {
                intersection.RotateSections();
                return "RepeatLip";
            }
            if (values[3] < H && values[3] < values[0] && values[3] < values[2] && values[3] < values[1])
            {
                intersection.RotateSections();
                return "RepeatLip";
            }
            else
            {
                return "";
            }
        }
        private List<VertexCommand> AddChamfer(Intersection intersect, IAngle sect1, IAngle sect2, IMeasurable sect1Measurenment, IMeasurable sect2Measurenment)
        {
            var commands = new List<VertexCommand>();
            if (!sect1.IsOrthogonal)
            {
                commands.AddRange(CreateChamfer(intersect.Section1Name, sect1Measurenment.Width));
            }
            if (!sect2.IsOrthogonal)
            {
                commands.AddRange(CreateChamfer(intersect.Section2Name, sect2Measurenment.Width));
            }
            return commands;
        }
        private List<VertexCommand> CreateChamfer(string name, double width)
        {
          
            var commands = new List<VertexCommand>();
            commands.Add(new VertexCommand(name, Operations.Chamfer, 0));
            commands.Add(new VertexCommand(name, Operations.Chamfer, width));
            // Rule 2
            //commands.Add(new VertexCommand(name, Operations.Swage, 33));
            //commands.Add(new VertexCommand(name, Operations.Swage, width-33));
            return commands;
        }
        private List<VertexCommand> AddNotch(Intersection intersect, double[] values, double H, bool n1)
        {
            var commands = new List<VertexCommand>();
            if (n1)
            {
                commands.Add(new VertexCommand(intersect.Section2Name, Operations.Notch, values[2]));
            }
            else
            {
                commands.Add(new VertexCommand(intersect.Section1Name, Operations.Notch, values[0]));
                commands.Add(new VertexCommand(intersect.Section1Name, Operations.Lip, values[0]));
            }
            return commands;
        }
        private double[] GetValues(Point2d intersectPoint, ISectionable sect1Points, ISectionable sect2Points)
        {
            var values = new double[4];
            values[0] = CalcLength(sect1Points.StartPoint, intersectPoint);
            values[1] = CalcLength(intersectPoint, sect1Points.EndPoint);
            values[2] = CalcLength(sect2Points.StartPoint, intersectPoint);
            values[3] = CalcLength(intersectPoint, sect2Points.EndPoint);
            return values;
        }
        private List<VertexCommand> AddLip(Intersection intersect, double H, double[] values, IAngle sect1Angles, ISectionable sect2Points, double sect1Height)
        {
            List<VertexCommand> commands = new List<VertexCommand>();
            double cutoutLength = Math.Abs(sect1Height / sect1Angles.CosY);

            if (intersect.IsOrthogonal)
            {
                    commands.Add(new VertexCommand(intersect.Section2Name, Operations.Lip, values[2]));
            }
            else
            {
                Point2d midPoint;
                if (values[0] < H)
                {
                    midPoint = DefineElementPoint(H, sect1Angles.CosX, sect1Angles.SinX, intersect.InterPoint);
                }
                else if (values[1] < H)
                {
                    midPoint = DefineElementPoint(H, sect1Angles.CosX, sect1Angles.SinX, intersect.InterPoint, true);
                }
                else
                {
                    MessageBox.Show("AAA");
                    throw new Exception();
                }
                double value = CalcLength(sect2Points.StartPoint, midPoint);
                commands.AddRange(CreateLips(cutoutLength, value, intersect.Section2Name));
                
            }
            return commands;
        }
        private List<VertexCommand> CreateLips(double cutoutLength, double pointPosition, string name)
        {
            // TODO: Add atribute instead of hard value
            var commands = new List<VertexCommand>();
            if (cutoutLength <= 54)
            {
                commands.Add(new VertexCommand(name, Operations.Lip, pointPosition));
            }
            else
            {
                var dist = (cutoutLength - 54)/ 2;
                commands.Add(new VertexCommand(name, Operations.Lip, pointPosition + dist));
                commands.Add(new VertexCommand(name, Operations.Lip, pointPosition - dist));
            }
            return commands;
        }
        private Point2d DefineElementPoint(double hypotenuse, double firstCos, double firstSin, Point2d interPoint, bool isNegative = false)
        {
            if (!isNegative)
            {
                return new Point2d(interPoint.X + (hypotenuse * firstCos), interPoint.Y + (hypotenuse * firstSin));
            }
            return new Point2d(interPoint.X - (hypotenuse * firstCos), interPoint.Y - (hypotenuse * firstSin));

        }
        private double CalcHypotenuse(ShelvsDirection direction, double cos, double sin, double sideA)
        {
            double sideB;
            if (direction == ShelvsDirection.U || direction == ShelvsDirection.D)
            {
                sideB = sideA / (sin / cos);
            }
            else
            {
                sideB = sideA / (cos / sin);
            }
            return Math.Sqrt(Math.Pow(sideB, 2) + Math.Pow(sideA, 2));
        }
        private double CalcLength(Point2d first, Point2d second)
        {
            return Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
        }

    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        