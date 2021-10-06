using NLog;
using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VertexCodeMakerDomain;

namespace NxAssemblyReaderLib.BaseBuilder
{
    class CommandsReader
    {
        private List<VertexCommand> _commands;
        private ErrorMessageStore _errorStore;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private FeaturesHandler _fHandler;
        private string _currentPartName;
        private BasePart _currentBasePart;
        public CommandsReader(FeaturesHandler featuresHandler)
        {
            _errorStore = ErrorMessageStore.GetStore();
            _fHandler = featuresHandler;
        }

        public List<VertexCommand> GetCommandsFrom(BasePart part, string partName)
        {
            _commands = new List<VertexCommand>();
            _currentBasePart = part;
            _currentPartName = partName;
            FindServicesInPart();
            FindCutoutInPart();
            return _commands;

        }
        private void FindServicesInPart()
        {
            List<Feature> features = _fHandler.FindIn<HolePackage>(_currentBasePart, "HOLE PACKAGE");
            // if part dont contain cutouts return - list without elements
            if (features == null || features.Count == 0)
            {
                return;
            }
            // Try cast features to HolePackage objects
            foreach (var hole in _fHandler.CastFeaturesTo<HolePackage>(features))
            {
                CreateServiceBy(hole);
            }

        }
        private void CreateServiceBy(HolePackage hole)
        {
            hole.GetOrigins(out Point3d[] points);
            if (points.Length != 1)
            {
                _errorStore.AddMessage(new ErrorMessage { SectionName = _currentPartName, Message = $"Service \"{hole.Name}\" dont contain points or more then 1" });
                _logger.Warn($"Service \"{hole.Name}\" dont contain points or more then 1");

            }
            else
            {
                _commands.Add(new VertexCommand(_currentPartName, Operations.Service, points[0].X));
            }

        }
        private void FindCutoutInPart()
        {
            List<Feature> features = _fHandler.FindIn<NormalCutout>(_currentBasePart, "Normal Cutout");
            // if part dont contain cutouts return - list without elements
            if (features == null || features.Count == 0)
            {
                return;
            }
            foreach (var feature in features)
            {
                if (feature.GetUserAttributeAsString("Type", NXObject.AttributeType.String, -1) == "Notch")
                {
                    CreateNotchCommandBy(feature);
                }
                if (feature.GetUserAttributeAsString("Type", NXObject.AttributeType.String, -1) == "Service")
                {
                    CreateService(feature);
                }
                if (feature.GetUserAttributeAsString("Type", NXObject.AttributeType.String, -1) == "Dimple")
                {
                    CreateDimple(feature);
                }
            }
        }
        private void CreateNotchCommandBy(Feature feature)
        {
           
            Line[] lines = _fHandler.GetGeometryFrom(feature).Select(x => _fHandler.CastNxObjectTo<Line>(x)).ToArray();
            // Find midpoint on axis X in line where start and end points is not equal
            var midpoint = lines.Where(x => x.StartPoint.X != x.EndPoint.X).Select(x => (x.StartPoint.X + x.EndPoint.X) / 2).Max();

            _commands.Add(new VertexCommand(_currentPartName, Operations.Notch, midpoint));
        }
        private void CreateService(Feature feature)
        {

            var a  = _fHandler.GetGeometryFrom(feature).Select(x => _fHandler.CastNxObjectTo<Arc>(x)).ToArray();
            if (a.Length != 1)
            {
                MessageBox.Show("GG");
            }

            _commands.Add(new VertexCommand(_currentPartName, Operations.Service, a[0].CenterPoint.X));
        }
        private void CreateDimple(Feature feature)
        {

            var a = _fHandler.GetGeometryFrom(feature).Select(x => _fHandler.CastNxObjectTo<Arc>(x)).ToArray();
            if (a.Length != 1)
            {
                MessageBox.Show($"Section {_currentPartName}. Sketch in Normal cutout with Type \"Service\" contain Arc elements more than 1 or don contain anything");
                _errorStore.AddMessage(new ErrorMessage { SectionName = _currentPartName, Message = $"Sketch in Normal cutout with Type \"Service\" contain Arc elements more than 1 or don contain anything" });
                _logger.Warn($"Section {_currentPartName}. Sketch in Normal cutout with Type \"Service\" contain Arc elements more than 1 or don contain anything");
            }
            _commands.Add(new VertexCommand(_currentPartName, Operations.Dimple, a[0].CenterPoint.X));
        }

    }
}
