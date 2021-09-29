using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VertexCodeMakerDomain;

namespace VisualControlApp
{
    class OutputBuilder
    {

        public string Build(List<PresentVertexSection> sections, string cabinName, string frameName)
        {
            string retVal = cabinName + "\n";
            
            foreach (var section in sections)
            {
                retVal += WriteSection(section, frameName, cabinName);
            }
            return retVal;
        }


        private string WriteSection(PresentVertexSection section, string frameName, string cabinName)
        {
            string retVal = null;
            if (section.IsEmpty)
            {
                retVal = $"IN={cabinName} " +
                         $"CU={section.CU} " +
                         $"GA={section.GA.ToString(CultureInfo.InvariantCulture)} " +
                         $"PR={section.SectionName} " +
                         $"PA={frameName} " +
                         $"QT={section.QT} " +
                         $"MM={section.Width.ToString(CultureInfo.InvariantCulture)} " +
                         $"DE={section.DE}\n";
            }
            else
            {
                retVal = $"IN={cabinName} " +
              $"CU={section.CU} " +
              $"GA={section.GA.ToString(CultureInfo.InvariantCulture)} " +
              $"PR={section.SectionName} " +
              $"PA={frameName} " +
              $"QT={section.QT} " +
              $"MM={section.Width.ToString(CultureInfo.InvariantCulture)} " +
              $"DE={section.DE} " +
              $"X1={Math.Round(section.X1, 1).ToString(CultureInfo.InvariantCulture)} " +
              $"Y1={Math.Round(section.Y1, 1).ToString(CultureInfo.InvariantCulture)} " +
              $"X2={Math.Round(section.X2, 1).ToString(CultureInfo.InvariantCulture)} " +
              $"Y2={Math.Round(section.Y2, 1).ToString(CultureInfo.InvariantCulture)} " +
              $"HI={section.HI.ToString(CultureInfo.InvariantCulture)}\n";

                int counter = 0;
                foreach (var item in section.CommandsCollection.OrderBy(x => x.Ordinate))
                {
                    if (counter == 2)
                    {
                        retVal += $"-1,100.0,{section.DE}\n";
                    }
                    retVal += $"{item.Operation},{Math.Round(item.Ordinate, 1).ToString(CultureInfo.InvariantCulture)}\n";
                    counter++;
                }
            }
            return retVal;
        }
    }
}
