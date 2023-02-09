using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
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
    public class GetPostByIdQuery : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;

        public GetPostByIdQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post. Invalid user details specified");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);
                if (post == null)
                {
                    return Result.Failure("Unable to retrieve post. Invalid post Id for this user");
                }
                return Result.Success("Post retrieval was successful", post);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Post retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
