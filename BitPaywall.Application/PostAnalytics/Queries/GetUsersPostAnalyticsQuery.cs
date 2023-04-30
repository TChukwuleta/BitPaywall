using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.PostAnalytics.Queries
{
    public class GetUsersPostAnalyticsQuery : AuthToken, IRequest<Result>
    {
        public string UserId { get; set; }
    }

    public class GetUsersPostAnalyticsQueryHandler : IRequestHandler<GetUsersPostAnalyticsQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetUsersPostAnalyticsQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _authService = authService;
            _context = context;
        }
        public async Task<Result> Handle(GetUsersPostAnalyticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post rating. Invalid user details specified.");
                }
                var userPostsRating = await _context.PostAnalytics.ToListAsync();
                if (userPostsRating == null || userPostsRating.Count() <= 0)
                {
                    return Result.Failure("No post rating found for this user");
                }
                return Result.Success("Post rating retrieved successfully", userPostsRating);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post rating retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
