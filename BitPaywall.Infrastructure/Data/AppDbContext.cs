using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<EngagedPost> EngagedPosts { get; set; }
        public DbSet<PostRating> PostRatings { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        // entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
