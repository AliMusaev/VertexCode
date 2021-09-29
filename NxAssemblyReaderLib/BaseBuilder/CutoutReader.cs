using NLog;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertexCodeMakerDomain;

namespace NxAssemblyReaderLib.BaseBuilder
{
    class CutoutReader
    {
        private FeaturesHandler _featuresHandler;
        private Logger _logger;
        private List<VertexCommand> _commands;
        public CutoutReader(FeaturesHandler featuresHandler, Logger logger)
        {
            _featuresHandler = featuresHandler;
            _logger = logger;
        }
        public List<VertexCommand> GetCutoutCommandsFrom(BasePart part, string partName)
        {
            _commands = new List<VertexCommand>();

            //var features = FindFeaturesBy<NormalCutout>(part);
            var features = _featuresHandler.FindBy<CutoutReader>(part, "Normal Cutout");

            // if part dont contain cutouts return - list without elements
            if (features == null || features.Count == 0)
            {
                return _commands;
            }
            // find notch in features
            foreach (var feature in features)
            {
                if (feature.GetUserAttributeAsString("Type", NXObject.AttributeType.String, -1) == "Notch")
                {
                    // TODO: add method
                    Line[] lines = _featuresHandler.GetGeometryFrom(feature).Select(x => _featuresHandler.CastNxObjectTo<Line>(x)).ToArray();
                   CreateNotchCommand(lines, partName);
                }
            }
            // Error 
            if (features.Count != _commands.Count)
            {
                Program.ErMessages.Add(new ErrorMessage { SectId = partName, Message = $"Some cutouts cant finded, input amount - {features.Count}, output amount - {_commands.Count}" });
                _logger.Warn($"Some cutouts cant finded, input amount - {features.Count}, output amount - {_commands.Count}");
            }

            return _commands;
        }
        private void CreateNotchCommand(Line[] lines, string partName)
        {
            // Find midpoint on axis X in line where start and end points is not equal

            var midpoint = lines.Where(x => x.StartPoint.X != x.EndPoint.X).Select(x => (x.StartPoint.X + x.EndPoint.X) / 2).Max();

            _commands.Add(new VertexCommand(partName, Operations.Notch, midpoint));
        }
    }
}
