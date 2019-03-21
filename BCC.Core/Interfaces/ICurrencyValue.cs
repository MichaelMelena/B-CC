using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public interface ICurrencyValue
    {
        float Buy { get; }
        float? Sell { get; }

    }
}
