﻿using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
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
    public class UpdatePostCommand : IRequest<Result>, IIdValidator, IPostRequestValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
    }

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        public UpdatePostCommandHandler(IAuthService authService, IAppDbContext context, IConfiguration config)
        {
            _authService = authService;
            _context = context;
            _config = config;
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
                var post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.Id);
                if (post == null)
                {
                    return Result.Failure("Post update was not successful. Invalid post details");
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
                post.Image = request.Image; // cloudinary image
                post.Story = request.Story;
                post.Description = request.Description;
                post.Amount = request.Amount;
                post.LastModifiedDate = DateTime.Now;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Post update was successful", post);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Post update was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
