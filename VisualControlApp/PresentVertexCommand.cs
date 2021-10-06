using VertexCodeMakerDomain;

namespace VisualControlApp
{
    public class PresentVertexCommand 
    {
        public string ParentName { get; set; }
        public Operations Operation { get; set; }
        public double Ordinate { get; set; }
    }
}