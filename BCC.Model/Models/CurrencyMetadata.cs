using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class CurrencyMetadata
    {
        public CurrencyMetadata()
        {
            Currency = new HashSet<Currency>();
            TrackedCurrency = new HashSet<TrackedCurrency>();
        }

        public string IsoName { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Currency> Currency { get; set; }
        public virtual ICollection<TrackedCurrency> TrackedCurrency { get; set; }
    }
}
