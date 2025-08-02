using Microsoft.EntityFrameworkCore;
using PaymentService.Models.v1.Common;
using PaymentService.Models.v1.DomainModels;

namespace PaymentService.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.ExternalId);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => new { p.UserId, p.OrderId });

            modelBuilder.Entity<Payment>()
                .Property(o => o.Status)
                .HasMaxLength(32)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(o => o.PaymentProvider)
                .HasMaxLength(32)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .HasMany(p => p.PaymentTransactions)
                .WithOne(pt => pt.Payment)
                .HasForeignKey(pt => pt.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentTransaction>()
                .HasKey(pt => new { pt.PaymentId, pt.TransactionId });

            modelBuilder.Entity<PaymentTransaction>()
                .Property(o => o.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Payment)
                .WithMany(p => p.PaymentTransactions)
                .HasForeignKey(od => od.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAtUtc = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAtUtc = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAtUtc = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAtUtc = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
