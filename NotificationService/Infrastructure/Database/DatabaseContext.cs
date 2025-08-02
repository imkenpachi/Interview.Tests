using Microsoft.EntityFrameworkCore;
using NotificationService.Models.v1.Common;
using NotificationService.Models.v1.DomainModels;

namespace NotificationService.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        
        public virtual DbSet<EmailLog> EmailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailLog>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<EmailLog>()
                .HasIndex(o => o.UserId);

            modelBuilder.Entity<EmailLog>()
                .Property(op => op.Subject)
                .IsUnicode()
                .HasMaxLength(512);

            modelBuilder.Entity<EmailLog>()
                .Property(op => op.From)
                .IsUnicode()
                .HasMaxLength(128);

            modelBuilder.Entity<EmailLog>()
                .Property(op => op.To)
                .IsUnicode()
                .HasMaxLength(512);

            modelBuilder.Entity<EmailLog>()
                .Property(op => op.Bcc)
                .IsUnicode()
                .HasMaxLength(512);

            modelBuilder.Entity<EmailLog>()
                .Property(op => op.Body)
                .IsUnicode()
                .HasColumnType("nvarchar(max)");
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
