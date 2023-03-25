using BitPaywall.Application.Common.Interfaces;
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
    public class GetDraftPostByIdQuery : AuthToken, IRequest<Result>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class GetDraftPostByIdQueryHandler : IRequestHandler<GetDraftPostByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetDraftPostByIdQueryHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result> Handle(GetDraftPostByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post. Invalid user details specified");
                }

                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId && c.PostType == Core.Enums.PostStatusType.Draft);
                if (post == null)
                {
                    return Result.Failure("No record found");
                }
                return Result.Success(post);
            }
            catch (Exception ex)
            {
                return Result.Failure("Post retrieval was not successful", ex?.Message ?? ex?.InnerException.Message);
            }
        }
    }
}
