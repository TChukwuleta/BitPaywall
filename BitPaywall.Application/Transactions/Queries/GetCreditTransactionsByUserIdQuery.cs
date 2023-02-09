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
    public class GetCreditTransactionsByUserIdQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetCreditTransactionsByUserIdQueryHandler : IRequestHandler<GetCreditTransactionsByUserIdQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;

        public GetCreditTransactionsByUserIdQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetCreditTransactionsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var response = new List<Transaction>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Credit transactions retrieval was not successful. Invalid user details specified");
                }
                var creditTransactions = await _context.Transactions.Where(c => c.TransactionType == TransactionType.Credit && c.UserId == request.UserId).ToListAsync();
                if(creditTransactions.Count() <= 0)
                {
                    return Result.Failure("No credit transactions found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    response = creditTransactions;
                }
                else
                {
                    response = creditTransactions.Skip(request.Skip).Take(request.Take).ToList();
                }
                var entity = new
                {
                    Entity = response,
                    Count = creditTransactions.Count()
                };
                return Result.Success("Credit transactions retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User credit transactions retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
