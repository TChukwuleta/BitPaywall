using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Transactions.Commands
{
    public class CreateTransactionCommand : IRequest<Result>, IBaseValidator
    {
        public string Description { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string UserId { get; set; }
    }

    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public CreateTransactionCommandHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var reference = $"BitPaywall_{DateTime.Now.Ticks}";
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transaction creation failed. Invalid user details");
                }
                var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.AccountNumber == request.DebitAccount);
                if (account == null)
                {
                    return Result.Failure("Unable to create transaction. Account does not exist for this useer");
                }

                if (account.Balance < request.Amount)
                {
                    return Result.Failure("Insufficient funds. Kindly topup your account to continue");
                }
                var creditAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.AccountNumber == request.CreditAccount);
                if (creditAccount == null)
                {
                    return Result.Failure("Invalid credit account specified");
                }

                var entity = new Transaction
                {
                    DebitAccount = request.DebitAccount,
                    CreditAccount = request.CreditAccount,
                    UserId = request.UserId,
                    Amount = request.Amount,
                    TransactionType = request.TransactionType,
                    TransactionReference = reference,
                    TransactionStatus = TransactionStatus.Success,
                    Narration = request.Description,
                    CreatedDate = DateTime.Now,
                    AccountId = account.Id
                };
                await _context.Transactions.AddAsync(entity);

                creditAccount.Balance += request.Amount;
                account.Balance -= request.Amount;
                _context.Accounts.Update(creditAccount);
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Transaction creation was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transactions creation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
