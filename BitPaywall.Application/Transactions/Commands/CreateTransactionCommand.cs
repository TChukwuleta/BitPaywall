using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Transactions.Commands
{
    public class CreateTransactionCommand : IRequest<Result>, IBaseValidator
    {
        public string? Description { get; set; }
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
                var creditAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.AccountNumber == request.CreditAccount);
                if (creditAccount == null)
                {
                    return Result.Failure("Invalid credit account specified");
                }
                if (!string.IsNullOrEmpty(request.DebitAccount))
                {
                    var account = await _context.Accounts.FirstOrDefaultAsync(c => c.AccountNumber == request.DebitAccount);
                    if (account == null)
                    {
                        return Result.Failure("Unable to create transaction. Account does not exist for this useer");
                    }
                    account.Balance -= request.Amount;
                    _context.Accounts.Update(account);
                    var debitEntity = new Transaction
                    {
                        DebitAccount = request.DebitAccount,
                        CreditAccount = request.CreditAccount,
                        UserId = account.UserId,
                        Amount = request.Amount,
                        TransactionType = TransactionType.Debit,
                        TransactionReference = reference,
                        TransactionStatus = TransactionStatus.Success,
                        Narration = request.Description,
                        CreatedDate = DateTime.Now,
                        AccountId = account.Id
                    };
                    await _context.Transactions.AddAsync(debitEntity);
                    account.Balance -= request.Amount;
                    _context.Accounts.Update(account);
                }
                var entity = new Transaction
                {
                    DebitAccount = string.IsNullOrEmpty(request.DebitAccount) ? "" : request.DebitAccount,
                    CreditAccount = request.CreditAccount,
                    UserId = creditAccount.UserId,
                    Amount = request.Amount,
                    TransactionType = TransactionType.Credit,
                    TransactionReference = reference,
                    TransactionStatus = TransactionStatus.Success,
                    Narration = request.Description,
                    CreatedDate = DateTime.Now,
                    AccountId = creditAccount.Id
                };
                await _context.Transactions.AddAsync(entity);
                switch (entity.TransactionType)
                {
                    case TransactionType.Debit:
                        creditAccount.Balance -= request.Amount;
                        break;
                    case TransactionType.Credit:
                        creditAccount.Balance += request.Amount;
                        break;
                    default:
                        break;
                }
                _context.Accounts.Update(creditAccount);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Transaction creation was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Transactions creation was not successful. {ex?.Message ?? ex?.InnerException.Message }");
            }
        }
    }
}
