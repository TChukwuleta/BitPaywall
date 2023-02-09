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
                };
                await _context.Transactions.AddAsync(entity);
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
