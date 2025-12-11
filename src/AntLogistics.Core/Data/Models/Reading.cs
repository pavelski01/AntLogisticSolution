namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Represents a warehouse reading/inventory measurement.
/// Append-only table for audit trail.
/// </summary>
public class Reading
{
    private string _sku = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the reading (auto-generated).
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the warehouse identifier (foreign key).
    /// </summary>
    public Guid WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the commodity identifier (foreign key).
    /// </summary>
    public Guid CommodityId { get; set; }

    /// <summary>
    /// Gets or sets the SKU code (denormalized copy for audit trail).
    /// </summary>
    public string Sku
    {
        get => _sku;
        set => _sku = (value ?? throw new ArgumentNullException(nameof(value))).ToLowerInvariant();
    }

    /// <summary>
    /// Gets or sets the unit of measure at the time of reading.
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the measured quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the batch/lot number (optional).
    /// </summary>
    public string? BatchNumber { get; set; }

    /// <summary>
    /// Gets or sets the warehouse zone where the reading was taken.
    /// </summary>
    public string WarehouseZone { get; set; } = "DEFAULT";

    /// <summary>
    /// Gets or sets the operator identifier who created the reading (nullable for audit).
    /// </summary>
    public Guid? OperatorId { get; set; }

    /// <summary>
    /// Gets or sets the immutable operator label/name at creation time.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source of the reading (manual, API, import, etc.).
    /// </summary>
    public string Source { get; set; } = "manual";

    /// <summary>
    /// Gets or sets the timestamp when the reading occurred physically.
    /// </summary>
    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the reading was persisted to the database.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets free-form metadata as JSON.
    /// </summary>
    public string Metadata { get; set; } = "{}";

    /// <summary>
    /// Gets or sets the associated warehouse (navigation property).
    /// </summary>
    public virtual Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated commodity (navigation property).
    /// </summary>
    public virtual Commodity Commodity { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated operator (navigation property, nullable).
    /// </summary>
    public virtual Operator? Operator { get; set; }
}
