using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Model.Request
{
    public class TransactionRequest : ITransactionRequestValidator
    {
        public string Description { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
    }
}
