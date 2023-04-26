using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Posts.Commands
{
    public class CreatePostCommand : AuthToken, IRequest<Result>, IBaseValidator, IPostRequestValidator
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public PostCategory PostCategory { get; set; }
    }

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;

        public CreatePostCommandHandler(IAuthService authService, IAppDbContext context, IConfiguration config, ICloudinaryService cloudinaryService)
        {
            _authService = authService;
            _context = context;
            _config = config;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Post creation was not successful. Invalid user details specified");
                }
                var existingPost = await _context.Posts.FirstOrDefaultAsync(c => c.Title.ToLower() == request.Title.ToLower());
                if (existingPost != null)
                {
                    return Result.Failure("Post creation was not successful. Another post with this title already exist");
                }
                var minAmount = _config["Post:MinimumAmount"];
                var maxAmount = _config["Post:MaximumAmount"];
                if(request.Amount < int.Parse(minAmount))
                {
                    return Result.Failure("Amount specified less than the minimum specified amount by the system");
                }
                if (request.Amount > int.Parse(maxAmount))
                {
                    return Result.Failure("Amount specified is greater than the maximum specified amount by the system");
                }
                var post = new Post
                {
                    Title = request.Title,
                    Image = string.IsNullOrEmpty(request.Image) ? "" : await _cloudinaryService.UploadImage(request.Image, request.UserId),
                    Amount = request.Amount,
                    Views = default,
                    CreatedDate = DateTime.Now,
                    PostType = PostStatusType.Draft,
                    PostCategory = request.PostCategory,
                    Status = Status.Active,
                    UserId = request.UserId,
                    Description = request.Description,
                    Story = request.Story
                };

                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Post creation was successful", post);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Post creation was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
