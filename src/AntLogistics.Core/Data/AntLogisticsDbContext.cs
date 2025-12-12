using AntLogistics.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
    /// Gets or sets the Stocks DbSet.
    /// </summary>
    public DbSet<Stock> Stocks { get; set; } = null!;

    /// <summary>
    /// Configures the entity model and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var roleConverter = new ValueConverter<OperatorRole, string>(
            role => role == OperatorRole.Admin ? "admin" : "operator",
            value => value == "admin" ? OperatorRole.Admin : OperatorRole.Operator);

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("warehouses");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Code)
                .HasColumnName("code")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.AddressLine)
                .HasColumnName("address_line")
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.City)
                .HasColumnName("city")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CountryCode)
                .HasColumnName("country_code")
                .IsRequired()
                .HasMaxLength(2);

            entity.Property(e => e.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(20);

            entity.Property(e => e.DefaultZone)
                .HasColumnName("default_zone")
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("DEFAULT");

            entity.Property(e => e.Capacity)
                .HasColumnName("capacity")
                .HasPrecision(18, 2);

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(e => e.DeactivatedAt)
                .HasColumnName("deactivated_at");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("now()");

            entity.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("ux_warehouses_code");

            entity.HasIndex(e => e.Code)
                .HasDatabaseName("idx_warehouses_active")
                .HasFilter("is_active = true");

            entity.HasIndex(e => new { e.IsActive, e.CountryCode })
                .HasDatabaseName("idx_warehouses_active_country");

            entity.HasCheckConstraint("ck_warehouses_code_lower", "code = lower(code)");
            entity.HasCheckConstraint("ck_warehouses_country_code", "country_code ~ '^[A-Z]{2}$'");
            entity.HasCheckConstraint("ck_warehouses_capacity_positive", "capacity > 0");
        });

        modelBuilder.Entity<Commodity>(entity =>
        {
            entity.ToTable("commodities");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Sku)
                .HasColumnName("sku")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.UnitOfMeasure)
                .HasColumnName("unit_of_measure")
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.ControlParameters)
                .HasColumnName("control_parameters")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(e => e.DeactivatedAt)
                .HasColumnName("deactivated_at");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("now()");

            entity.HasIndex(e => e.Sku)
                .IsUnique()
                .HasDatabaseName("ux_commodities_sku");

            entity.HasIndex(e => e.Sku)
                .HasDatabaseName("idx_commodities_active")
                .HasFilter("is_active = true");

            entity.HasIndex(e => e.ControlParameters)
                .HasDatabaseName("idx_commodities_control_parameters")
                .HasMethod("gin");

            entity.HasCheckConstraint("ck_commodities_sku_lower", "sku = lower(sku)");
        });

        modelBuilder.Entity<Operator>(entity =>
        {
            entity.ToTable("operators");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Username)
                .HasColumnName("username")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FullName)
                .HasColumnName("full_name")
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion(roleConverter)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValueSql("'operator'");

            entity.Property(e => e.IdleTimeoutMinutes)
                .HasColumnName("idle_timeout_minutes")
                .IsRequired()
                .HasDefaultValue(30);

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(e => e.LastLoginAt)
                .HasColumnName("last_login_at");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("now()");

            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("ux_operators_username");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("idx_operators_active")
                .HasFilter("is_active = true");

            entity.HasCheckConstraint("ck_operators_username_lower", "username = lower(username)");
            entity.HasCheckConstraint("ck_operators_role", "role in ('operator','admin')");
            entity.HasCheckConstraint("ck_operators_idle_timeout", "idle_timeout_minutes BETWEEN 5 AND 180");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("stocks");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityByDefaultColumn();

            entity.Property(e => e.WarehouseId)
                .HasColumnName("warehouse_id");

            entity.Property(e => e.CommodityId)
                .HasColumnName("commodity_id");

            entity.Property(e => e.Sku)
                .HasColumnName("sku")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.UnitOfMeasure)
                .HasColumnName("unit_of_measure")
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity")
                .HasPrecision(18, 3);

            entity.Property(e => e.WarehouseZone)
                .HasColumnName("warehouse_zone")
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("DEFAULT");

            entity.Property(e => e.OperatorId)
                .HasColumnName("operator_id");

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Source)
                .HasColumnName("source")
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("manual");

            entity.Property(e => e.OccurredAt)
                .HasColumnName("occurred_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.Stocks)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Commodity)
                .WithMany(c => c.Stocks)
                .HasForeignKey(e => e.CommodityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Operator)
                .WithMany(o => o.Stocks)
                .HasForeignKey(e => e.OperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.WarehouseId, e.OccurredAt })
                .HasDatabaseName("idx_stocks_wh_time");

            entity.HasIndex(e => new { e.CommodityId, e.OccurredAt })
                .HasDatabaseName("idx_stocks_commodity_time");

            entity.HasIndex(e => e.Sku)
                .HasDatabaseName("idx_stocks_sku")
                .HasMethod("hash");

            entity.HasIndex(e => new { e.WarehouseId, e.CommodityId })
                .HasDatabaseName("idx_stocks_active_wh")
                .HasFilter("quantity > 0");

            entity.HasIndex(e => e.Metadata)
                .HasDatabaseName("idx_stocks_metadata")
                .HasMethod("gin");

            entity.HasCheckConstraint("ck_stocks_quantity_positive", "quantity > 0");
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
        UpdateAuditedEntities(
            ChangeTracker.Entries<Warehouse>(),
            (entity, timestamp) =>
            {
                entity.CreatedAt = timestamp;
                entity.UpdatedAt = timestamp;
            },
            (entity, timestamp) => entity.UpdatedAt = timestamp);

        UpdateAuditedEntities(
            ChangeTracker.Entries<Commodity>(),
            (entity, timestamp) =>
            {
                entity.CreatedAt = timestamp;
                entity.UpdatedAt = timestamp;
            },
            (entity, timestamp) => entity.UpdatedAt = timestamp);

        UpdateAuditedEntities(
            ChangeTracker.Entries<Operator>(),
            (entity, timestamp) =>
            {
                entity.CreatedAt = timestamp;
                entity.UpdatedAt = timestamp;
            },
            (entity, timestamp) => entity.UpdatedAt = timestamp);

        var stockEntries = ChangeTracker.Entries<Stock>();
        foreach (var entry in stockEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                if (entry.Entity.OccurredAt == default)
                {
                    entry.Entity.OccurredAt = entry.Entity.CreatedAt;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static void UpdateAuditedEntities<T>(
        IEnumerable<EntityEntry<T>> entries,
        Action<T, DateTime> setCreated,
        Action<T, DateTime> setUpdated)
        where T : class
    {
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                var timestamp = DateTime.UtcNow;
                setCreated(entry.Entity, timestamp);
            }
            else if (entry.State == EntityState.Modified)
            {
                setUpdated(entry.Entity, DateTime.UtcNow);
            }
        }
    }
}
