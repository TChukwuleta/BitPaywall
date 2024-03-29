﻿using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Posts.Queiries
{
    public class GetUserPostByIdQuery : AuthToken, IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
    }

    public class GetUserPostByIdQueryHandler : IRequestHandler<GetUserPostByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        private readonly ILightningService _lightningService;
        public GetUserPostByIdQueryHandler(IAuthService authService, IAppDbContext context, ILightningService lightningService)
        {
            _authService = authService;
            _context = context;
            _lightningService = lightningService;

        }

        public async Task<Result> Handle(GetUserPostByIdQuery request, CancellationToken cancellationToken)
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
                if (post == null || post?.Id <= 0)
                {
                    return Result.Failure("Post retrieval was not successful. Invalid post specified");
                }
                var postAnalytics = await _context.PostAnalytics.FirstOrDefaultAsync(c => c.PostId == request.Id);
                if (post.UserId != request.UserId)
                {
                    if (post.PostType == Core.Enums.PostStatusType.Draft)
                    {
                        return Result.Failure("Post retrieval was not successful. Invalid post specified");
                    }
                    var engagedPost = await _context.EngagedPosts.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.PostId == post.Id);
                    if (engagedPost != null)
                    {
                        if (postAnalytics == null || postAnalytics?.Id <= 0)
                        {
                            var newPostAnalytics = new PostAnalytic
                            {
                                CreatedDate = DateTime.Now,
                                Status = Core.Enums.Status.Active,
                                PostId = request.Id,
                                ReadCount = 1,
                                UserId = post.UserId,
                            };
                            await _context.PostAnalytics.AddAsync(newPostAnalytics);
                        }
                        else
                        {
                            postAnalytics.ReadCount = postAnalytics.ReadCount + 1;
                            _context.PostAnalytics.Update(postAnalytics);
                        }
                    }
                    else
                    {
                        var invoice = await _lightningService.CreateInvoice((long)post.Amount, $"{post.Id}/{request.UserId}");
                        if (string.IsNullOrEmpty(invoice))
                        {
                            return Result.Failure("An error occured while generating invoice");
                        }
                        return Result.Success("Invoice generated successfully", invoice);
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Post retrieval was successful", post);
            }
            catch (Exception ex)
            {
                return Result.Failure("Post retrieval was not successful", ex?.Message ?? ex?.InnerException.Message);
            }
        }
    }
}
