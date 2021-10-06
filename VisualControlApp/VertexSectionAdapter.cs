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
    class VertexSectionAdapter
    {
        ProfileMarker _profileMarker;
        public VertexSectionAdapter()
        {
            _profileMarker = new ProfileMarker();
        }
        public List<PresentVertexSection> AdaptData(VertexFrame frame)
        {

            var retVal = new List<PresentVertexSection>();
            foreach (var item in frame.SectionsCollection)
            {
                retVal.Add(ConvertSection(item));
            }
            return retVal;
        }
        private PresentVertexSection ConvertSection(VertexSection section)
        {
            double GA = 0.6;
            int quantity = 1;
            var rSection = new PresentVertexSection();
            rSection.CU = "Vertex Systems UK";
            rSection.GA = GA;
            rSection.QT = quantity;
            rSection.Mark = _profileMarker.GetNextMark().ToString();
            rSection.SectionName = Convert.ToByte(rSection.Mark[0]).ToString();
            rSection.Width = section.Width;
            rSection.X1 = section.StartPoint.X;
            rSection.Y1 = section.StartPoint.Y;
            rSection.X2 = section.EndPoint.X;
            rSection.Y2 = section.EndPoint.Y;
            rSection.HI = ChangeOrientation(section.Height, section.Direction);
            rSection.Direction = section.Direction.ToString();
            rSection.CosX = section.CosX;
            rSection.CosY = section.CosY;
            rSection.SinX = section.SinX;
            rSection.SinY = section.SinY;
            rSection.Length = section.Length;
            rSection.Height = section.Height;
            rSection.Thickness = section.Thickness;
            rSection.ExtensionsString = FindErrorMessages(section.SectionName);
            rSection.DE = $"{rSection.Mark}-{rSection.Direction}";
            rSection.AddCommands(ConvertCommands(section.CommandsCollection));
            rSection.IsOrthogonal = section.IsOrthogonal;
            if (rSection.ExtensionsString != null)
            {
                rSection.LoadWithError = true;
            }
            return rSection;
           
               

        }
        private string FindErrorMessages(string sectName)
        {
            var messages = ErrorMessageStore.GetStore().ErrorsCollection;
            string retVal = null;
            foreach (var item in messages)
            {
                if (item.SectionName == sectName)
                {
                    retVal += item.Message + "\n";
                }
            }
            return retVal;
        }
        private List<PresentVertexCommand> ConvertCommands(IReadOnlyList<VertexCommand> commands)
        {
            return commands.Select(x => new PresentVertexCommand { ParentName = x.ParentName, Operation = x.Operation, Ordinate = Math.Round(x.Ordinate, 3) }).ToList();
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
