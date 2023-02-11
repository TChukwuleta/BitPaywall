using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Accounts.Queries
{
    public class GetAllAccountsQuery : IRequest<Result>, IBaseValidator
    {
        public string UserId { get; set; }
    }

    public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;

        public GetAllAccountsQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Accounts retrieval was not successful. Invalid user details");
                }
                var accounts = await _context.Accounts.ToListAsync();
                if (accounts.Count() <= 0)
                {
                    return Result.Failure("Accounts retrieval was not successful. No accounts currently exist on the system");
                }
                return Result.Success("Accounts retrieval was successful.", accounts);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Account retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
