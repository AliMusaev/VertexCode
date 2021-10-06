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
    class BaseAttributesReader
    {

        private FeaturesHandler _featuresHandler;
        private Logger _logger=  LogManager.GetCurrentClassLogger();
        private BasePart _currentPart;
        private string _currentPartName;
        private ContourFlange _flange;
        private ErrorMessageStore _errorStore;
        private double _width;
        private double _hight;
        private double _length;
        private double _thickness;
        public BaseAttributesReader(FeaturesHandler featuresHandler)
        {
            _errorStore = ErrorMessageStore.GetStore();
            _featuresHandler = featuresHandler;
        }

        public bool FindFlange(BasePart part, string partName)
        {
            ResetFilds();
            _currentPart = part;
            _currentPartName = partName;
            try
            {
                _flange = _currentPart.Features.ToArray().Where(x => x.FeatureType == "BCFLANGE").First() as ContourFlange;
                GetSizeAttributes();
                return true;
            }
            catch (Exception ex)
            {
                _errorStore.AddMessage(new ErrorMessage() { SectionName = partName, Message = $"ContourFlange dont find in {_currentPartName}" });
                _logger.Error(ex, $"part \"{_currentPartName}\" dont contain feature BCFLANGE");
                return false;
            }
        }
        public double GetWidth()
        {
            return Math.Round(_width, 3);
        }
        public double GetHight()
        {
            return Math.Round(_hight, 3);
        }
        public double GetLength()
        {
            return Math.Round(_length, 3);
        }
        public double GetThickness()
        {
            return Math.Round(_thickness, 3);
        }

        private void GetSizeAttributes()
        {
            Line[] lines = GetAllLinesFromFlange();

            _hight = GetLongestLineLengthFrom(SelectVerticalLines(lines));
            _width = GetExpressionValueByName("Width");
            _length = GetLongestLineLengthFrom(SelectHorizontalLines(lines));
            _thickness = GetExpressionValueByName("Thickness");
        }
        
        private Line[] GetAllLinesFromFlange()
        {
           return _featuresHandler.GetGeometryFrom(_flange).Select(x => _featuresHandler.CastNxObjectTo<Line>(x)).ToArray();
        }
        private Line[] SelectVerticalLines(Line[] lines)
        {
            return lines.Where(x => Math.Round(x.StartPoint.Z, 3) != Math.Round(x.EndPoint.Z, 3) && Math.Round(x.StartPoint.Y, 3) == Math.Round(x.EndPoint.Y, 3)).ToArray();
        }
        private Line[] SelectHorizontalLines(Line[] lines)
        {
            return lines.Where(x => Math.Round(x.StartPoint.Y, 3) != Math.Round(x.EndPoint.Y, 3) && Math.Round(x.StartPoint.Z, 3) == Math.Round(x.EndPoint.Z, 3)).ToArray();
        }
        private double GetLongestLineLengthFrom(Line[] lines)
        {
            try
            {
                return lines.Max(x => x.GetLength());
            }
            catch (Exception)
            {

                throw;
            }
        }
        private double GetExpressionValueByName(string name)
        {
            return _flange.GetExpressions().Where(x => x.Description.Contains(name)).First().Value;
        }

        private void ResetFilds()
        {
            _currentPart = null;
            _currentPartName = null;
            _flange = null;
            _width = 0;
            _hight = 0;
            _length = 0;
            _thickness = 0;
        }
    }
}
