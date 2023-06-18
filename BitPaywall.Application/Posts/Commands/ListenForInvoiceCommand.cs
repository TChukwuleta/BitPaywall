using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Transactions.Commands;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Commands
{
    public class ListenForInvoiceCommand : IRequest<Result>
    {
    }

    public class ListenForInvoiceCommandHandler : IRequestHandler<ListenForInvoiceCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ILightningService _lightningService;
        private readonly IAppDbContext _context;

        public ListenForInvoiceCommandHandler(IAuthService authService, ILightningService lightningService, IAppDbContext context)
        {
            _authService = authService;
            _lightningService = lightningService;
            _context = context;
        }

        public async Task<Result> Handle(ListenForInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var listener = await _lightningService.ListenForSettledInvoice();
                var fundingTypeResponse = listener.Type.Split('|');
                var fundingType = fundingTypeResponse[0];
                var fundingId = fundingTypeResponse[1];
                Enum.TryParse(fundingType, out PaymentType type);
                int.TryParse(fundingId, out int id);
                switch (type)
                {
                    case PaymentType.Funding:
                        var userAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.Id == id);
                        if (userAccount != null)
                        {
                            var fundingTransactionRequest = new CreateTransactionCommand
                            {
                                Description = "account funding",
                                CreditAccount = userAccount.AccountNumber,
                                Amount = listener.AmountInSat,
                                TransactionType = TransactionType.Credit,
                                UserId = listener.UserId
                            };
                            var fundingTransaction = await new CreateTransactionCommandHandler(_authService, _context).Handle(fundingTransactionRequest, cancellationToken);
                            var fundingTransactionMessage = string.IsNullOrEmpty(fundingTransaction.Message) ? fundingTransaction.Messages.FirstOrDefault() : fundingTransaction.Message;
                            if (!fundingTransaction.Succeeded)
                            {
                                return Result.Failure(fundingTransactionMessage);
                            }
                        }
                        break;
                    case PaymentType.Purchase:
                        var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == id);
                        var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == listener.UserId);
                        if (account == null)
                        {
                            return Result.Failure("Invalid user account. Kindly create a new account.");
                        }
                        var authorsAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == post.UserId);
                        if (authorsAccount == null)
                        {
                            return Result.Failure("Invalid author account. Please contact support.");
                        }
                        var transactionRequest = new CreateTransactionCommand
                        {
                            Description = "Payment for a post",
                            DebitAccount = account.AccountNumber,
                            CreditAccount = authorsAccount.AccountNumber,
                            Amount = post.Amount,
                            TransactionType = TransactionType.Debit,
                            UserId = listener.UserId
                        };
                        var transaction = await new CreateTransactionCommandHandler(_authService, _context).Handle(transactionRequest, cancellationToken);
                        var transactionMessage = string.IsNullOrEmpty(transaction.Message) ? transaction.Messages.FirstOrDefault() : transaction.Message;
                        if (!transaction.Succeeded)
                        {
                            return Result.Failure(transactionMessage);
                        }
                        var engagedPost = new EngagedPost
                        {
                            CreatedDate = DateTime.Now,
                            PostId = post.Id,
                            Post = post,
                            UserId = listener.UserId,
                            Status = Status.Active
                        };
                        await _context.EngagedPosts.AddAsync(engagedPost);

                        var activityPost = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.PostId == post.Id);
                        if (activityPost == null)
                        {
                            var newPostActivity = new PostAnalytic
                            {
                                Status = Status.Active,
                                PostId = post.Id,
                                ReadCount = 0,
                                AmountGenerated = post.Amount
                            };
                            await _context.PostAnalytics.AddAsync(newPostActivity);
                        }
                        else
                        {
                            activityPost.AmountGenerated += post.Amount;
                            _context.PostAnalytics.Update(activityPost);
                        }
                        break;
                    default:
                        break;
                }
                if (listener == null)
                {
                    return Result.Failure("An error occured.");
                }
                
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Invoice has been confirmed");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Invoice confirmation was not successful. {ex?.Message ?? ex?.InnerException.Message }");
            }
        }
    }
}
