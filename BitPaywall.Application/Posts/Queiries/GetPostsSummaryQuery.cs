using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetPostsSummaryQuery : IRequest<Result>
    {
    }

    public class GetPostsSummaryQueryHandler : IRequestHandler<GetPostsSummaryQuery, Result>
    {
        private readonly IAppDbContext _context;
        public GetPostsSummaryQueryHandler(IAppDbContext context)
        {
            _context = context;   
        }
        public async Task<Result> Handle(GetPostsSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var enumCounts = await _context.Posts.GroupBy(c => c.PostCategory)
                    .Select(g => new {
                        CategoryName = g.Key.ToString(),
                        Count = g.Count()
                    }).ToListAsync();

                return Result.Success("Posts summary retrieval was successful", enumCounts);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Posts summary retrieval was not successful {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
