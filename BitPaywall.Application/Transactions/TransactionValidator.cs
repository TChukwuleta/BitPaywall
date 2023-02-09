using BitPaywall.Application.Common.Interfaces.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Transactions
{
    public class TransactionValidator
    {
    }

    public class TransactionRequestValidator : AbstractValidator<ITransactionRequestValidator>
    {
        public TransactionRequestValidator()
        {
            RuleFor(c => c.Amount).NotEmpty().WithMessage("Amount must be specified");
            RuleFor(c => c.TransactionType).NotEmpty().WithMessage("Transaction must be specified");
            RuleFor(c => c.Sender).NotEmpty().WithMessage("Sender must be specified");
            RuleFor(c => c.Recipient).NotEmpty().WithMessage("Recipient must be specified");
        }
    }
}
