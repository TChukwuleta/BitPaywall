using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Accounts.Queries
{
    public class GetAccountByAccountIdQuery : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class GetAccountByAccountIdQueryHandler : IRequestHandler<GetAccountByAccountIdQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;

        public GetAccountByAccountIdQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        async Task<Result> IRequestHandler<GetAccountByAccountIdQuery, Result>.Handle(GetAccountByAccountIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Account retrieval was not successful. Invalid user details");
                }
                var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.Id == request.Id);
                if (account == null)
                {
                    return Result.Failure("Account retrieval was not successful. Invalid account specified");
                }
                return Result.Success("Account retrieval was successful.", account);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Account retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message }");
            }
        }
    }
}
