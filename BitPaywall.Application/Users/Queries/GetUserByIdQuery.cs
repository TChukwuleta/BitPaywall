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
    public class GetUserByIdQuery : IRequest<Result>, IBaseValidator
    {
        public string UserId { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        public GetUserByIdQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.GetUserById(request.UserId);
                if (result.user == null)
                {
                    return Result.Failure("No user found");
                }
                return Result.Success(result.user);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Getting user by Id was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
