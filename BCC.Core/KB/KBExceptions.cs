using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core.KB
{
    class KBExceptions : BCCCoreException
    {
        public KBExceptions() : base() { }

        public KBExceptions(string message) : base(message) { }

        public KBExceptions(string message, Exception innerException) : base(message, innerException) { }
    }

    class KBInvalidDate : KBExceptions
    {
        public KBInvalidDate(string message) : base(message) { }

        public KBInvalidDate() : base() { }
    }

    class KBInvalidData : KBExceptions
    {
        public KBInvalidData() : base() { }

        public KBInvalidData(string message) : base(message) { }
    }

}
