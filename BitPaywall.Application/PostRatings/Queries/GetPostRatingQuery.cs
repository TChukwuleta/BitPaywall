using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Application.Common.Model.Response;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.PostRatings.Queries
{
    public class GetPostRatingQuery : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class GetPostRatingQueryHandler : IRequestHandler<GetPostRatingQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetPostRatingQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetPostRatingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to publish post. Invalid user details specified.");
                }
                var postRating = await _context.PostRatings.Where(c => c.PostId == request.Id).ToListAsync();
                if (postRating.Count() <= 0)
                {
                    return Result.Failure("No post rating found for this post");
                }
                var response = new PostRatingResponse();
                response.PostId = request.Id;
                var totalRatingCount = postRating.Sum(c => c.Rating);
                var totalAchievableRates = postRating.Count() * 5;
                var postAverageRating = (totalRatingCount / totalAchievableRates);
                response.PostAverageRating = postAverageRating;
                return Result.Success("Post rating retrieved successfully", response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Post rating retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
