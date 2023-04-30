﻿using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Common.Interfaces.Validators;
using BitPaywall.Core.Entities;
using BitPaywall.Core.Enums;
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
    public class GetDeactivatedEngagedPostQuery : AuthToken, IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetDeactivatedEngagedPostQueryHandler : IRequestHandler<GetDeactivatedEngagedPostQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetDeactivatedEngagedPostQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetDeactivatedEngagedPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var posts = new List<EngagedPost>();
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to retrieve deactivated engaged posts. Invalid user details specified");
                }
                var engagedPosts = await _context.EngagedPosts.Include(c => c.Post).Where(c => c.UserId == request.UserId && c.Status == Status.Deactivated).ToListAsync();
                if (engagedPosts.Count() <= 0)
                {
                    return Result.Failure("No deactivated engaged posts found for this user");
                }
                if (request.Skip == 0 && request.Take == 0)
                {
                    posts = engagedPosts;
                }
                else
                {
                    posts = engagedPosts.Skip(request.Skip).Take(request.Take).ToList();
                }
                var entity = new
                {
                    Entity = posts,
                    Count = engagedPosts.Count()
                };
                return Result.Success("Deactivated engaged posts retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Deactivated engaged posts retrieval was not successful. {ex?.Message ?? ex?.InnerException.Message}");
            }
        }
    }
}
