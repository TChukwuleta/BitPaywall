using BitPaywall.Application.Common.Interfaces.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Posts
{
    public class PostValidator
    {
    }

    public class PostRequestValidator : AbstractValidator<IPostRequestValidator>
    {
        public PostRequestValidator()
        {
            RuleFor(c => c.Title).NotEmpty().WithMessage("Title must be speicified");
            RuleFor(c => c.Image).NotEmpty().WithMessage("Image must be specified");
            RuleFor(c => c.Story).NotEmpty().WithMessage("Story must be specified");
            RuleFor(c => c.Amount).NotEmpty().WithMessage("Amount must be specified");
            RuleFor(c => c.Description).NotEmpty().WithMessage("Description must be specified");
        }
    }
}
