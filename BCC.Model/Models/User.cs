using System;
using System.Collections.Generic;

namespace BCC.Model.Models
{
    public partial class User
    {
        public User()
        {
            TrackedCurrency = new HashSet<TrackedCurrency>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastLogin { get; set; }

        public virtual ICollection<TrackedCurrency> TrackedCurrency { get; set; }
    }
}
