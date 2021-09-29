using NLog;
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
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private Intersection _intersection;
        private double _height;
        private List<VertexCommand> _commands;
        public StandartFeaturesFinder(Intersection intersection, VertexFrame frame)
        {
            _intersection = intersection;
            _commands = new List<VertexCommand>();
        }
        public List<VertexCommand> FindFeaturesIn()
        {

            _height = Math.Round(DefineHeight(),3);

            if (_intersection.Type == IntersectionType.Angular)
            {
                AddAngularFeatures();
            }
            if (_intersection.Type == IntersectionType.Сruciform)
            {
                AddСruciformFeatures();
            }
            if (_intersection.Type == IntersectionType.Tee)
            {
                AddTeeFeatures();
            }
            AddDimple();
            AddChamfer();

            return _commands;
        }
        // Define section1 half hight with angular bias
        private void AddTeeFeatures()
        {
            if (!_intersection.IsOrthogonal)
            {
                DefineAngularTeeFeatures();
            }
            else
            {
                DefineSectionPosition();
            }
            

        }
        private void AddСruciformFeatures()
        {
            if (_intersection.Section1.Direction == ShelvsDirection.U || _intersection.Section1.Direction == ShelvsDirection.D)
            {
                _commands.Add(new VertexCommand(_intersection.Section1.SectionName, Operations.Notch, _intersection.Segments[0]));
                _commands.Add(new VertexCommand(_intersection.Section1.SectionName, Operations.Lip, _intersection.Segments[0]));
            }
            else if (_intersection.Section1.Direction == ShelvsDirection.R || _intersection.Section1.Direction == ShelvsDirection.L)
            {
                _commands.Add(new VertexCommand(_intersection.Section2.SectionName, Operations.Notch, _intersection.Segments[2]));
                _commands.Add(new VertexCommand(_intersection.Section2.SectionName, Operations.Lip, _intersection.Segments[2]));
            }
            else
            {
                throw new Exception();
            }
        }
        private void AddAngularFeatures()
        {
            
            if (_intersection.Segments[0] < _height || _intersection.Segments[1] < _height)
            {
                _commands.Add(new VertexCommand(_intersection.Section2.SectionName, Operations.Lip, _intersection.Segments[2]));
            }
            else if (_intersection.Segments[2] < _height || _intersection.Segments[3] < _height) 
            {
                _commands.Add(new VertexCommand(_intersection.Section1.SectionName, Operations.Lip, _intersection.Segments[0]));
            }
            else
            {
                throw new Exception();
            }
        }
        private void AddDimple()
        {
            _commands.Add(new VertexCommand(_intersection.Section1.SectionName, Operations.Dimple, _intersection.Segments[0]));
            _commands.Add(new VertexCommand(_intersection.Section2.SectionName, Operations.Dimple, _intersection.Segments[2]));
        }
        private void AddChamfer()
        {
            if (!_intersection.Section1.IsOrthogonal)
            {
                _commands.Add(new VertexCommand(_intersection.Section1.SectionName, Operations.Chamfer, 0));
                _commands.Add(new VertexCommand(_intersection.Section1.SectionName, Operations.Chamfer, _intersection.Section1.Width));
                return;
            }
            if (!_intersection.Section2.IsOrthogonal)
            {
                _commands.Add(new VertexCommand(_intersection.Section2.SectionName, Operations.Chamfer, 0));
                _commands.Add(new VertexCommand(_intersection.Section2.SectionName, Operations.Chamfer, _intersection.Section2.Width));
                return;
            }
        }
        private void DefineSectionPosition()
        {
            if (_intersection.Segments[0] > _height && _intersection.Segments[1] > _height)
            {
                AddFeatureInSecondSection(DefineSectionPosRelativeIntersectionPoint(_intersection.Section2, _intersection.Segments[2], _intersection.Segments[3]), _intersection.Section1, _intersection.Segments[0]);
            }
            else if (_intersection.Segments[2] > _height && _intersection.Segments[3] > _height)
            {
                AddFeatureInSecondSection(DefineSectionPosRelativeIntersectionPoint(_intersection.Section1, _intersection.Segments[0], _intersection.Segments[1]), _intersection.Section2, _intersection.Segments[2]);

            }
        }
        private void AddFeatureInSecondSection(string pos, VertexSection sect, double value)
        {
            if (pos == "right")
            {
                if (sect.Direction == ShelvsDirection.L)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Notch, value));
                }
                if (sect.Direction == ShelvsDirection.R)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Lip, value));
                }
            }
            if (pos == "left")
            {
                if (sect.Direction == ShelvsDirection.L)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Lip, value));
                }
                if (sect.Direction == ShelvsDirection.R)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Notch, value));
                }
            }
            if (pos == "up")
            {
                if (sect.Direction == ShelvsDirection.U)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Lip, value));
                }
                else if (sect.Direction == ShelvsDirection.D)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Notch, value));
                }
            }
            if (pos == "down")
            {
                if (sect.Direction == ShelvsDirection.U)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Notch, value));
                }
                else if (sect.Direction == ShelvsDirection.D)
                {
                    _commands.Add(new VertexCommand(sect.SectionName, Operations.Lip, value));
                }
            }
        }
        private string DefineSectionPosRelativeIntersectionPoint(VertexSection section, double segment1, double segment2)
        {

            if (section.StartPoint.X > _intersection.InterPoint.X && section.EndPoint.X < _intersection.InterPoint.X)
            {
                if (segment1 <= _height && segment2 > _height)
                {
                    return "left";
                }
                return "right";

            }
            else if (section.StartPoint.X < _intersection.InterPoint.X && section.EndPoint.X > _intersection.InterPoint.X)
            {
                if (segment1 <= _height && segment2 > _height)
                {
                    return "right";
                }
                return "left";
            }
            if (section.StartPoint.Y > _intersection.InterPoint.Y && section.EndPoint.Y < _intersection.InterPoint.Y)
            {
                if (segment1 <= _height && segment2 > _height)
                {
                    return "down";
                }
                return "up";
            }
            else if (section.StartPoint.Y < _intersection.InterPoint.Y && section.EndPoint.Y > _intersection.InterPoint.Y)
            {
                if (segment1 <= _height && segment2 > _height)
                {
                    return "up";
                }
                return "down";
            }
            throw new Exception();

        }
        private void DefineAngularTeeFeatures()
        {
            if (!_intersection.Section1.IsOrthogonal)
            {
                CreateFeatures(_intersection.Section1, _intersection.Section2, _intersection.Segments[0], _intersection.Segments[1]);
            }
            else if (!_intersection.Section2.IsOrthogonal)
            {
                CreateFeatures(_intersection.Section2, _intersection.Section1, _intersection.Segments[2], _intersection.Segments[3]);
            }
            else
                throw new Exception();
           
        }
        private void CreateFeatures(VertexSection section, VertexSection section1, double segment1, double segment2)
        {
            double cutoutLength = Math.Abs(section.Height / section.CosY);
            Point2d midPoint;
            if (segment1 < _height)
            {
                midPoint = DefineElementPoint(_height, section.CosX, section.SinX, _intersection.InterPoint);
            }
            else if (segment2 < _height)
            {
                midPoint = DefineElementPoint(_height, section.CosX, section.SinX, _intersection.InterPoint, true);
            }
            else
            {
                throw new Exception();
            }
            double value = CalcLength(section1.StartPoint, midPoint);
            _commands.AddRange(CreateLips(cutoutLength, value, section1.SectionName));
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
        private double DefineHeight()
        {
            if (_intersection.IsOrthogonal)
            {
                return _intersection.Section1.Height / 2;
            }
            else
            {
                return CalcHypotenuse();
            }
        }
        private double CalcHypotenuse()
        {
            double sideA = _intersection.Section1.Height / 2;
            double sideB;
            if (_intersection.Section2.Direction == ShelvsDirection.U || _intersection.Section2.Direction == ShelvsDirection.D)
            {
                sideB = sideA / (_intersection.Section1.SinX / _intersection.Section1.CosX);
            }
            else
            {
                sideB = sideA / (_intersection.Section1.CosX / _intersection.Section1.SinX);
            }
            return Math.Sqrt(Math.Pow(sideB, 2) + Math.Pow(sideA, 2));
        }
        private double CalcLength(Point2d first, Point2d second)
        {
            return Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
        }

    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        