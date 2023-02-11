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
    public class GetAllPostsQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;

        public GetAllPostsQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            var posts = new List<Post>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Posts retrieval was not successful. Invalid user details");
                }
                var allPosts = await _context.Posts.Select(item => new Post
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Status = item.Status,
                    Amount = item.Amount,
                    Views = item.Views,
                    CreatedDate = item.CreatedDate
                }).ToListAsync();
                if (allPosts.Count() <= 0)
                {
                    return Result.Failure("No post available");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    posts = allPosts;
                }
                else
                {
                    posts = allPosts.Skip(request.Skip).Take(request.Take).ToList();
                }

                return Result.Success("All posts retrieval was successful", posts);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Posts retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
