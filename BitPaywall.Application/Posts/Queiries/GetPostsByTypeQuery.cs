using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetPostsByTypeQuery : IRequest<Result>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public PostCategory PostCategory { get; set; }
    }

    public class GetPostsByTypeQueryHandler : IRequestHandler<GetPostsByTypeQuery, Result>
    {
        private readonly IAppDbContext _context;
        public GetPostsByTypeQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<Result> Handle(GetPostsByTypeQuery request, CancellationToken cancellationToken)
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
                }).Where(c => c.PostCategory == request.PostCategory).ToListAsync();
                if (posts == null || !posts.Any())
                {
                    return Result.Failure("No record found");
                }
                var result = Extensions.SkipAndTake<Post>(posts, request.Skip, request.Take);
                return Result.Success("All posts retrieval was successful", result);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts retrieval was not successful {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
