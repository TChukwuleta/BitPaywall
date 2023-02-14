using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
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
    public class GetCreditTransactionByAccountNumberQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string AccountNumber { get; set; }
        public string UserId { get; set; }
    }

    public class GetCreditTransactionByAccountNumberQueryHandler : IRequestHandler<GetCreditTransactionByAccountNumberQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetCreditTransactionByAccountNumberQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetCreditTransactionByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            var transactions = new List<Transaction>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve transactions. Invalid user details specified");
                }
                var userTransactions = await _context.Transactions.Where(c => c.CreditAccount == request.AccountNumber).ToListAsync();
                if (userTransactions.Count() <= 0)
                {
                    return Result.Failure("Unable to retrieve transactions. No credit transactions found for this account number");
                }
                if (request.Skip == 0 && request.Take == 0) 
                {
                    transactions = userTransactions;
                }
                else
                {
                    transactions = userTransactions.Skip(request.Skip).Take(request.Take).ToList();
                }
                var entity = new
                {
                    Entity = transactions,
                    Count = userTransactions.Count()
                };
                return Result.Success("Transactions retrieved successfully", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transaction retrieval by account number was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
