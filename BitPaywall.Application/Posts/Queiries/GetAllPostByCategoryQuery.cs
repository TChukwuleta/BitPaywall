using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
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
    public class GetAllPostByCategoryQuery : IRequest<Result>, IPostCategoryValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public PostCategory PostCategory { get; set; }
        public string UserId { get; set; }
    }

    public class GetAllPostByCategoryQueryHandler : IRequestHandler<GetAllPostByCategoryQuery, Result>
    {
        private readonly IAuthService _authService; 
        private readonly IAppDbContext _context;
        public GetAllPostByCategoryQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<Result> Handle(GetAllPostByCategoryQuery request, CancellationToken cancellationToken)
        {
            var posts = new List<Post>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post. Invalid user details specified");
                }
                var allPosts = await _context.Posts.Select(item => new Post
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    PostCategory = item.PostCategory,
                    Status = item.Status,
                    Amount = item.Amount,
                    Views = item.Views,
                    CreatedDate = item.CreatedDate
                }).Where(c => c.PostCategory == request.PostCategory).ToListAsync();
                if (allPosts.Count() <= 0)
                {
                    return Result.Failure($"No post available for {request.PostCategory.ToString()}");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    posts = allPosts;
                }
                else
                {
                    posts = allPosts.Skip(request.Skip).Take(request.Take).ToList();
                }

                return Result.Success($"All {request.PostCategory.ToString()} posts retrieval was successful", posts);
            }
            catch (Exception ex)
            {
                return Result.Failure("Posts retrieval was not successful", ex?.Message ?? ex?.InnerException.Message);
            }
        }
    }
}
