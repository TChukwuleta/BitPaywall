using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Entities
{
    public class Account : AuditableEntity
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string UserId { get; set; }
    }
}
