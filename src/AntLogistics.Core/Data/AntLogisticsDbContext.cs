using AntLogistics.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AntLogistics.Core.Data;

/// <summary>
/// Database context for the Ant Logistics application.
/// </summary>
public class AntLogisticsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AntLogisticsDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public AntLogisticsDbContext(DbContextOptions<AntLogisticsDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Warehouses DbSet.
    /// </summary>
    public DbSet<Warehouse> Warehouses { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Commodities DbSet.
    /// </summary>
    public DbSet<Commodity> Commodities { get; set; } = null!;

    /// <summary>
    /// Configures the entity model and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Warehouse entity
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(e => e.Code)
                .IsUnique();
            
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Country)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20);
            
            entity.Property(e => e.Capacity)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        // Configure Commodity entity
        modelBuilder.Entity<Commodity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Sku)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.HasIndex(e => e.Sku)
                .IsUnique();
            
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            
            entity.Property(e => e.Category)
                .HasMaxLength(100);
            
            entity.Property(e => e.UnitOfMeasure)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(e => e.WeightPerUnit)
                .HasPrecision(18, 3);
            
            entity.Property(e => e.VolumePerUnit)
                .HasPrecision(18, 3);
            
            entity.Property(e => e.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);
            
            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasPrecision(18, 3);
            
            entity.Property(e => e.MinimumStockLevel)
                .HasPrecision(18, 3);
            
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            // Configure relationship with Warehouse
            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.Commodities)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Automatically sets CreatedAt and UpdatedAt timestamps.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var warehouseEntries = ChangeTracker.Entries<Warehouse>();
        var commodityEntries = ChangeTracker.Entries<Commodity>();

        foreach (var entry in warehouseEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        foreach (var entry in commodityEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
