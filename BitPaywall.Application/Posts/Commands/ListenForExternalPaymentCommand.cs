using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Transactions.Commands;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Posts.Commands
{
    public class ListenForExternalPaymentCommand : IRequest<Result>
    {
    }

    public class ListenForExternalPaymentCommandHandler : IRequestHandler<ListenForExternalPaymentCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ILightningService _lightningService;
        private readonly IAppDbContext _context;
        public ListenForExternalPaymentCommandHandler(IAuthService authService, ILightningService lightningService, IAppDbContext context)
        {
            _authService = authService;
            _lightningService = lightningService;
            _context = context;
        }

        public async Task<Result> Handle(ListenForExternalPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var listener = await _lightningService.ListenForSettledInvoice();
                if (listener == null)
                {
                    return Result.Failure("An error occured.");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == listener.PostId);
                var authorsAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == listener.UserId);
                if (authorsAccount == null)
                {
                    return Result.Failure("Invalid author account. Kindly create a new account.");
                }

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
                var postAnalytics = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.PostId == post.Id);
                if (postAnalytics == null)
                {
                    var newPostAnalytics = new PostAnalytic
                    {
                        CreatedDate = DateTime.Now,
                        Status = Core.Enums.Status.Active,
                        PostId = post.Id,
                        AmountGenerated = post.Amount,
                    };
                    await _context.PostAnalytics.AddAsync(newPostAnalytics);
                }
                else
                {
                    postAnalytics.AmountGenerated += post.Amount;
                    _context.PostAnalytics.Update(postAnalytics);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Invoice has been confirmed. You can now read the post", post);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
