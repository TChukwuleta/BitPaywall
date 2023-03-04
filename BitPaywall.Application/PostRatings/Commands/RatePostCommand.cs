using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
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

namespace BitPaywall.Application.PostRatings.Commands
{
    public class RatePostCommand : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public string UserId { get; set; }
    }

    public class RatePostCommandHandler : IRequestHandler<RatePostCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        public RatePostCommandHandler(IAppDbContext context, IAuthService authService, IConfiguration config)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(RatePostCommand request, CancellationToken cancellationToken)
        {
            var maxRating = _config["PostRating:MaximumValue"];
            var minRating = _config["PostRating:MinimumValue"];
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to publish post. Invalid user details specified.");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (post == null)
                {
                    return Result.Failure("Unable to rate post. Invalid post specified");
                }
                if (request.Rate < int.Parse(minRating))
                {
                    return Result.Failure("Invalid rating. Rate too low");
                }
                if (request.Rate > int.Parse(maxRating))
                {
                    return Result.Failure("Invalid rating. Rate too high");
                }
                var alreadyRatedPost = await _context.PostRatings.FirstOrDefaultAsync(c => c.PostId == request.Id && c.UserId == request.UserId);
                if (alreadyRatedPost != null)
                {
                    return Result.Failure("You have already given rated for this post");
                }
                var newRating = new PostRating
                {
                    Status = Core.Enums.Status.Active,
                    CreatedDate = DateTime.Now,
                    PostId = request.Id,
                    Rating = request.Rate,
                    UserId = request.UserId
                };
                await _context.PostRatings.AddAsync(newRating);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("User rating on post was completed successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Post rating was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
