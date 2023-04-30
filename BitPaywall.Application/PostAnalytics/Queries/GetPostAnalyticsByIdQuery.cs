using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.PostAnalytics.Queries
{
    public class GetPostAnalyticsByIdQuery : AuthToken, IRequest<Result>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class GetPostAnalyticsByIdQueryHandler : IRequestHandler<GetPostAnalyticsByIdQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetPostAnalyticsByIdQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _authService = authService;
            _context = context;
        }
        public async Task<Result> Handle(GetPostAnalyticsByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post rating. Invalid user details specified.");
                }
                var userPostsRating = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (userPostsRating == null)
                {
                    return Result.Failure("No post rating found");
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
