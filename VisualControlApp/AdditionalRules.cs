using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VisualControlApp
{
    class AdditionalRules
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private List<PresentVertexSection> _outputSections;
        private PresentVertexSection _section;
        public List<PresentVertexSection> AddAddintionalRulesInSections(List<PresentVertexSection> sections)
        {
            _outputSections = new List<PresentVertexSection>();
            var retVal = new List<PresentVertexSection>();
            var orderedList = new List<PresentVertexSection>();
            foreach (var section in sections)
            {
                _section = section;
                LipAndNotchRule();
                ChamferRule();
                ThreeNotchRule();
                
                var b = _section.CommandsCollection.Where(x => x.Operation == "Chamfer").GroupBy(y => y.Ordinate).Select(z => z.First()).ToList();
                section.CommandsCollection.RemoveAll(x => x.Operation == "Chamfer");
                section.CommandsCollection.AddRange(b);
                _section.CommandsCollection =_section.CommandsCollection.OrderBy(x => x.Ordinate).ToList();
                retVal.Add(_section);
            }
            ImplementRotationRules(retVal);
            return _outputSections;


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
                if (item.Operation == "Notch")
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
            if (_section.CommandsCollection.Any(x=>x.Operation == "Chamfer"))
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
                if (c.Length >= 2 && c.Any(x => x.Operation == "Notch") && c.Any(x => x.Operation == "Lip"))
                {

                    return true;
                }
            }
            return false;
        }
        private void AddSwage(double distance)
        {
            if (_section.CommandsCollection.Any(x=>x.Operation =="Swage"))
            {
                return;
            }
            _section.CommandsCollection.Add(new PresentVertexCommand { Operation = "Swage", Ordinate = distance });
            _section.CommandsCollection.Add(new PresentVertexCommand { Operation = "Swage", Ordinate = _section.Width - distance });
        }
        private void ImplementRotationRules(List<PresentVertexSection> sections)
        {
            var bindex = 0;
            for (int i = 0; i < sections.Count; i++)
            {
              
                if (sections[i].CommandsCollection.Any(x => x.Operation == "Chamfer"))
                {
                    try
                    {
                        bindex = sections.IndexOf(sections.Where(x => x.Width == sections.Max(y => y.Width)).First());
                        // Longest section in pool
                        _outputSections.Add(sections[bindex]);
                        // Fil before section with chamfer
                        _outputSections.Add(new PresentVertexSection(sections[i]));
                        // Section with chamfer
                        _outputSections.Add(sections[i]);
                        // Fil before section with chamfer
                        _outputSections.Add(new PresentVertexSection(sections[i]));
                        // Remove from pool long section
                        sections.RemoveAt(bindex);
                        // remove from pool section with chamfer
                        sections.RemoveAt(i);
                        // reset counter
                        i = 0;

                    }
                    catch (Exception ex)
                    {

                        _logger.Fatal(ex);
                        break;
                    }
                    
                }
            }
            for (int i = 0; i < sections.Count; i++)
            {
                if (IsNotchAndLipInOneCoordinate(sections[i]))
                {
                    try
                    {
                        bindex = sections.IndexOf(sections.Where(x => x.Width > 1600).First());
                        _outputSections.Add(sections[bindex]);
                        _outputSections.Add(new PresentVertexSection(sections[i]));
                        _outputSections.Add(sections[i]);
                        _outputSections.Add(new PresentVertexSection(sections[i]));
                        sections.RemoveAt(bindex);
                        sections.RemoveAt(i);
                        i = 0;
                    }
                    catch (Exception ex)
                    {
                     
                        _logger.Error(ex, "Not found sections whrere width > 1600");
                        break;
                    }
                }
            }
            _outputSections.AddRange(sections);
        }
    }
}
