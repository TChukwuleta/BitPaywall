using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Posts.Commands
{
    public class PublishPostCommand : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;

        public PublishPostCommandHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(PublishPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to publish post. Invalid user details specified.");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.Id == request.Id);
                if (post == null)
                {
                    return Result.Failure("Unable to publish post. Invalid post specified");
                }
                post.PostType = Core.Enums.PostType.Published;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Post published successfully", post);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Publishing post was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
