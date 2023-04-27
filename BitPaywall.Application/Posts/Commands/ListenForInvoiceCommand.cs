using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Transactions.Commands;
using BitPaywall.Core.Entities;
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
                if (listener == null)
                {
                    return Result.Failure("An error occured.");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == listener.PostId);
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
                var engagedPost = new EngagedPost
                {
                    CreatedDate = DateTime.Now,
                    PostId = post.Id,
                    Post = post,
                    UserId = listener.UserId,
                    Status = Core.Enums.Status.Active
                };
                await _context.EngagedPosts.AddAsync(engagedPost);

                var activityPost = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.PostId == post.Id);
                if (activityPost == null)
                {
                    var newPostActivity = new PostAnalytic
                    {
                        Status = Core.Enums.Status.Active,
                        PostId = post.Id,
                        AmountGenerated = post.Amount
                    };
                    await _context.PostAnalytics.AddAsync(newPostActivity);
                }
                else
                {
                    activityPost.AmountGenerated = activityPost.AmountGenerated + post.Amount;
                    _context.PostAnalytics.Update(activityPost);
                }

                var transactionRequest = new CreateTransactionCommand
                {
                    Description = "Payment for a post",
                    DebitAccount = account.AccountNumber,
                    CreditAccount = authorsAccount.AccountNumber,
                    Amount = post.Amount,
                    TransactionType = Core.Enums.TransactionType.Debit,
                    UserId = listener.UserId
                };
                var transaction = await new CreateTransactionCommandHandler(_authService, _context).Handle(transactionRequest, cancellationToken);
                var transactionMessage = string.IsNullOrEmpty(transaction.Message) ? transaction.Messages.FirstOrDefault() : transaction.Message;
                if (!transaction.Succeeded)
                {
                    return Result.Failure(transactionMessage);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Invoice has been confirmed. You can now read the post", post);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Invoice confirmation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
