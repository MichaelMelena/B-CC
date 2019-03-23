using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class BankConnector
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BankShortName { get; set; }
        public string DllName { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }

        public virtual Bank BankShortNameNavigation { get; set; }
    }
}
