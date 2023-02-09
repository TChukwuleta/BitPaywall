using BitPaywall.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Entities
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public Status Status { get; set; }
    }
}
