using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class TrackedCurrency
    {
        public int UserId { get; set; }
        public string IsoName { get; set; }
        public DateTime Created { get; set; }

        public virtual CurrencyMetadata IsoNameNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
