using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.PostAnalytics.Queries
{
    public class GetAllSystemPostAnalyticsQuery : AuthToken, IRequest<Result>
    {
        public string UserId { get; set; }
    }

    public class GetAllSystemPostAnalyticsQueryHandler : IRequestHandler<GetAllSystemPostAnalyticsQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetAllSystemPostAnalyticsQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _authService = authService;
            _context = context;
        }
        public async Task<Result> Handle(GetAllSystemPostAnalyticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post analytics. Invalid user details specified.");
                }

                var postsAnalytics = await _context.PostAnalytics.Select(item => new PostAnalytic
                {
                    Id = item.Id,
                    AmountGenerated = item.AmountGenerated,
                    ReadCount = item.ReadCount,
                    CreatedDate = item.CreatedDate
                }).ToListAsync();
                if (postsAnalytics == null || postsAnalytics.Count() <= 0)
                {
                    return Result.Failure("No post analytics found");
                }
                return Result.Success("Posts analytics retrieved successfully", postsAnalytics);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts analytics retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
