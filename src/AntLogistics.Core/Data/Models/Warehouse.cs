namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Represents a warehouse in the logistics system.
/// </summary>
public class Warehouse
{
    /// <summary>
    /// Gets or sets the unique identifier for the warehouse.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the warehouse name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the warehouse code (unique identifier for business use).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the warehouse address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city where the warehouse is located.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country where the warehouse is located.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the warehouse capacity in cubic meters.
    /// </summary>
    public decimal Capacity { get; set; }

    /// <summary>
    /// Gets or sets whether the warehouse is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the warehouse was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the warehouse was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
