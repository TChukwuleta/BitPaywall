using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators.UserValidator;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Accounts.Commands
{
    public class CreateAccountCommand : IRequest<Result>, IUserNamesValidator
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
    }

    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;

        public CreateAccountCommandHandler(IAuthService authService, IAppDbContext context, IConfiguration config)
        {
            _authService = authService;
            _config = config;
            _context = context;
        }

        public async Task<Result> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Account creation was not successful. Invalid user details");
                }

                var existingAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.FirstName.ToLower() == request.FirstName.ToLower() && c.LastName.ToLower() == request.LastName.ToLower());
                if (existingAccount != null)
                {
                    return Result.Failure("An account already exist for this user");
                }
                var accountPrefix = _config["AccountNumberPrefix"];
                var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                // Trim to 7 if greater than 7 else add trailing zeros
                timeStamp = Math.Abs(int.Parse(timeStamp.Length >= 7 ? timeStamp.Substring(timeStamp.Length - 7) : timeStamp)).ToString();
                var accountNum = accountPrefix + timeStamp;
                if (accountNum.Length < 10)
                {
                    accountNum.PadRight(10, '0');
                }
                var account = new Account
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    AccountNumber = accountNum,
                    Balance = default,
                    UserId = request.UserId,
                    CreatedDate = DateTime.Now
                };
                var userAccount = await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Account creation was successful", userAccount);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Account creation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
