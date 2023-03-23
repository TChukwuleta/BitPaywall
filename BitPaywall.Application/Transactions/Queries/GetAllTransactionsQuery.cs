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
    public class GetAllTransactionsQuery : AuthToken, IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetAllTransactionsQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            var response = new List<Transaction>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transactions retrieval was not successful. Invalid user details specified");
                }
                var transactions = await _context.Transactions.Where(c => c.UserId == request.UserId).ToListAsync();
                if(transactions.Count <= 0)
                {
                    return Result.Failure("No transactions found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    response = transactions;
                }
                else
                {
                    response = transactions.Skip(request.Skip).Take(request.Take).ToList();
                }
                var entity = new
                {
                    Entity = response,
                    Count = transactions.Count()
                };
                return Result.Success("Transactions retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User transactions retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
