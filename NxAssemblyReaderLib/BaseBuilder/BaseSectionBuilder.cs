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
using VertexCodeMakerDomain.Interfaces;

namespace NxAssemblyReaderLib.BaseBuilder
{
    class BaseSectionBuilder
    {

        private Logger _logger = LogManager.GetCurrentClassLogger();
        private FeaturesHandler _featuresHandler;
        private ServiceReader _serviceReader;
        private CutoutReader _cutoutReader;
        private FlangeReader _flangeReader;
        public BaseSectionBuilder()
        {
            _featuresHandler = new FeaturesHandler();
            _serviceReader = new ServiceReader(_featuresHandler, _logger);
            _cutoutReader = new CutoutReader(_featuresHandler, _logger);
            _flangeReader = new FlangeReader(_featuresHandler, _logger);
        }
        public List<BaseSection> Build(BasePart[] parts, string assemblyName)
        {
            List<BaseSection> bases = new List<BaseSection>();
            
            foreach (var part in parts)
            {
                var name = FindPartName(part);
                // ignore Assembly part
                if (name != assemblyName)
                {
                    bases.Add(BuildBaseSecionsBy(part, name));
                }
            }
            return bases;
        }
        private BaseSection BuildBaseSecionsBy(BasePart part,string name)
        {
            var commands = new List<VertexCommand>();
            // Get commands and sort by ordinate distance
            BaseAttributes bases = _flangeReader.GeteSizeAttributeFrom(part, name);
            commands.AddRange(_cutoutReader.GetCutoutCommandsFrom(part, name));
            commands.AddRange(_serviceReader.GetServiceCommandsFrom(part, name));
            return new BaseSection(name, commands, bases.Width, bases.Hight, bases.Length, bases.Thickness);
        }
        private string FindPartName(BasePart part)
        {
            try
            {
                return part.GetUserAttributeAsString("DB_PART_NAME", NXObject.AttributeType.String, -1);
            }
            catch (Exception)
            {

                _logger.Warn($"DB name not found of part {part.Name}");
            }
            return part.Name;
        }
     
        
       
        
        
       
    }
}
