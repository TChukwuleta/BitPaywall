using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.EngagedPosts.Queries
{
    public class GetActiveEngagedPostQuery : AuthToken, IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }
    public class GetActiveEngagedPostQueryHandler : IRequestHandler<GetActiveEngagedPostQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public GetActiveEngagedPostQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetActiveEngagedPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var posts = new List<EngagedPost>();
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve engaged posts. Invalid user details specified");
                }
                var engagedPosts = await _context.EngagedPosts.Include(c => c.Post).Where(c => c.UserId == request.UserId && c.Status == Status.Active).ToListAsync();
                if (engagedPosts.Count() <= 0)
                {
                    return Result.Failure("No active engaged posts found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    posts = engagedPosts;
                }
                else
                {
                    posts = engagedPosts.Skip(request.Skip).Take(request.Take).ToList();
                }
                var entity = new
                {
                    Entity = posts,
                    Count = engagedPosts.Count()
                };
                return Result.Success("Active engaged posts retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Engaged posts retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
