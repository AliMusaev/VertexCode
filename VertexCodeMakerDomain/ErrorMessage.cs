using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain
{
    public class ErrorMessage
    {

        public string SectionName { get; set; }
        public string Message { get; set; }
        public ErrorMessage()
        {

        }
        public ErrorMessage(string section, string message)
        {
            SectionName = section;
            Message = message;
        }
    }
}
