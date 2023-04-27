using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Application.Common.Model.Response;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.PostRatings.Queries
{
    public class GetAllPostsRatingQuery : AuthToken, IRequest<Result>, IBaseValidator
    {
        public string UserId { get; set; }
    }

    public class GetAllPostsRatingQueryHandler : IRequestHandler<GetAllPostsRatingQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetAllPostsRatingQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetAllPostsRatingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post rating. Invalid user details specified.");
                }
                var postRating = await _context.PostRatings.ToListAsync();
                if (postRating.Count() <= 0)
                {
                    return Result.Failure("No post rating found for this post");
                }
                var response = new List<PostRatingResponse>();
                var groupedRating = postRating.GroupBy(c => c.PostId).ToList();
                foreach (var item in groupedRating)
                {
                    decimal totalRatingCount = item.Sum(c => c.Rating);
                    decimal totalAchievableRates = item.Count() * 5;
                    decimal postAverageRating = (totalRatingCount / totalAchievableRates);
                    response.Add(new PostRatingResponse
                    {
                        PostId = item.Key,
                        PostAverageRating = postAverageRating,
                    });
                }
                return Result.Success("Posts rating retrieved successfully", response);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts rating retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message }");
            }
        }
    }
}
