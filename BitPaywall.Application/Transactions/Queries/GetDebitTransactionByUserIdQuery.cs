using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Transactions.Queries
{
    public class GetDebitTransactionByUserIdQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }
    public class GetDebitTransactionByUserIdQueryHandler : IRequestHandler<GetDebitTransactionByUserIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetDebitTransactionByUserIdQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetDebitTransactionByUserIdQuery request, CancellationToken cancellationToken)
        {
            var response = new List<Transaction>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Cannot retrieve debit transactions. Invalid user details specified");
                }
                var debitTransactions = await _context.Transactions.Where(c => c.UserId == request.UserId && c.TransactionType == TransactionType.Debit).ToListAsync();
                if (debitTransactions.Count() <= 0)
                {
                    return Result.Failure("No debit transactions found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    response = debitTransactions;
                }
                else
                {
                    response = debitTransactions.Skip(request.Skip).Take(request.Take).ToList();
                }

                var entity = new
                {
                    Entity = response,
                    Count = debitTransactions.Count()
                };
                return Result.Success("Debit transactions retrieval was not successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User debit transactions retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
