using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Entities;
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
    public class GetAccountByAccountNumberQuery : IRequest<Result>
    {
        public string AccountNumber { get; set; }
        public string UserId { get; set; }
    }

    public class GetAccountByAccountNumberQueryHandler : IRequestHandler<GetAccountByAccountNumberQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;

        public GetAccountByAccountNumberQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetAccountByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            var account = new Account();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve account by account number. Invalid user details specified");
                }
                var userAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.AccountNumber == request.AccountNumber && c.UserId == request.UserId);
                if (userAccount == null)
                {
                    account = await _context.Accounts.Select(c => new Account
                    {
                        AccountNumber = c.AccountNumber,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                    }).FirstOrDefaultAsync(c => c.AccountNumber == request.AccountNumber);
                    if (account == null)
                    {
                        return Result.Failure("Unable to retrieve account by account number. Invalid account number specified");
                    }
                }
                else
                {
                    account = userAccount;
                }
                return Result.Success("Account details retrieval was successful", account);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Account retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
