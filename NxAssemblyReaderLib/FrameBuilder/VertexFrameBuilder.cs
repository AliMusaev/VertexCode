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
        public VertexFrame BuildFrameBy(List<VertexSection> vertexSections, string assemblyName)
        {
            
            var intersectionsList = FindIntersections(vertexSections);
            VertexFrame frame = new VertexFrame(vertexSections, assemblyName);
            frame.AddCommands(StandartFeaturesFinder(intersectionsList, frame));
            return frame;
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
            StandartCommandsBuilder commandsBuilder = new StandartCommandsBuilder();
            var commands = new List<VertexCommand>();
            foreach (var intersection in intersections)
            {
                commands.AddRange(commandsBuilder.BuildCommandsAccording(intersection));
            }
            return commands;
        }

    }
}
