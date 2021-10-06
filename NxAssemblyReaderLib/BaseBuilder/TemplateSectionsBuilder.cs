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
    class TemplateSectionsBuilder
    {
        private ErrorMessageStore _errorStore;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private FeaturesHandler _featuresHandler;
        private CommandsReader _cReader;
        private BaseAttributesReader _bReader;
        public TemplateSectionsBuilder()
        {
            _featuresHandler = new FeaturesHandler();
            _cReader = new CommandsReader(_featuresHandler);
            _bReader = new BaseAttributesReader(_featuresHandler);
            _errorStore = ErrorMessageStore.GetStore();
        }
      
        public TemplateSection Build(BasePart part,string name)
        {

            // Get commands and sort by ordinate distance
            if (!_bReader.FindFlange(part, name))
            {
                _errorStore.AddMessage(new ErrorMessage() { SectionName = name, Message = "Section cant be loaded" });
                _logger.Error($"Section with name {name} cant be loaded");
                return null;
            }
            return new TemplateSection(name, _cReader.GetCommandsFrom(part, name), _bReader.GetWidth(), _bReader.GetHight(), _bReader.GetLength(), _bReader.GetThickness());
        }
     
    }
}
