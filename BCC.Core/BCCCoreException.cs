using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class BCCCoreException : ApplicationException  {
        public BCCCoreException() : base(){ }

        public BCCCoreException(string message) : base(message){ }

        public BCCCoreException(string message, Exception innerException) : base(message, innerException) { }
    }
    public class BCCWebclientException: BCCCoreException
    {
        public BCCWebclientException() : base() { }
        public BCCWebclientException(string message) : base(message) { }

        public BCCWebclientException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BCCERMNullReference:BCCCoreException
    {
        public BCCERMNullReference():base() { }
    }

    
}
