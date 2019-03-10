using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    class BCCCoreException : ApplicationException  {
        public BCCCoreException() : base(){ }

        public BCCCoreException(string message) : base(message){ }

        public BCCCoreException(string message, Exception innerException) : base(message, innerException) { }
    }
    
}
