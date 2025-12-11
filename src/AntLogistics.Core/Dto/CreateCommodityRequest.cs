namespace AntLogistics.Core.Dto;

/// <summary>
/// Request model for creating a new commodity.
/// </summary>
public class CreateCommodityRequest
{
    /// <summary>
    /// Gets or sets the SKU (Stock Keeping Unit) code - globally unique (max 100 characters).
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the commodity name (max 200 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base unit of measure (e.g., kg, pcs, m) (max 20 characters).
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether batch/lot tracking is required for this commodity.
    /// </summary>
    public bool BatchRequired { get; set; }

    /// <summary>
    /// Gets or sets control parameters as JSON (e.g., temperature range, humidity constraints).
    /// </summary>
    public string ControlParameters { get; set; } = "{}";

    /// <summary>
    /// Gets or sets whether the commodity is initially active (default: true).
    /// </summary>
    public bool IsActive { get; set; } = true;
}
