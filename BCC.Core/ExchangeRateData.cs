using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    abstract class AExchangeRateData
    {
        public double Buy {  get;  protected set; }
        public double Sell { get; protected set; }
        
    }
}
