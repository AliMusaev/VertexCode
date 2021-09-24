using NXOpen;
using NXOpen.Features;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen.Assemblies;
using VertexCodeMakerDomain;
using VisualControlApp;
using System.Threading.Tasks;
using NXOpenUI;
using System.Linq;
using System.Windows;
using Expression = NXOpen.Expression;
using MessageBox = System.Windows.MessageBox;
using System.IO;
using NxAssemblyReaderLib.FrameBuilder;

namespace NxAssemblyReaderLib
{
    public class Program
    {
        private static UI _ui;
        private static Session _session;
        private static VertexFrameBuilder _vBuilder;
        private static BaseSectionBuilder _bBuilder;
        private static readonly Dictionary<Operations, double> dict = new Dictionary<Operations, double>()
        {
            { Operations.Service, 30},
            { Operations.Service1, 20},
            { Operations.Service2, 35},
        };
        private static void InitializeLib()
        {
            // Set environment
            EnvironmentController enviSetter = new EnvironmentController();
            enviSetter.NX1980Set();
            // Instantiate
            _vBuilder = new VertexFrameBuilder();
            _bBuilder = new BaseSectionBuilder();
            // Load Nx session
            _ui = UI.GetUI();
            _session = Session.GetSession();

        }
        public static void Main(string[] args)
        {
            InitializeLib();

            // Get All baseSections
            List<BaseSection> bSections = _bBuilder.Build(_session.Parts.ToArray(), dict, _session.Parts.Display.ComponentAssembly.RootComponent.DisplayName);
            Component[] assemblyComponents = _session.Parts.Display.ComponentAssembly.RootComponent.GetChildren();
            // Get all VertexSections (with world coodinate and direction)
          
            var frame = _vBuilder.BuildFrameBy(_session.Parts.Display.ComponentAssembly.RootComponent.DisplayName, assemblyComponents, bSections);
            
            string text = null;
            //foreach (var item in frame.IntersectionsList)
            //{
            //    text += $"l1 {item.Section1.SectionName} X1 {item.Section1.StartPoint.X} Y1 {item.Section1.StartPoint.Y} X2 {item.Section1.EndPoint.X} Y2 {item.Section1.EndPoint.Y}\n" +
            //        $"l2 {item.Section2.SectionName} X1 {item.Section2.StartPoint.X} Y1 {item.Section2.StartPoint.Y} X2 {item.Section2.EndPoint.X} Y2 {item.Section2.EndPoint.Y}\n" +
            //        $"POINT X {item.InterPoint.X} Y {item.InterPoint.Y}\n\n";
            //}
            File.WriteAllText("file.txt", text);
            MainWindow mainwindow = new MainWindow(frame);
            mainwindow.ShowDialog();
        }

      
        private static void ImplementRotationRules(List<VertexSection> sections)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                if (i - 1 >= 0)
                {
                    if (sections[i].CommandsCollection.Any(x => x.Operation == Operations.Chamfer))
                    {
                        var bindex = sections.IndexOf(sections.Where(z => z.IsSwapped == false).Where(x => x.Width == sections.Max(y => y.Width)).First());
                        sections[bindex].IsSwapped = true;
                        Extensions.Swap(sections, i - 1, bindex);
                    }
                    if (IsNotchAndLip(sections[i]))
                    {
                        var bindex = sections.IndexOf(sections.Where(z => z.IsSwapped == false).Where(x => x.Width > 1600).First());
                        sections[bindex].IsSwapped = true;
                        Extensions.Swap(sections, i - 1, bindex);
                    }
                }
                else
                {
                    if (sections[i].CommandsCollection.Any(x => x.Operation == Operations.Chamfer))
                    {
                        Extensions.Swap(sections, i, i + 1);
                    }
                    if (IsNotchAndLip(sections[i]))
                    {
                        Extensions.Swap(sections, i, i + 1);
                    }
                }

            }
        }
        private static bool IsNotchAndLip(VertexSection section)
        {
            var b = section.CommandsCollection.GroupBy(x => x.Ordinate).Where(y => y.Count() > 1);
            foreach (var item in b)
            {
                var c = item.ToArray();
                foreach (var s in c)
                {
                    MessageBox.Show($"{s.Operation} {s.Ordinate}");
                }
                // Notch and lip
                if (c.Length == 2 && c.Any(x => x.Operation == Operations.Notch) && c.Any(x => x.Operation == Operations.Lip))
                {
                    return true;
                }
            }
            return false;
        }
        public static int GetUnloadOption(string arg)
        {
            return ((int)Session.LibraryUnloadOption.Immediately);
        }
        
    }  
}
