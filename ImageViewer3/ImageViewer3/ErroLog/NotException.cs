using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLog
{
    [Serializable()]
    class NotException : System.Exception
    {
        public NotException() : base() { }
        public NotException(string message) : base(message) { }
        public NotException(string message, System.Exception inner) : base(message, inner) { }

        protected NotException(System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
        {
        }
    }
}
