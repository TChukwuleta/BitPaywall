using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Users.Queries
{
    public class GetAllUsersQuery : IRequest<Result>, IEmailValidator
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string Email { get; set; }
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result>
    {
        private readonly IAuthService _authService;
        public GetAllUsersQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.GetAllUsers(request.Skip, request.Take);
                if (result.users == null)
                {
                    return Result.Failure("No users found");
                }
                return Result.Success(result.users);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Getting all system users was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
