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
        public static List<ErrorMessage> ErMessages { get; set; }
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static UI _ui;
        private static Session _session;
        private static VertexFrameBuilder _vBuilder;
        private static BaseSectionBuilder _bBuilder;
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
            ErMessages = new List<ErrorMessage>();



        }
        private static void ConfigurateLoggger()
        {
            var config = LogManager.Configuration;
           
            var log = new NLog.Targets.FileTarget("logfile1") { FileName = $"u:\\Musaev.Ali\\{Environment.MachineName}.log" };
            log.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=toString}";
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, log);
            LogManager.Configuration = config;
        }
        public static void Main(string[] args)
        {
            ConfigurateLoggger();
            InitializeLib();
            var a = _session.Parts.GetDisplayedParts();
            
            var assemblyName = FindPartName(_session.Parts.Display.ComponentAssembly.RootComponent.OwningPart);
           
            // Get All baseSections
            List<BaseSection> bSections = _bBuilder.Build(_session.Parts.ToArray(), assemblyName);
            Component[] assemblyComponents = _session.Parts.Display.ComponentAssembly.RootComponent.GetChildren();
            // Get all VertexSections (with world coodinate and direction)
          
            var frame = _vBuilder.BuildFrameBy(assemblyName, assemblyComponents, bSections);
            


            MainWindow mainwindow = new MainWindow(frame, ErMessages);
            mainwindow.ShowDialog();
        }

        private static string FindPartName(BasePart part)
        {
            try
            {
                return part.GetUserAttributeAsString("DB_PART_NAME", NXObject.AttributeType.String, -1);
            }
            catch (NXException ex)
            {
                _logger.Warn(ex, "DB name not found");
            }
            return part.Name;
        }
       
        public static int GetUnloadOption(string arg)
        {
            

            
            LogManager.Shutdown();
            return ((int)Session.LibraryUnloadOption.Immediately);
        }
        
    }  
}
