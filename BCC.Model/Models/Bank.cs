using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class Bank
    {
        public Bank()
        {
            Ticket = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}
