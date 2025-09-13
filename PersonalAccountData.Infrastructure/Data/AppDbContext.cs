using PersonalAccountData.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Resident> Residents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AccountNumber).IsRequired().HasMaxLength(10);
                entity.HasIndex(a => a.AccountNumber).IsUnique();
            });

            modelBuilder.Entity<Resident>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasOne(r => r.Account)
                    .WithMany(a => a.Residents)
                    .HasForeignKey(r => r.AccountId);
            });
        }
    }
}