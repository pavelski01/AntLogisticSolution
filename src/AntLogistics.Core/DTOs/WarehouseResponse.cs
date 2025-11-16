namespace AntLogistics.Core.DTOs;

/// <summary>
/// Response DTO for warehouse information.
/// </summary>
public record WarehouseResponse
{
    /// <summary>
    /// Gets the unique identifier for the warehouse.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the warehouse name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the warehouse code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the warehouse address.
    /// </summary>
    public required string Address { get; init; }

    /// <summary>
    /// Gets the city where the warehouse is located.
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Gets the country where the warehouse is located.
    /// </summary>
    public required string Country { get; init; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    /// Gets the warehouse capacity in cubic meters.
    /// </summary>
    public decimal Capacity { get; init; }

    /// <summary>
    /// Gets whether the warehouse is currently active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the date and time when the warehouse was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when the warehouse was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the count of commodities stored in this warehouse.
    /// </summary>
    public int CommodityCount { get; init; }
}
