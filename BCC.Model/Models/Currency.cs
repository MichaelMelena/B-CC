using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class Currency
    {
        public string IsoName { get; set; }
        public int TicketId { get; set; }
        public float Buy { get; set; }
        public float? Sell { get; set; }

        public virtual CurrencyMetadata IsoNameNavigation { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
