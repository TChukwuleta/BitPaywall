using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Application.Transactions.Commands;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Commands
{
    public class PayForPostCommand : AuthToken, IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public PaymentModeType PaymentModeType { get; set; }
        public string UserId { get; set; }
    }

    public class PayForPostCommandHandler : IRequestHandler<PayForPostCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        private readonly ILightningService _lightningService;
        public PayForPostCommandHandler(IAuthService authService, IAppDbContext context, ILightningService lightningService)
        {
            _authService = authService;
            _context = context;
            _lightningService = lightningService;
        }

        public async Task<Result> Handle(PayForPostCommand request, CancellationToken cancellationToken)
        {
            string message;
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Post creation was not successful. Invalid user details specified");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (post == null)
                {
                    return Result.Failure("Invalid post details");
                }
                if (post.UserId == request.UserId)
                {
                    return Result.Success("You cannot pay for your own post.", post);
                }
                var engagedPost = await _context.EngagedPosts.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.PostId == post.Id);
                if (engagedPost != null)
                {
                    var postAnalytics = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.PostId == request.Id);
                    if (postAnalytics == null)
                    {
                        var newPostAnalytics = new PostAnalytic
                        {
                            CreatedDate = DateTime.Now,
                            Status = Status.Active,
                            PostId = request.Id,
                            ReadCount = 1
                        };
                        await _context.PostAnalytics.AddAsync(newPostAnalytics);
                    }
                    else
                    {
                        postAnalytics.ReadCount += 1;
                        _context.PostAnalytics.Update(postAnalytics);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success("You already have access to this post", engagedPost);
                }
                var authorAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == post.UserId);
                if (authorAccount == null)
                {
                    return Result.Failure("Invalid author account");
                }

                switch (request.PaymentModeType)
                {
                    case PaymentModeType.Bitcoin:
                        return Result.Success("Coming soon. Please choose another payment method");
                        break;
                    case PaymentModeType.Lightning:
                        var invoice = await _lightningService.CreateInvoice((long)post.Amount, $"{post.Id}/{request.UserId}", UserType.Admin);
                        if (string.IsNullOrEmpty(invoice))
                        {
                            return Result.Failure("An error occured while generating invoice");
                        }
                        return Result.Success("Invoice generated successfully", invoice);
                    case PaymentModeType.Fiat:
                        return Result.Success("Not supported yet. Please choose another payment method");
                    case PaymentModeType.Account:
                        var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                        if (account == null)
                        {
                            return Result.Failure("Invalid account details");
                        }
                        if (account.Balance < post.Amount)
                        {
                            return Result.Failure("Cannot pay for post with account. Insufficient funds.");
                        }
                        account.Balance -= post.Amount;
                        _context.Accounts.Update(account);
                        var transactionRequest = new CreateTransactionCommand
                        {
                            Description = "Payment for a post",
                            DebitAccount = account.AccountNumber,
                            CreditAccount = authorAccount.AccountNumber,
                            Amount = post.Amount,
                            TransactionType = TransactionType.Debit,
                            UserId = request.UserId
                        };
                        var transaction = await new CreateTransactionCommandHandler(_authService, _context).Handle(transactionRequest, cancellationToken);
                        var transactionMessage = string.IsNullOrEmpty(transaction.Message) ? transaction.Messages.FirstOrDefault() : transaction.Message;
                        if (!transaction.Succeeded)
                        {
                            return Result.Failure(transactionMessage);
                        }
                        var newEngagedPost = new EngagedPost
                        {
                            CreatedDate = DateTime.Now,
                            PostId = post.Id,
                            Post = post,
                            UserId = request.UserId,
                            Status = Status.Active
                        };
                        await _context.EngagedPosts.AddAsync(newEngagedPost);
                        message = "You have successfully paid for this post with account successfully";
                        break;
                    default:
                        return Result.Success("No payment mode selected");
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success(message, post);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post payment was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
