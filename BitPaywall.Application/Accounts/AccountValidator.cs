using BitPaywall.Application.Common.Interfaces.Validators.UserValidator;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Accounts
{
    public class AccountValidator
    {
    }

    public class AccountRequestValidator : AbstractValidator<IUserNamesValidator>
    {
        public AccountRequestValidator()
        {
            RuleFor(c => c.FirstName).NotEmpty().NotNull().WithMessage("First name is required");
            RuleFor(c => c.LastName).NotEmpty().NotNull().WithMessage("Last name must be specified");
            RuleFor(c => c.UserId).NotEmpty().NotNull().WithMessage("User id must be specified");
        }
    }
}
