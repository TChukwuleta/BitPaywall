using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetAllPostsQuery : IRequest<Result>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, Result>
    {
        private readonly IAppDbContext _context;

        public GetAllPostsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var posts = await _context.Posts.Select(item => new Post
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
                }).ToListAsync();
                if (posts == null || !posts.Any())
                {
                    return Result.Failure("No post available");
                }
                var result = Extensions.SkipAndTake<Post>(posts, request.Skip, request.Take);
                return Result.Success("All posts retrieval was successful", result);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval was not successful {ex?.Message ?? ex?.InnerException.Message }");
            }
        }
    }
}
 