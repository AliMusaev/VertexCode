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
        private ProfileMarker _marker;
        public OutputBuilder()
        {

        }
       
        public string Build(VertexFrame frame)
        {
            _marker = new ProfileMarker();
            string objname = "BoxName\n";
            string retVal = objname;
            
            foreach (var section in frame.GetSections())
            {
                retVal += WriteSection(section, frame.Name);
            }
            return retVal;
        }
       
      
        private string WriteSection(VertexSection section, string frameId)
        {
            double GA = 0.6;
            int quantity = 1;
            string DE = $"{section.Mark}-{section.Direction} ";
            string fill = $"IN= CABIN NAME " +
                $"CU=Vertex Systems UK " +
                $"GA={GA.ToString(CultureInfo.InvariantCulture)} " +
                $"PR={section.SectionName} " +
                $"PA={frameId} " +
                $"QT={quantity} " +
                $"MM=50 " +
                $"DE=FIL\n";
           string sectionText = $"IN= CABIN NAME " +
                $"CU=Vertex Systems UK " +
                $"GA={GA.ToString(CultureInfo.InvariantCulture)} " +
                $"PR={section.SectionName} " +
                $"PA={frameId} " +
                $"QT={quantity} " +
                $"MM={section.Width.ToString(CultureInfo.InvariantCulture)} " +
                $"DE={DE} " +
                $"X1={Math.Round(section.StartPoint.X,1).ToString(CultureInfo.InvariantCulture)} " +
                $"Y1={Math.Round(section.StartPoint.Y,1).ToString(CultureInfo.InvariantCulture)} " +
                $"X2={Math.Round(section.EndPoint.X,1).ToString(CultureInfo.InvariantCulture)} " +
                $"Y2={Math.Round(section.EndPoint.Y,1).ToString(CultureInfo.InvariantCulture)} " +
                $"HI={ChangeOrientation(section.Height, section.Direction).ToString(CultureInfo.InvariantCulture)}\n";
            int counter = 0;
            foreach (var item in section.CommandsCollection.OrderBy(x => x.Ordinate))
            {
                if (counter == 2)
                {
                    sectionText += $"-1,100.0,{DE}\n";
                }
                sectionText += $"{item.Operation},{Math.Round(item.Ordinate,1).ToString(CultureInfo.InvariantCulture)}\n";
                counter++;
            }


            if (section.CommandsCollection.Any(x => x.Operation == Operations.Chamfer))
            {
                return fill + sectionText + fill;
            }
            else
            {
                return sectionText;
            }
        }
        private double ChangeOrientation(double hight, ShelvsDirection direction)
        {
            if (direction == ShelvsDirection.D || direction == ShelvsDirection.L)
            {
                return hight * -1;
            }
            return hight;
        }
        
    }
}
