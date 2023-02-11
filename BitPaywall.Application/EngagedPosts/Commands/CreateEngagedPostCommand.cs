using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
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
    public class CreateEngagedPostCommand : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class CreateEngagedPostCommandHandler : IRequestHandler<CreateEngagedPostCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;

        public CreateEngagedPostCommandHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(CreateEngagedPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Creating engaged posts failed. Invalid user details specified");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (post == null)
                {
                    return Result.Failure("Creating engaged posts failed. Invalid post specified");
                }
                var engagedPost = new EngagedPost
                {
                    CreatedDate = DateTime.Now,
                    Status = Status.Active,
                    PostId = request.Id,
                    Post = post,
                    Amount = post.Amount,
                    UserId = request.UserId
                };
                await _context.EngagedPosts.AddAsync(engagedPost);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Engaged post added successfully.", engagedPost);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Engaged post creation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
