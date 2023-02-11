﻿using BitPaywall.Application.Common.Interfaces;
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
    public class GetAllUsersPostQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetAllUsersPostQueryHandler : IRequestHandler<GetAllUsersPostQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;

        public GetAllUsersPostQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetAllUsersPostQuery request, CancellationToken cancellationToken)
        {
            var response = new List<Post>();
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Retrieving all user's post was not successful. Invalid user details");
                }
                var posts = await _context.Posts.Where(c => c.UserId == request.UserId).ToListAsync();
                if (posts.Count() <= 0)
                {
                    return Result.Failure("Retrieving user's post was not successful. No post found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    response = posts;
                }
                else
                {
                    response = posts.Skip(request.Skip).Take(request.Take).ToList();
                }
                var entity = new
                {
                    Entity = response,
                    Count = posts.Count()
                };
                return Result.Success("Retrieving user's post was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User's posts retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
