using BitPaywall.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<SystemUser> SystemUsers { get; set; }
        DbSet<Post> Posts { get; set; }
        DbSet<EngagedPost> EngagedPosts { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<PostRating> PostRatings { get; set; }
        DbSet<PostAnalytic> PostAnalytics { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
