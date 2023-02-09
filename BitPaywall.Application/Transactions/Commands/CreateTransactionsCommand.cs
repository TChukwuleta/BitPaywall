using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Application.Common.Model.Request;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Transactions.Commands
{
    public class CreateTransactionsCommand : IRequest<Result>, IBaseValidator
    {
        public string UserId { get; set; }
        public int AccountId { get; set; }
        public List<TransactionRequest> Transactions { get; set; }
        public string Description { get; set; }
    }

    public class CreateTransactionsCommandHandler : IRequestHandler<CreateTransactionsCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public CreateTransactionsCommandHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(CreateTransactionsCommand request, CancellationToken cancellationToken)
        {
            var reference = $"BitPaywall_{DateTime.Now.Ticks}";
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transaction creation failed. Invalid user details");
                }
                var transactions = new List<Transaction>();
                foreach (var item in request.Transactions)
                {
                    this.ValidateItem(item);
                    var entity = new Transaction
                    {
                        UserId = request.UserId,
                        Amount = item.Amount,
                        CreditAccount = item.CreditAccount,
                        TransactionType = item.TransactionType,
                        TransactionReference = reference,
                        DebitAccount = item.DebitAccount,
                        TransactionStatus = TransactionStatus.Success,
                        AccountId = request.AccountId,
                        CreatedDate = DateTime.Now,
                        Narration = request.Description,
                        Status = Status.Active
                    };
                    transactions.Add(entity);
                }
                await _context.Transactions.AddRangeAsync(transactions);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Transactions created successfully", transactions);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transactions creation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }

        private void ValidateItem(TransactionRequest item)
        {
            TransactionRequestValidator validator = new TransactionRequestValidator();
            ValidationResult validationResult = validator.Validate(item);
            string validateError = null;
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                {
                    validateError += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage + "\n";
                }
                throw new Exception(validateError);
            }
        }
    }
}
