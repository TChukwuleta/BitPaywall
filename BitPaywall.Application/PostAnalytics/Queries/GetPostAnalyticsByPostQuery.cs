using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.PostAnalytics.Queries
{
    public class GetPostAnalyticsByPostQuery : AuthToken, IRequest<Result>
    {
        public int PostId { get; set; }
        public string UserId { get; set; }   
    }

    public class GetPostAnalyticsByPostQueryHandler : IRequestHandler<GetPostAnalyticsByPostQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetPostAnalyticsByPostQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<Result> Handle(GetPostAnalyticsByPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post rating. Invalid user details specified.");
                }
                var postRating = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.PostId == request.PostId);
                if (postRating == null)
                {
                    return Result.Failure("No post rating found for this post");
                }
                return Result.Success("Post rating retrieved successfully", postRating);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post rating retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message }");
            }
        }
    }
}
