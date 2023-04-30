using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Accounts.Queries
{
    public class GetUserAccountQuery : IRequest<Result>
    {
        public string UserId { get; set; }
    }

    public class GetUserAccountQueryHandler : IRequestHandler<GetUserAccountQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetUserAccountQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetUserAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Accounts retrieval was not successful. Invalid user details");
                }
                var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (account == null)
                {
                    return Result.Failure("No account exists for user");
                }
                return Result.Success("Account retrieval was successful.", account);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Account retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
