using AuthServer.Core.Entities;
using AuthServer.Infrastructure.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthServer.Infrastructure.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(RefreshTokenConfiguration).Assembly);
        }
        
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
