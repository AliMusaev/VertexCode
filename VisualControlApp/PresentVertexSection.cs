using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualControlApp
{
    class PresentVertexSection : IPresentVertex
    {
        private List<PresentVertexCommand> _commandsCollection;
        public IReadOnlyList<PresentVertexCommand> CommandsCollection { get { return _commandsCollection.AsReadOnly(); } }
        public string SectionName { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Thickness { get; set; }
        public double GA { get; set; }
        public string CU { get; set; }
        public int QT { get; set; }
        public string DE { get; set; }
        public double HI { get; set; }
        private double _x1;
        private double _y1;
        private double _x2;
        private double _y2;
        public double X1
        {
            get {
                return _x1;
            }
            set
            {
                _x1 = Math.Round(value, 1);
            }
        }
        public double Y1
        {
            get
            {
                return _y1;
            }
            set
            {
                _y1 = Math.Round(value, 1);
            }
        }
        public double X2
        {
            get
            {
                return _x2;
            }
            set
            {
                _x2 = Math.Round(value, 1);
            }
        }
        public double Y2
        {
            get
            {
                return _y2;
            }
            set
            {
                _y2 = Math.Round(value, 1);
            }
        }
        private double _cosX;
        private double _sinX;
        private double _cosY;
        private double _sinY;
        public double CosX
        {
            get
            {
                return _cosX;
            }
            set
            {
                _cosX = Math.Round(value, 3);
            }
        }
        public double SinX
        {
            get
            {
                return _sinX;
            }
            set
            {
                _sinX = Math.Round(value, 3);
            }
        }
        public double CosY
        {
            get
            {
                return _cosY;
            }
            set
            {
                _cosY = Math.Round(value, 3);
            }
        }
        public double SinY
        {
            get
            {
                return _sinY;
            }
            set
            {
                _sinY = Math.Round(value, 3);
            }
        }
        public string Direction { get; set; }
        public string Mark { get; set; }
        public string Type { get; set; }
        public bool IsEmpty { get; set; }
        public string ExtensionsString { get; set; }
        public bool LoadWithError { get; set; }
        public bool IsOrthogonal { get; set; }
        public PresentVertexSection()
        {
            _commandsCollection = new List<PresentVertexCommand>();
            IsEmpty = false;
            DE = $"{Mark}-{Direction}";
        }
        
        public PresentVertexSection(PresentVertexSection section)
        {
            _commandsCollection = new List<PresentVertexCommand>();
            SectionName = section.SectionName;
            QT = section.QT;
            CU = section.CU;
            Width = 50;
            IsEmpty = true;
            DE = $"FIL";
        }
        public void AddCommand(PresentVertexCommand presentVertexCommand)
        {
            _commandsCollection.Add(new PresentVertexCommand { ParentName = presentVertexCommand.ParentName, Operation = presentVertexCommand.Operation, Ordinate = Math.Round(presentVertexCommand.Ordinate, 1) });
            Order();
        }
        public void AddCommands(List<PresentVertexCommand> commands)
        {
            _commandsCollection.AddRange(commands.Select(x=> new PresentVertexCommand { ParentName = x.ParentName, Operation = x.Operation, Ordinate = Math.Round(x.Ordinate, 1) }).ToList());
            Order();
        }
        private void Order()
        {
            _commandsCollection = _commandsCollection.OrderBy(x => x.Ordinate).ToList();
        }
    }
}
