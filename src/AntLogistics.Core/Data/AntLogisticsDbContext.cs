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
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Automatically sets CreatedAt and UpdatedAt timestamps.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Warehouse>();

        foreach (var entry in entries)
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
