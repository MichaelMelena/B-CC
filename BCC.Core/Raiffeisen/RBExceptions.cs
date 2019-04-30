using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core.RB
{
    class RBExceptions : BCCCoreException
    {
        public RBExceptions() : base() { }

        public RBExceptions(string message) : base(message) { }

        public RBExceptions(string message, Exception innerException) : base(message, innerException) { }
    }

    class RBInvalidDate : RBExceptions
    {
        public RBInvalidDate(string message) : base(message) { }

        public RBInvalidDate() : base() { }
    }

    class RBInvalidData :RBExceptions
    {
        public RBInvalidData() : base() { }

        public RBInvalidData(string message) : base(message) { }
    }

}
