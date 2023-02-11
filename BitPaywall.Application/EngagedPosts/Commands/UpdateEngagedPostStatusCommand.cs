using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.EngagedPosts.Commands
{
    public class UpdateEngagedPostStatusCommand : IRequest<Result>, IIdValidator, IStatusValidator
    {
        public int Id { get; set; }
        public Status Status { get; set; }
        public string UserId { get; set; }
    }

    public class UpdateEngagedPostStatusCommandHandler : IRequestHandler<UpdateEngagedPostStatusCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public UpdateEngagedPostStatusCommandHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(UpdateEngagedPostStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Updating engaged post failed. Invalid user details specified");
                }
                var engagedPost = await _context.EngagedPosts.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (engagedPost == null)
                {
                    return Result.Failure("Updating engaged post failed. Invalid post specified");
                }
                string message = default;
                switch (request.Status)
                {
                    case Status.Active:
                        engagedPost.Status = Status.Active;
                        message = "Post was activated successfully";
                        break;
                    case Status.Archived:
                        engagedPost.Status = Status.Archived;
                        message = "Post was archived successfully";
                        break;
                    case Status.Deactivated:
                        engagedPost.Status = Status.Deactivated;
                        message = "Post was deactivated successfully";
                        break;
                    default:
                        return Result.Failure("Invalid status type specified");
                }

                _context.EngagedPosts.Update(engagedPost);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success(message, engagedPost);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Engaged post status update was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
