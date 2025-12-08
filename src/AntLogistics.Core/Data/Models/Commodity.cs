namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Represents a commodity (item/product) stored in the logistics system.
/// </summary>
public class Commodity
{
    /// <summary>
    /// Gets or sets the unique identifier for the commodity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the commodity name (max 200 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SKU (Stock Keeping Unit) code - globally unique (max 100 characters, lowercase).
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the commodity description (max 1000 characters, optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category of the commodity (max 100 characters, optional).
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the base unit of measure (e.g., kg, pcs, m) (max 20 characters).
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the weight per unit in kilograms (optional).
    /// </summary>
    public decimal? WeightPerUnit { get; set; }

    /// <summary>
    /// Gets or sets the volume per unit in cubic meters (optional).
    /// </summary>
    public decimal? VolumePerUnit { get; set; }

    /// <summary>
    /// Gets or sets the unit price in base currency.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the warehouse ID where this commodity is primarily stored (foreign key).
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the warehouse where this commodity is stored (navigation property).
    /// </summary>
    public virtual Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current stock quantity in units.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the minimum stock level threshold (reorder point, optional).
    /// </summary>
    public decimal? MinimumStockLevel { get; set; }

    /// <summary>
    /// Gets or sets whether batch/lot tracking is required for this commodity.
    /// </summary>
    public bool BatchRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets control parameters as JSON (e.g., temperature range, humidity constraints).
    /// </summary>
    public string ControlParameters { get; set; } = "{}";

    /// <summary>
    /// Gets or sets whether the commodity is currently active (soft-delete flag).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the timestamp when the commodity was deactivated (soft-delete).
    /// </summary>
    public DateTime? DeactivatedAt { get; set; }

    /// <summary>
    /// Gets or sets the audit timestamp for creation.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the audit timestamp for last modification.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of inventory readings for this commodity (1:N relationship).
    /// </summary>
    public ICollection<Reading> Readings { get; set; } = new List<Reading>();
}
