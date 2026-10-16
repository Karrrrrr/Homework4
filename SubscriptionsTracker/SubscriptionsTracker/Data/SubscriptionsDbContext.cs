using Microsoft.EntityFrameworkCore;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Data
{
    public class SubscriptionsDbContext : DbContext
    {
        public SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscription>(b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.Name).IsRequired();
                b.Property(s => s.Amount).HasColumnType("decimal(18,2)");
                // enums will be stored as integers by default
            });
        }
    }
}
