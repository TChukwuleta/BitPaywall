using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Enums;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BitPaywall.Application.Posts.Commands
{
    public class UpdatePostCommand : AuthToken, IRequest<Result>, IIdValidator, IPostRequestValidator
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
        public PostCategory PostCategory { get; set; }
    }

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ICloudinaryService _cloudinaryService;
        public UpdatePostCommandHandler(IAuthService authService, IAppDbContext context, IConfiguration config, ICloudinaryService cloudinaryService)
        {
            _authService = authService;
            _context = context;
            _config = config;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Post update was not successful. Invalid user details");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);
                if (post == null)
                {
                    return Result.Failure("Post update was not successful. Invalid post details");
                }
                if (post.PostType == PostStatusType.Published)
                {
                    return Result.Failure("Post update was not successful. Post already published");
                }
                var minAmount = _config["Post:MinimumAmount"];
                var maxAmount = _config["Post:MaximumAmount"];
                if (request.Amount < int.Parse(minAmount))
                {
                    return Result.Failure("Amount specified less than the minimum specified amount by the system");
                }
                if (request.Amount > int.Parse(maxAmount))
                {
                    return Result.Failure("Amount specified is greater than the maximum specified amount by the system");
                }
                post.Image = string.IsNullOrEmpty(request.Image) ? post.Image : await _cloudinaryService.UploadImage(request.Image, request.UserId);
                post.Story = request.Story;
                post.Description = request.Description;
                post.Amount = request.Amount;
                post.PostCategory = request.PostCategory;
                post.LastModifiedDate = DateTime.Now;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Post updated successfully", post);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Post update was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
