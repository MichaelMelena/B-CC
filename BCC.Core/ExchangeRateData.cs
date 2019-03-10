using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class ExchangeRateData
    {
        public double Buy {  get;  protected set; }
        public double? Sell { get; protected set; }
        public int Quantity { get; protected set; }
        public string Name { get; protected set; }
        public string ShortName { get; protected set; }
        public string IsoName { get; protected set; }

        public ExchangeRateData(double buy, double? sell,int quantity, string name, string shortName, string isoName)
        {
            this.Buy = buy;
            this.Sell = sell;
            this.Quantity = quantity;
            this.Name = name;
            this.ShortName = shortName;
            this.IsoName = isoName;
        }
    }
}
