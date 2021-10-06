using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain
{
    public class ErrorMessageStore
    {
        private static readonly ErrorMessageStore instance = new ErrorMessageStore();
        private List<ErrorMessage> _errorsCollection;
        public IReadOnlyList<ErrorMessage> ErrorsCollection { get { return _errorsCollection.AsReadOnly(); } }
        private ErrorMessageStore()
        {
            _errorsCollection = new List<ErrorMessage>();
        }
        public void AddMessage(ErrorMessage message)
        {
            _errorsCollection.Add(message);
        }
        public static ErrorMessageStore GetStore()
        {
            return instance;
        }
    }
}
