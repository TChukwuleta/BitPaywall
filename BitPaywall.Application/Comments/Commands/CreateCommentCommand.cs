using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BitPaywall.Application.Comments.Commands
{
    public class CreateCommentCommand : AuthToken, IRequest<Result>
    {
        public int PostId { get; set; }
        public int ParentCommentId { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public CreateCommentCommandHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }
        public async Task<Result> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Invalid user");
                }
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.PostId);
                if (post == null || post?.Id <= 0)
                {
                    return Result.Failure("Invalid post specified");
                }
                var comment = new Comment
                {
                    CreatedDate = DateTime.Now,
                    Text = request.Comment,
                    UserId = request.UserId,
                    FirstName = user.user.FirstName,
                    LastName = user.user.LastName,
                    ParentCommentId = 0
                };
                if (request.ParentCommentId > 0)
                {
                    var parentComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == request.ParentCommentId);
                    comment.ParentCommentId = parentComment != null ? parentComment.Id : 0;
                }
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Comment creation was successful", comment);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Comments creation was not successful {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
