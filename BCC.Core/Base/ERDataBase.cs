using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class ERDataBase : ICurrencyData
    {
        public string ISOName { get; protected set; }

        public string Country { get; protected set; }

        public string Name { get; protected set; }

        public int Quantity { get; protected set; }

        public float Buy { get; protected set; }

        public float? Sell { get; protected set; }

        public ERDataBase(string isoName,  string name, string country, int quantity, float buy, float? sell)
        {
            ISOName = isoName;
            Name = name;
            Country = country;
            Quantity = quantity;
            Buy = buy;
            Sell = sell;
        }

        public ERDataBase(ICurrencyMetada metaData, float buy, float? sell)
        {
            ISOName = metaData.ISOName;
            Name = metaData.Name;
            Country = metaData.Country;
            Quantity = metaData.Quantity;
            Buy = Buy;
            Sell = sell;
        }
        public ICurrencyMetada GetMetadata()
        {
            return new ERMetadataBase(ISOName, Country, Name, Quantity);
        }
    }
}
