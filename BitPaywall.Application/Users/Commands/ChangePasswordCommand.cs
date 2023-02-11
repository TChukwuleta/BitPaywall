using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Users.Commands
{
    public class ChangePasswordCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IAuthService _authService;
        public ChangePasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _authService.GetUserByEmail(request.Email);
                if (existingUser.user == null)
                {
                    return Result.Failure("Password change was not successful. Invalid user details");
                }
                return await _authService.ChangePasswordAsync(request.Email, request.OldPassword, request.NewPassword);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Password change was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
