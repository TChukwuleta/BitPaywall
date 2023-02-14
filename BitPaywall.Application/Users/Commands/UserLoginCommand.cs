using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators.UserValidator;
using BitPaywall.Core.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Users.Commands
{
    public class UserLoginCommand : IRequest<Result>, IUserLoginValidator
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result>
    {
        private readonly IAuthService _authService;
        public UserLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _authService.Login(request.Email, request.Password);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User login was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
