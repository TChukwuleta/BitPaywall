using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetLatestPostQuery : AuthToken, IRequest<Result>
    {
        public int Take { get; set; }
    }

    public class GetLatestPostQueryHandler : IRequestHandler<GetLatestPostQuery, Result>
    {
        private readonly IAppDbContext _context;
        public GetLatestPostQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<Result> Handle(GetLatestPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var posts = await _context.Posts.OrderByDescending(c => c.CreatedDate).Take(request.Take).ToListAsync();
                if (posts == null || !posts.Any())
                {
                    return Result.Failure("No posts found");
                }
                return Result.Success("Posts retrieval was successful", posts);

            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
