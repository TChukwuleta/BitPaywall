using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetPopularPostsQuery : IRequest<Result>
    {
        public int Take { get; set; }
    }

    public class GetPopularPostsQueryHandler : IRequestHandler<GetPopularPostsQuery, Result>
    {
        private readonly IAppDbContext _context;
        public GetPopularPostsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<Result> Handle(GetPopularPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<Post> posts = new List<Post>();
                var topPostAnalytics = await _context.PostAnalytics.OrderByDescending(c => c.ReadCount).Take(request.Take).ToListAsync();
                if (topPostAnalytics == null || !topPostAnalytics.Any())
                {
                    return Result.Failure("No analytics found");
                }
                foreach (var analytic in topPostAnalytics)
                {
                    var post = await _context.Posts.Select(item => new Post
                    {
                        Id = item.Id,
                        Image = item.Image,
                        Title = item.Title,
                        Description = item.Description,
                        PostCategory = item.PostCategory,
                        Status = item.Status,
                        Amount = item.Amount,
                        Views = item.Views,
                        CreatedDate = item.CreatedDate
                    }).FirstOrDefaultAsync(c => c.Id == analytic.Id);
                    if (post != null && post.Id > 0)
                    {
                        posts.Add(post);
                    }
                }
                return Result.Success("Top posts retrieval was successful", posts);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval was not successful {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
