using NLog;
using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NxAssemblyReaderLib.BaseBuilder
{
    class FlangeReader
    {

        private FeaturesHandler _featuresHandler;
        private Logger _logger;
        public FlangeReader(FeaturesHandler featuresHandler, Logger logger)
        {
            _featuresHandler = featuresHandler;
            _logger = logger;
        }
        



        // TODO: ADD SAFETY COMPONENTS
        public BaseAttributes GeteSizeAttributeFrom(BasePart part , string partName)
        {
            BaseAttributes Attributes;

            var flange = GetContourFlangeFrom(part, partName);

            var lines = _featuresHandler.GetGeometryFrom(flange).Select(x => _featuresHandler.CastNxObjectTo<Line>(x)).ToArray();
            Line[] topLines;

            topLines = lines.Where(x => Math.Round(x.StartPoint.Z, 3) != Math.Round(x.EndPoint.Z, 3) && Math.Round(x.StartPoint.Y, 3) == Math.Round(x.EndPoint.Y, 3)).ToArray();

            Attributes.Hight = topLines.Max(x => x.GetLength());

            Attributes.Width = flange.GetExpressions().Where(x => x.Description.Contains("Width")).First().Value;
            topLines = lines.Where(x => Math.Round(x.StartPoint.Y, 3) != Math.Round(x.EndPoint.Y, 3) && Math.Round(x.StartPoint.Z, 3) == Math.Round(x.EndPoint.Z, 3)).ToArray();
            Attributes.Length = topLines.Max(x => x.GetLength());
            Attributes.Thickness = flange.GetExpressions().Where(x => x.Description.Contains("Thickness")).First().Value;
            return Attributes;
        }
        private ContourFlange GetContourFlangeFrom(BasePart part, string partName)
        {
            try
            {
                return part.Features.ToArray().Where(x => x.FeatureType == "BCFLANGE").First() as ContourFlange;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Attention ContourFlange dont find in {partName}. The program will be closed!");
                _logger.Error(ex, $"part \"{partName}\" dont contain feature BCFLANGE");
                throw;
                
            }
        }
    }
}
