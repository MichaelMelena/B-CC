using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class Ticket
    {
        public Ticket()
        {
            Currency = new HashSet<Currency>();
        }

        public int Id { get; set; }
        public string BankShortName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }

        public virtual Bank BankShortNameNavigation { get; set; }
        public virtual ICollection<Currency> Currency { get; set; }
    }
}
