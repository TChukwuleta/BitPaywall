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
    public class GetPostByIdQuery : IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly ILightningService _lightningService;
        private readonly IAppDbContext _context;

        public GetPostByIdQueryHandler(IAuthService authService, IAppDbContext context, ILightningService lightningService)
        {
            _authService = authService;
            _context = context;
            _lightningService = lightningService;
        }

        public async Task<Result> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var post = new Post();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve post. Invalid user details specified");
                }
                post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (post == null)
                {
                    return Result.Failure("Post retrieval was not successful. Invalid post specified");
                }
                if (post.UserId != request.UserId)
                {
                    var engagedPost = await _context.EngagedPosts.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.PostId == post.Id);
                    if (engagedPost != null)
                    {
                        return Result.Success("Post retrieval was successful", post);
                    }
                    else
                    {
                        // Generate invoice using the post amount
                        var invoice = await _lightningService.CreateInvoice((long)post.Amount, $"{post.Id}/{request.UserId}", Core.Enums.UserType.Admin);
                        if (string.IsNullOrEmpty(invoice))
                        {
                            return Result.Failure("An error occured while generating invoice");
                        }
                        return Result.Success("Invoice generated successfully", invoice);
                    }
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
