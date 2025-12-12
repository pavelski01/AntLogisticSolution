namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Represents a commodity (item/product) stored in the logistics system.
/// </summary>
public class Commodity
{
    private string _sku = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the commodity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the SKU (Stock Keeping Unit) code - globally unique (max 100 characters, lowercase).
    /// </summary>
    public string Sku
    {
        get => _sku;
        set => _sku = (value ?? throw new ArgumentNullException(nameof(value))).ToLowerInvariant();
    }

    /// <summary>
    /// Gets or sets the commodity name (max 200 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base unit of measure (e.g., kg, pcs, m) (max 20 characters).
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

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
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of stock records for this commodity (1:N relationship).
    /// </summary>
    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
