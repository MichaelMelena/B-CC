using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    class ERMetadataBase : ICurrencyMetada
    {
        public string ISOName { get; protected set; }

        public string Country { get; protected set; }

        public string Name { get; protected set; }

        public int Quantity { get; protected set; }

        public ERMetadataBase(string isoName, string country, string name, int quantity)
        {
            ISOName = isoName;
            Country = country;
            Name = name;
            Quantity = quantity;
        }
    }
}
