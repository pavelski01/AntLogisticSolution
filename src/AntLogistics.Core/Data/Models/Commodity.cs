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
    /// Gets or sets the commodity name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SKU (Stock Keeping Unit) code.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the commodity description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category of the commodity.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure (e.g., pcs, kg, m).
    /// </summary>
    public string UnitOfMeasure { get; set; } = "pcs";

    /// <summary>
    /// Gets or sets the weight per unit in kilograms.
    /// </summary>
    public decimal? WeightPerUnit { get; set; }

    /// <summary>
    /// Gets or sets the volume per unit in cubic meters.
    /// </summary>
    public decimal? VolumePerUnit { get; set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the warehouse ID where this commodity is primarily stored.
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the warehouse navigation property.
    /// </summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current stock quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the minimum stock level (reorder point).
    /// </summary>
    public decimal? MinimumStockLevel { get; set; }

    /// <summary>
    /// Gets or sets whether the commodity is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the commodity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the commodity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
