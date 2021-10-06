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
using NLog;
using NxAssemblyReaderLib.BaseBuilder;

namespace NxAssemblyReaderLib
{
    public class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static UI _ui;
        private static Session _session;
        private static BasePart[] _parts;
        private static Component[] _components;
        private static VertexFrameBuilder _vBuilder;
        
        private static string _assemblyName;
        private static void InitializeLib()
        {
            // Set environment
            EnvironmentController enviSetter = new EnvironmentController();
            enviSetter.NX1980Set();
            // Load Nx session
            _ui = UI.GetUI();
            _session = Session.GetSession();
            // Load necessary data
            _components = _session.Parts.Display.ComponentAssembly.RootComponent.GetChildren();
            _assemblyName = SectionName.Find(_session.Parts.Display.ComponentAssembly.RootComponent.OwningPart);
            // Fully load of components
            _session.Parts.LoadOptions.UsePartialLoading = false;
            _session.Parts.Display.ComponentAssembly.OpenComponents(ComponentAssembly.OpenOption.ComponentOnly, _components, out ComponentAssembly.OpenComponentStatus[] openstatus);
            _session.Parts.LoadOptions.UsePartialLoading = true;

            // Load parts after loading components
            _parts = _session.Parts.ToArray().Where(x => _components.Select(y => SectionName.Find(y)).Contains(SectionName.Find(x))).ToArray();
            // Instantiate
            _vBuilder = new VertexFrameBuilder();
            


        }
        private static void ConfigurateLoggger()
        {
            var config = LogManager.Configuration;
           
            var log = new NLog.Targets.FileTarget("logfile") { FileName = $"u:\\Musaev.Ali\\{Environment.MachineName}.{Environment.UserName}.log" };
            log.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=toString}";
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, log);
            LogManager.Configuration = config;
        }
        public static void Main(string[] args)
        {
            ConfigurateLoggger();
            InitializeLib();

            var templates = GetTemplates();
            
            _components = _components.Where(x => templates.Select(y => y.SectionName).Contains(SectionName.Find(x))).ToArray();

            
            List<VertexSection> vertexSections = CreateVertexSections(templates, _components, DetermineSecondAxis());
        
            VertexFrame frame = _vBuilder.BuildFrameBy(vertexSections, _assemblyName);
            MainWindow mainwindow = new MainWindow(frame);
            mainwindow.ShowDialog();
        }

        private static List<VertexSection> CreateVertexSections(List<TemplateSection> sections, Component[] components, SecondAxis axis)
        {
            VertexSectionBuilder sectionBuilder = new VertexSectionBuilder();
            List<VertexSection> retVal = new List<VertexSection>();
            foreach (var component in components)
            {
                try
                {
                    var name = SectionName.Find(component);
                    if (component.OwningPart.Name != name)
                    {
                        var template = sections.FirstOrDefault(x => x.SectionName.Equals(name, StringComparison.OrdinalIgnoreCase)).Clone() as TemplateSection;
                        component.GetPosition(out Point3d point, out Matrix3x3 matrix);
                        retVal.Add(sectionBuilder.BuildSectionBy(template, name, point, matrix, axis));
                    }
                }
                catch (Exception)
                {
                    _logger.Warn("Template cannot be find");
                    continue;
                }
              
            }
            return retVal;
        }
        private static List<TemplateSection> GetTemplates()
        {
            TemplateSectionsBuilder templateBuilder = new TemplateSectionsBuilder();
            List<TemplateSection> templates = new List<TemplateSection>();
            // Get All templates
            foreach (var part in _parts)
            {

                var partName = SectionName.Find(part);
                if (partName != _assemblyName)
                {
                    var template = templateBuilder.Build(part, partName);
                    if (template != null)
                    {
                        templates.Add(templateBuilder.Build(part, partName));
                    }
                }
            }
            return templates;
        }
        private static SecondAxis DetermineSecondAxis()
        {
            List<double> results = new List<double>();
            foreach (var item in _components)
            {
                item.GetPosition(out Point3d point, out Matrix3x3 matrix);
                results.Add(point.Y);
            }

            var b = results.GroupBy(x => Math.Round(x, 1)).ToList();
            if (b.Count == 1)
            {
                return SecondAxis.Z;
            }
            return SecondAxis.Y;
           
        }

        public static int GetUnloadOption(string arg)
        {
            LogManager.Shutdown();
            return ((int)Session.LibraryUnloadOption.Immediately);
        }
        
    }  
}
