using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetAllPublishedPostsQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }
    public class GetAllPublishedPostsQueryHandler : IRequestHandler<GetAllPublishedPostsQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetAllPublishedPostsQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetAllPublishedPostsQuery request, CancellationToken cancellationToken)
        {
            var posts = new List<Post>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Published posts retrieval was not successful. Invalid user details specified");
                }
                var draftPosts = await _context.Posts.Where(c => c.UserId == request.UserId && c.PostType == Core.Enums.PostType.Draft).ToListAsync();
                if (draftPosts.Count() <= 0)
                {
                    return Result.Failure("Published posts retrieval was not successful. No draft posts found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    posts = draftPosts;
                }
                else
                {
                    posts = draftPosts.Skip(request.Skip).Take(request.Take).ToList();
                }
                var entity = new
                {
                    Entity = posts,
                    Count = draftPosts.Count()
                };
                return Result.Success("Pubkished posts retrieval was successful", posts);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Published posts retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
