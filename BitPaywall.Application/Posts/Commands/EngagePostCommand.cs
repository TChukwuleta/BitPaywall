using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Application.Lightning.Commands;
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
    public class EngagePostCommand : IRequest<Result>, IBaseValidator
    {
        public int PostId { get; set; }
        public string Invoice { get; set; }
        public string UserId { get; set; }
    }

    public class EngagePostCommandHandler : IRequestHandler<EngagePostCommand, Result>
    {
        private readonly ILightningService _lightningService;
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;

        public EngagePostCommandHandler(ILightningService lightningService, IAuthService authService, IAppDbContext context)
        {
            _lightningService = lightningService;
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(EngagePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to pay for post. Invalid user details specified");
                }
                var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (account == null)
                {
                    return Result.Failure("Unable to pay for post. User account details does not exist");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.PostId && c.PostType == Core.Enums.PostType.Published);
                if (post == null)
                {
                    return Result.Failure("Unable to pay for post. Invalid post specified");
                }
                var creditAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == post.UserId);
                if (creditAccount == null)
                {
                    return Result.Failure("Unable to pay for post. Post owner account does not exist");
                }
                var invoicePaymentRequest = new PayInvoiceCommand
                {
                    UserId = request.UserId,
                    PaymentRequest = request.Invoice
                };

                var paidInvoice = await new PayInvoiceCommandHandler(_authService, _lightningService).Handle(invoicePaymentRequest, cancellationToken);
                var paidInvoiceErrorMessage = string.IsNullOrEmpty(paidInvoice.Message) ? paidInvoice.Messages.FirstOrDefault() : paidInvoice.Message;
                if (!paidInvoice.Succeeded)
                {
                    return Result.Failure(paidInvoiceErrorMessage);
                }
                var engagedPost = new EngagedPost
                {
                    CreatedDate = DateTime.Now,
                    PostId = request.PostId,
                    Post = post,
                    UserId = request.UserId,
                    Status = Core.Enums.Status.Active
                };
                await _context.EngagedPosts.AddAsync(engagedPost);

                var transactionRequest = new CreateTransactionCommand
                {
                    Description = "Payment for a post",
                    DebitAccount = account.AccountNumber,
                    CreditAccount = creditAccount.AccountNumber,
                    Amount = post.Amount,
                    TransactionType = Core.Enums.TransactionType.Debit,
                    UserId = request.UserId
                };

                var transaction = await new CreateTransactionCommandHandler(_authService, _context).Handle(transactionRequest, cancellationToken);
                var transactionMessage = string.IsNullOrEmpty(transaction.Message) ? transaction.Messages.FirstOrDefault() : transaction.Message;
                if (!transaction.Succeeded)
                {
                    return Result.Failure(transactionMessage);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Invoice paid for successfully", engagedPost);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Invoice payment was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
