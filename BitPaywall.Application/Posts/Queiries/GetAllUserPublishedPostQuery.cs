using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetAllUserPublishedPostQuery : AuthToken, IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetAllUserPublishedPostQueryHandler : IRequestHandler<GetAllUserPublishedPostQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetAllUserPublishedPostQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetAllUserPublishedPostQuery request, CancellationToken cancellationToken)
        {
            object entity = default;
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Published posts retrieval was not successful. Invalid user details specified");
                }
                var publishedPosts = await _context.Posts.Where(c => c.UserId == request.UserId && c.PostType == Core.Enums.PostStatusType.Draft).ToListAsync();
                if (publishedPosts.Count() <= 0)
                {
                    return Result.Failure("Published posts retrieval was not successful. No published posts found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    entity = new
                    {
                        Posts = publishedPosts,
                        Count = publishedPosts.Count()
                    };
                }
                else
                {
                    entity = new
                    {
                        Posts = publishedPosts.Skip(request.Skip).Take(request.Take).ToList(),
                        Count = publishedPosts.Count()
                    };
                }

                return Result.Success("Published posts retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Published posts retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
