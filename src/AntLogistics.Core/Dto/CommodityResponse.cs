namespace AntLogistics.Core.Dto;

/// <summary>
/// Response model for commodity data.
/// </summary>
public class CommodityResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the commodity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the SKU (Stock Keeping Unit) code.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the commodity name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base unit of measure.
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets control parameters as JSON.
    /// </summary>
    public string ControlParameters { get; set; } = "{}";

    /// <summary>
    /// Gets or sets whether the commodity is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the deactivation timestamp.
    /// </summary>
    public DateTime? DeactivatedAt { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
