using Microsoft.EntityFrameworkCore;
using OrderService.Models.v1.Common;
using OrderService.Models.v1.DomainModels;

namespace OrderService.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderProcess> OrderProcesses { get; set; }
        public virtual DbSet<OrderProcessJob> OrderProcessJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasSequence<int>("OrderNumers")
                .StartsAt(1)
                .IncrementsBy(1)
                .HasMin(1)
                .HasMax(99999999)
                .IsCyclic();

            modelBuilder.Entity<Product>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Product>()
                .Property(o => o.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Product>()
                .Property(o => o.Description)
                .HasMaxLength(255);

            modelBuilder.Entity<Product>()
                .Property(o => o.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.UserId, o.Id });

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.UserId, o.Name });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .Property(o => o.Name)
                .HasMaxLength(255)
                .HasDefaultValueSql("CONCAT('Order-', FORMAT(NEXT VALUE FOR OrderNumers, '00000000'))");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasMaxLength(32)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderProcesses)
                .WithOne(op => op.Order)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.Quantity)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderProcess>()
                .HasKey(op => op.Id);

            modelBuilder.Entity<OrderProcess>()
                .HasIndex(op => op.OrderId);

            modelBuilder.Entity<OrderProcess>()
                .Property(op => op.Status)
                .HasMaxLength(32)
                .HasConversion<string>();

            modelBuilder.Entity<OrderProcess>()
                .Property(op => op.Note)
                .HasMaxLength(255);

            modelBuilder.Entity<OrderProcess>()
                .HasMany(o => o.ProcessJobs)
                .WithOne(od => od.OrderProcess)
                .HasForeignKey(od => od.ProcessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderProcess>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProcesses)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderProcessJob>()
                .HasKey(j => j.Id);

            modelBuilder.Entity<OrderProcessJob>()
                .HasIndex(j => j.ProcessId);

            modelBuilder.Entity<OrderProcessJob>()
                .HasIndex(j => new { j.ProcessId, j.ProcessJobType });

            modelBuilder.Entity<OrderProcessJob>()
                .Property(op => op.ProcessJobType)
                .HasMaxLength(32)
                .HasConversion<string>();

            modelBuilder.Entity<OrderProcessJob>()
                .Property(op => op.ProcessJobStatus)
                .HasMaxLength(32)
                .HasConversion<string>();

            modelBuilder.Entity<OrderProcessJob>()
                .HasOne(od => od.OrderProcess)
                .WithMany(p => p.ProcessJobs)
                .HasForeignKey(od => od.ProcessId)
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
