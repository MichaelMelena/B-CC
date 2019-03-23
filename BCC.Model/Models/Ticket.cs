using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class Ticket
    {
        public int Id { get; set; }
        public string BankShortName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }

        public virtual Bank BankShortNameNavigation { get; set; }
    }
}
