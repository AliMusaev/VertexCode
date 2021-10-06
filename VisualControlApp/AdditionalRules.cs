using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VertexCodeMakerDomain;

namespace VisualControlApp
{
    class AdditionalRules
    {
        private ErrorMessageStore _errorStore;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private List<PresentVertexSection> _outputSections;
        private List<PresentVertexSection> _sections;
        private PresentVertexSection _section;
        public AdditionalRules()
        {
            _errorStore = ErrorMessageStore.GetStore();
        }
        public List<PresentVertexSection> AddAddintionalRulesInSections(List<PresentVertexSection> sections)
        {
            _sections = sections;
            _outputSections = new List<PresentVertexSection>();
            var retVal = new List<PresentVertexSection>();
            var orderedList = new List<PresentVertexSection>();
            foreach (var section in _sections)
            {
                _section = section;
                // Remove dublicated chamfers
                // TODO: FIX bug double writing chamfers
                


                DimplePositionScale();
                LipAndNotchRule();
                ChamferRule();
                ThreeNotchRule();

                //_section.CommandsCollection = _section.CommandsCollection.OrderBy(x => x.Ordinate).ToList();
                retVal.Add(_section);
            }
            OrderWithChamferRule();
            OrderWithNotchAndLipRule();
            
            _outputSections.AddRange(_sections);
            return _outputSections;


        }
        private void DimplePositionScale()
        {
            var first = _section.CommandsCollection.FirstOrDefault(x => x.Operation == Operations.Dimple && x.Ordinate < 17.4);
            var last = _section.CommandsCollection.FirstOrDefault(x => x.Operation == Operations.Dimple && _section.Width - x.Ordinate < 17.4);
            double dif1 = 0;
            double dif2 = 0;
            
            if (first != null)
            {
                dif1 = Math.Abs(first.Ordinate - 17.4);
                _section.X1 = _section.X1 - dif1 * _section.CosX;
                _section.Y1 = _section.Y1 - dif1 * _section.SinX;
                first.Ordinate = first.Ordinate + dif1;
            }
            if (last != null)
            {
                dif2 = Math.Abs(_section.Width - last.Ordinate - 17.4);
                _section.X2 =_section.X2 + dif2 * _section.CosX;
                _section.Y2 = _section.Y2 + dif2 * _section.SinX;
                // update section width 
                _section.Width = _section.Width + dif1 + dif2;
                last.Ordinate = last.Ordinate +  dif1;
            }

            UpdateChamferPositionAfterScale();
        }
        
        private void UpdateChamferPositionAfterScale()
        {
            var chamfers = _section.CommandsCollection.Where(x => x.Operation == Operations.Chamfer).ToArray();
            if (chamfers.Length > 0)
                chamfers.First(x => x.Ordinate == chamfers.Max(y => y.Ordinate)).Ordinate = _section.Width;
        }
        private void ThreeNotchRule()
        {
            int counter = 0;
            foreach (var item in _section.CommandsCollection)
            {
                if (counter == 3)
                {
                    AddSwage(15);
                }
                if (item.Operation == Operations.Notch)
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }
            }
        }
        private void ChamferRule()
        {
            if (_section.CommandsCollection.Any(x => x.Operation == Operations.Chamfer))
            {
                AddSwage(33);
            }
        }
        private void LipAndNotchRule()
        {
            if (IsNotchAndLipInOneCoordinate(_section))
            {
                AddSwage(15);
            }
        }
        private bool IsNotchAndLipInOneCoordinate(PresentVertexSection section)
        {
            var b = section.CommandsCollection.GroupBy(x => x.Ordinate).Where(y => y.Count() > 1);
            foreach (var item in b)
            {
                var c = item.ToArray();
                // Notch and lip (and something - dimple or service)
                if (c.Length >= 2 && c.Any(x => x.Operation == Operations.Notch) && c.Any(x => x.Operation == Operations.Lip))
                {

                    return true;
                }
            }
            return false;
        }
        private void AddSwage(double distance)
        {
            if (_section.CommandsCollection.Any(x => x.Operation == Operations.Swage))
            {
                return;
            }
            _section.AddCommand(new PresentVertexCommand { Operation = Operations.Swage,Ordinate = distance });
            _section.AddCommand(new PresentVertexCommand { Operation = Operations.Swage, Ordinate = _section.Width - distance });
        }
        
        private void OrderWithChamferRule()
        {
            var chamferSections = _sections.Where(y => y.CommandsCollection.Any(x => x.Operation == Operations.Chamfer)).ToList();
            foreach (var item in chamferSections)
            {
                var bindex = FindSectionIdWhereWidthMax(_sections.IndexOf(item));
                _outputSections.Add(_sections[bindex]);
                _outputSections.Add(new PresentVertexSection(item));
                _outputSections.Add(item);
                _outputSections.Add(new PresentVertexSection(item));
                RemovePairFromSectionsList(bindex, _sections.IndexOf(item));
            }

        }
        private void OrderWithNotchAndLipRule()
        {
            if (_sections.Count < 2)
            {
                return;
            }
            var sections = _sections.Where(y => IsNotchAndLipInOneCoordinate(y)).ToList();
            foreach (var item in sections)
            {
                var bindex = FindSectionIdWhereWidthGreaterThen(1600, _sections.IndexOf(item));
                _outputSections.Add(_sections[bindex]);
                _outputSections.Add(item);
                RemovePairFromSectionsList(bindex, _sections.IndexOf(item));
            }
        }
        private void RemovePairFromSectionsList(int a, int b)
        {
            var elemA = _sections[a];
            var elemb = _sections[b];
            _sections.Remove(elemA);
            _sections.Remove(elemb);
        }
        private int FindSectionIdWhereWidthGreaterThen(int value, int ignoreIndex)
        {
            var indexes = _sections.Where(x => x.Width > value).Select(y=>_sections.IndexOf(y)).ToArray();

            try
            {
                return indexes.First(x => x != ignoreIndex);
            }
            catch (Exception)
            {
                _errorStore.AddMessage(new ErrorMessage() { Message = "Not found sections whrere width > 1600. Sorting rules changes. Now will be find max width element" });
                MessageBox.Show($"Not found sections whrere width > 1600. Sorting rules changes. Now will be find max width element");
                return FindSectionIdWhereWidthMax(ignoreIndex);

            }
        }
        private int FindSectionIdWhereWidthMax(int ignoreIndex)
        {
            var indexes = _sections.Where(x => x.Width == _sections.Max(y => y.Width)).Select(y => _sections.IndexOf(y)).ToArray();
            try
            {
                return indexes.First(x => x != ignoreIndex);
            }
            catch (Exception ex)
            {
                _errorStore.AddMessage(new ErrorMessage() { Message = "Fatal error! Please contact with program author. System dont find sections" });
                MessageBox.Show($"Fatal error! Please contact with program author. System dont find sections");
                _logger.Fatal(ex, "Fatal error! Please contact with program author. System dont find sections");
                throw ex;
            }
            
        }
    }
     
}
