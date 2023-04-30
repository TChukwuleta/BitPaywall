using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetPostByIdQuery : AuthToken, IRequest<Result>, IIdValidator
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
                switch (post.PostType)
                {
                    case Core.Enums.PostStatusType.Draft:
                        if (post.UserId != request.UserId)
                        {
                            return Result.Failure("Invalid post specified");
                        }
                        return Result.Success("Draft post retrieved successfully", post);
                    case Core.Enums.PostStatusType.Published:
                        if (post.UserId == request.UserId)
                        {
                            return Result.Success("Post retrieved successfully", post);
                        }
                        var postAnalytics = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.PostId == request.Id);
                        var engagedPost = await _context.EngagedPosts.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.PostId == post.Id);
                        if (engagedPost != null)
                        {
                            if (postAnalytics == null)
                            {
                                var newPostAnalytics = new PostAnalytic
                                {
                                    CreatedDate = DateTime.Now,
                                    Status = Core.Enums.Status.Active,
                                    PostId = request.Id,
                                    ReadCount = 1
                                };
                                await _context.PostAnalytics.AddAsync(newPostAnalytics);
                            }
                            else
                            {
                                postAnalytics.ReadCount += 1;
                                _context.PostAnalytics.Update(postAnalytics);
                            }
                            await _context.SaveChangesAsync(cancellationToken);
                            return Result.Success("Post retrieval was successful", post);
                        }
                        else
                        {
                            // Generate invoice using the post amount
                            var invoice = await _lightningService.CreateInvoice((long)post.Amount, $"{(int)PaymentType.Purchase}|{post.Id}/{request.UserId}");
                            if (string.IsNullOrEmpty(invoice))
                            {
                                return Result.Failure("An error occured while generating invoice");
                            }
                            return Result.Success("Invoice generated successfully", invoice);
                        }
                        break;
                    default:
                        break;
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
