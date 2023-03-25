using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.EngagedPosts.Commands
{
    public class DeactivateEngagedPostCommand : AuthToken, IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class DeactivateEngagedPostCommandHandler : IRequestHandler<DeactivateEngagedPostCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public DeactivateEngagedPostCommandHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(DeactivateEngagedPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Deactivating post status failed. Invalid user details specified");
                }
                var statusUpdateRequest = new UpdateEngagedPostStatusCommand
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    Status = Status.Deactivated
                };
                var updatePostStatus = await new UpdateEngagedPostStatusCommandHandler(_context, _authService).Handle(statusUpdateRequest, cancellationToken);
                var updatePostStatusResponse = string.IsNullOrEmpty(updatePostStatus.Message) ? updatePostStatus.Messages.FirstOrDefault() : updatePostStatus.Message;
                if (!updatePostStatus.Succeeded)
                {
                    return Result.Failure(updatePostStatusResponse);
                }
                return Result.Success(updatePostStatusResponse);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Deactivating posts was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
