using NLog;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpenUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VertexCodeMakerDomain;

namespace NxAssemblyReaderLib.FrameBuilder
{
    class VertexFrameBuilder
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public VertexFrameBuilder()
        {
         
        }
        public VertexFrame BuildFrameBy(string frameName, Component[] components, List<BaseSection> baseSections)
        {
            var axis = DetermineSecondAxis(components);
            var a = CreateVertexSections(baseSections, components, axis);
            var intersectionsList = FindIntersections(a);
            VertexFrame frame = new VertexFrame(a, frameName);
            frame.AddCommands(StandartFeaturesFinder(intersectionsList, frame));
            return frame;
        }
        private SecondAxis DetermineSecondAxis(Component[] components)
        {
            List<double> results = new List<double>();
            foreach (var item in components)
            {
                item.GetPosition(out Point3d point, out Matrix3x3 matrix);
                results.Add(point.Y);
            }

            var b = results.GroupBy(x => Math.Round(x, 1)).ToList();
            if (b.Count == 1)
            {
                return SecondAxis.Z;
            }
            return SecondAxis.Y;
        }
        private List<VertexSection> CreateVertexSections(List<BaseSection> sections, Component[] components, SecondAxis axis)
        {
            VertexSectionBuilder sectionBuilder = new VertexSectionBuilder();
            List<VertexSection> retVal = new List<VertexSection>();
            foreach (var component in components)
            {
                //var c = component.GetAttributeTitlesByType(NXObject.AttributeType.String);
                //foreach (var item in c)
                //{
                //    MessageBox.Show($"{item}");
                //}
                // If component name is not assembly name (ignore assembly component)

                var name = FindPartName(component);
                if (component.OwningPart.Name != name)
                {
                    var baseSection = sections.Find(x => x.SectionName.Equals(name, StringComparison.OrdinalIgnoreCase)).Clone() as BaseSection;
                    component.GetPosition(out Point3d point, out Matrix3x3 matrix);
                    retVal.Add(sectionBuilder.BuildSectionBy(baseSection, point, matrix, axis));
                }
            }
            return retVal;
        }
        private string FindPartName(Component part)
        {
            try
            {
                return part.GetUserAttributeAsString("DB_PART_NAME", NXObject.AttributeType.String, -1);
            }
            catch (NXException ex)
            {
                _logger.Warn(ex, "DB name not found");
            }
            return part.Name;
        }
        private List<Intersection> FindIntersections(List<VertexSection> sections)
        {
            List<Intersection> intersections = new List<Intersection>();
            foreach (var item in sections)
            {

                for (int i = 0; i < sections.Count; i++)
                {
                    if (item.SectionName != sections[i].SectionName)
                    {
                        var inter = Intersection.Create(item, sections[i]);
                        if (inter != null && !intersections.Any(x => x.InterPoint.X == inter.InterPoint.X && x.InterPoint.Y == inter.InterPoint.Y))
                        {
                            intersections.Add(inter);
                        }
                    }
                }
            }
            return intersections;
        }
   
        private List<VertexCommand> StandartFeaturesFinder(List<Intersection> intersections, VertexFrame frame)
        {
            var commands = new List<VertexCommand>();
            foreach (var intersection in intersections)
            {
                commands.AddRange(new StandartFeaturesFinder(intersection, frame).FindFeaturesIn());
            }
            return commands;
        }

    }
}
