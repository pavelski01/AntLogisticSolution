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
    /// Gets or sets the Operators DbSet.
    /// </summary>
    public DbSet<Operator> Operators { get; set; } = null!;

    /// <summary>
    /// Gets or sets the OperatorSessions DbSet.
    /// </summary>
    public DbSet<OperatorSession> OperatorSessions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Readings DbSet.
    /// </summary>
    public DbSet<Reading> Readings { get; set; } = null!;

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

            entity.Property(e => e.DefaultZone)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("DEFAULT");

            entity.Property(e => e.Capacity)
                .HasPrecision(18, 2);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // Partial index for active warehouses
            entity.HasIndex(e => new { e.IsActive, e.Country })
                .HasDatabaseName("idx_warehouses_active");
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

            entity.Property(e => e.BatchRequired)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.ControlParameters)
                .IsRequired()
                .HasDefaultValue("{}");

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

            // Partial index for active commodities
            entity.HasIndex(e => new { e.IsActive, e.Sku })
                .HasDatabaseName("idx_commodities_active");
        });

        // Configure Operator entity
        modelBuilder.Entity<Operator>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(OperatorRole.Operator);

            entity.Property(e => e.IdleTimeoutMinutes)
                .IsRequired()
                .HasDefaultValue(30);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        // Configure OperatorSession entity
        modelBuilder.Entity<OperatorSession>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SessionToken)
                .IsRequired();

            entity.HasIndex(e => e.SessionToken)
                .IsUnique();

            entity.Property(e => e.IssuedAt)
                .IsRequired();

            entity.Property(e => e.LastSeenAt)
                .IsRequired();

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            entity.Property(e => e.ClientIp)
                .HasMaxLength(45);

            entity.Property(e => e.UserAgent)
                .HasMaxLength(500);

            // Configure relationship with Operator
            entity.HasOne(e => e.Operator)
                .WithMany(o => o.Sessions)
                .HasForeignKey(e => e.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Reading entity
        modelBuilder.Entity<Reading>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Sku)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.UnitOfMeasure)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasPrecision(18, 3);

            entity.Property(e => e.BatchNumber)
                .HasMaxLength(100);

            entity.Property(e => e.WarehouseZone)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("DEFAULT");

            entity.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Source)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("manual");

            entity.Property(e => e.OccurredAt)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.Metadata)
                .IsRequired()
                .HasDefaultValue("{}");

            // Configure relationships
            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.Readings)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Commodity)
                .WithMany(c => c.Readings)
                .HasForeignKey(e => e.CommodityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Operator)
                .WithMany(o => o.Readings)
                .HasForeignKey(e => e.OperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for performance
            entity.HasIndex(e => new { e.WarehouseId, e.OccurredAt })
                .HasDatabaseName("idx_readings_wh_time");

            entity.HasIndex(e => new { e.CommodityId, e.OccurredAt })
                .HasDatabaseName("idx_readings_commodity_time");

            entity.HasIndex(e => e.Sku)
                .HasDatabaseName("idx_readings_sku");
        });

        // Seed initial warehouse data
        modelBuilder.Entity<Warehouse>().HasData(
            new Warehouse
            {
                Id = 1,
                Name = "Central Distribution Center",
                Code = "CDC-001",
                Address = "123 Industrial Boulevard",
                City = "Chicago",
                Country = "United States",
                PostalCode = "60601",
                Capacity = 50000.00m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new Warehouse
            {
                Id = 2,
                Name = "Eastern Regional Hub",
                Code = "ERH-002",
                Address = "456 Commerce Street",
                City = "New York",
                Country = "United States",
                PostalCode = "10001",
                Capacity = 35000.00m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 2, 20, 14, 30, 0, DateTimeKind.Utc)
            },
            new Warehouse
            {
                Id = 3,
                Name = "Western Logistics Center",
                Code = "WLC-003",
                Address = "789 Pacific Avenue",
                City = "Los Angeles",
                Country = "United States",
                PostalCode = "90001",
                Capacity = 42000.00m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 3, 10, 9, 15, 0, DateTimeKind.Utc)
            }
        );
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
