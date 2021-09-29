using NLog;
using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertexCodeMakerDomain;

namespace NxAssemblyReaderLib.BaseBuilder
{
    class ServiceReader
    {
        private FeaturesHandler _featuresHandler;
        private Logger _logger;
        private List<VertexCommand> _commands;
        public ServiceReader(FeaturesHandler featuresHandler, Logger logger)
        {
            _featuresHandler = featuresHandler;
            _logger = logger;
        }
        public List<VertexCommand> GetServiceCommandsFrom(BasePart part, string partName)
        {
            _commands = new List<VertexCommand>();
            
            // var features = FindFeaturesBy<HolePackage>(part);
            var features = _featuresHandler.FindBy<HolePackage>(part, "HOLE PACKAGE");

            // if part dont contain cutouts return - list without elements
            if (features == null || features.Count == 0)
            {
                return _commands;
            }

            var holes = features.Select(x => _featuresHandler.CastNxObjectTo<HolePackage>(x)).ToList();
            // Create holes commands
            foreach (var item in holes)
            {
                CreateService(item, partName);
            }
            //Error
            if (features.Count != _commands.Count)
            {
                Program.ErMessages.Add(new ErrorMessage {  SectId = partName, Message = $"Some services cant finded, input amount - {features.Count}, output amount - {_commands.Count}" });
                _logger.Warn($"Some services cant finded, input amount - {features.Count}, output amount - {_commands.Count}");
            }
            return _commands;
        }
        private void CreateService(HolePackage hole, string partName)
        {
            hole.GetOrigins(out Point3d[] points);
            if (points.Length != 1)
            {
                Program.ErMessages.Add(new ErrorMessage { SectId = partName, Message = $"Service \"{hole.Name}\" dont contain points or more then 1"});
                _logger.Warn($"Service \"{hole.Name}\" dont contain points or more then 1");
                
            }
            else
            {
                _commands.Add(new VertexCommand(partName, Operations.Service, points[0].X));
            }
           
        }
    }
}
