using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core.CNB
{
    class CNBException : BCCCoreException
    {
        public CNBException() : base() { }

        public CNBException(string message) : base(message) { }

        public CNBException(string message, Exception innerException) : base(message, innerException) { }
    }

    class CNBInvalidDate : CNBException
    {
        public CNBInvalidDate(string message) : base(message) { }

        public CNBInvalidDate() : base() { }
    }

    class CNBInvalidData: CNBException
    {
        public CNBInvalidData(): base() { }

        public CNBInvalidData(string message): base(message) { }
    }

}
