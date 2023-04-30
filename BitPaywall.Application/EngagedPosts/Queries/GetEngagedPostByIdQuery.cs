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

namespace BitPaywall.Application.EngagedPosts.Queries
{
    public class GetEngagedPostByIdQuery : AuthToken, IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }
    public class GetEngagedPostByIdQueryHandler : IRequestHandler<GetEngagedPostByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetEngagedPostByIdQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetEngagedPostByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve engaged post. Invalid user details specified");
                }
                var engagedPost = await _context.EngagedPosts.Include(c => c.Post).FirstOrDefaultAsync(c => c.UserId == request.UserId && c.Id == request.Id);
                if (engagedPost == null)
                {
                    return Result.Failure("Unable to retrieve engaged post. Invalid engaged post specified");
                }
                return Result.Success("Engaged post retrieval was successful", engagedPost);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Engaged post retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
