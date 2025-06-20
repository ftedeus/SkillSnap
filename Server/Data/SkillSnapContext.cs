using Microsoft.EntityFrameworkCore;
using SkillSnap.Shared.Models;

namespace SkillSnap.Server.Data
{
    public class SkillSnapContext : DbContext
    {
        public SkillSnapContext(DbContextOptions<SkillSnapContext> options)
            : base(options)
        {
        }

        public DbSet<PortfolioUser> PortfolioUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Add model configuration here
            modelBuilder.Entity<PortfolioUser>()
                .HasMany(p => p.Projects)
                .WithOne(p => p.PortfolioUser)
                .HasForeignKey(p => p.PortfolioUserId);

            modelBuilder.Entity<PortfolioUser>()
                .HasMany(p => p.Skills)
                .WithOne(s => s.PortfolioUser)
                .HasForeignKey(s => s.PortfolioUserId);

            modelBuilder.Entity<PortfolioUser>()
                    .HasIndex(u => u.Name)
                    .IsUnique();
                
            modelBuilder.Entity<Project>()
            .HasIndex(p => new { p.PortfolioUserId, p.Title })
            .IsUnique();

        }
    }
}