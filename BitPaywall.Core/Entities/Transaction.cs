using BitPaywall.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Entities
{
    public class Transaction : AuditableEntity
    {
        public int AccountId { get; set; }
        public string? DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TransactionReference { get; set; }
        public string UserId { get; set; }
    }
}
