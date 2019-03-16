using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public interface ICurrencyMetada
    {
        string ISOName { get; }
        string Country { get; }
        string Name { get; }
        int Quantity { get;}
    }
}
