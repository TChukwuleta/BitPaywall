using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetPostByIdQuery : IRequest<Result>
    {
        public int Id { get; set; }
    }
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result>
    {
        private readonly IAppDbContext _context;

        public GetPostByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            try
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
                }).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (post == null)
                {
                    return Result.Failure("Post retrieval was not successful. Invalid post specified");
                }
                return Result.Success("Post retrieval was successful", post);
            }
            catch (Exception ex)
            {
                return Result.Failure("Post retrieval was not successful", ex?.Message ?? ex?.InnerException.Message);
            }
        }
    }
}
