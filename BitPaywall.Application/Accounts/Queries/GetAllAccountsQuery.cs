using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Accounts.Queries
{
    public class GetAllAccountsQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
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
                object entity = default;
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
                if (request.Skip == 0 && request.Take == 0)
                {
                    entity = new
                    {
                        Accounts = accounts,
                        Count = accounts.Count()
                    };
                }
                else
                {
                    entity = new
                    {
                        Accounts = accounts.Skip(request.Skip).Take(request.Take).ToList(),
                        Count = accounts.Count()
                    };
                }
                return Result.Success("Accounts retrieval was successful.", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Accounts retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message }");
            }
        }
    }
}
