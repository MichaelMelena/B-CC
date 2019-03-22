using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core.CSOB
{
    class CSOBExceptions : BCCCoreException
    {
        public CSOBExceptions() : base() { }

        public CSOBExceptions(string message) : base(message) { }

        public CSOBExceptions(string message, Exception innerException) : base(message, innerException) { }
    }

    class CSOBInvalidDate : CSOBExceptions
    {
        public CSOBInvalidDate(string message) : base(message) { }

        public CSOBInvalidDate() : base() { }
    }

    class CSOBInvalidData : CSOBExceptions
    {
        public CSOBInvalidData() : base() { }

        public CSOBInvalidData(string message) : base(message) { }
    }

}
