using Microsoft.EntityFrameworkCore;
using AppPlusSQL.Domain.Entities;

namespace AppPlusSQL.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Team> Teams { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(m => m.TeamId);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Member)
                .WithMany(m => m.Activities)
                .HasForeignKey(a => a.MemberId);
        }
    }
}
