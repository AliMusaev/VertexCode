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
        public List<PresentVertexSection> AdaptData(VertexFrame frame, List<ErrorMessage> messages)
        {

            var retVal = new List<PresentVertexSection>();
            foreach (var item in frame.GetSections())
            {

                retVal.Add(ConvertSection(item, messages));
            }
            return retVal;
        }
        private PresentVertexSection ConvertSection(VertexSection section, List<ErrorMessage> messages)
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
            rSection.X1 = Math.Round(section.StartPoint.X, 3);
            rSection.Y1 = Math.Round(section.StartPoint.Y, 3);
            rSection.X2 = Math.Round(section.EndPoint.X, 3);
            rSection.Y2 = Math.Round(section.EndPoint.Y, 3);
            rSection.HI = ChangeOrientation(section.Height, section.Direction);
            rSection.Direction = section.Direction.ToString();
            rSection.CosX = section.CosX;
            rSection.CosY = section.CosY;
            rSection.SinX = section.SinX;
            rSection.SinY = section.SinY;
            rSection.Length = section.Length;
            rSection.Height = section.Height;
            rSection.Thickness = section.Thickness;
            rSection.ExtensionsString = FindErrorMessages(messages, section.SectionName);
            rSection.DE = $"{rSection.Mark}-{rSection.Direction}";
            rSection.CommandsCollection = ConvertCommands(section.CommandsCollection).OrderBy(x => x.Ordinate).ToList();
            if (rSection.ExtensionsString != null)
            {
                MessageBox.Show(rSection.ExtensionsString);
                rSection.LoadWithError = true;
            }
            return rSection;

        }
        private string FindErrorMessages(List<ErrorMessage> messages, string sectName)
        {
            string retVal = null;
            foreach (var item in messages)
            {
                if (item.SectId == sectName)
                {
                    retVal += item.Message + "\n";
                }
            }
            return retVal;
        }
        private List<PresentVertexCommand> ConvertCommands(IReadOnlyList<VertexCommand> commands)
        {
            return commands.Select(x => new PresentVertexCommand { ParentName = x.ParentName, Operation = x.Operation.ToString(), Ordinate = Math.Round(x.Ordinate, 3) }).ToList();
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
