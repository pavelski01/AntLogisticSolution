namespace AntLogistics.Core.DTOs;

/// <summary>
/// Request DTO for creating a new warehouse.
/// </summary>
public record CreateWarehouseRequest
{
    /// <summary>
    /// Gets the warehouse name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the warehouse code (unique identifier for business use).
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
    /// Gets whether the warehouse is active.
    /// </summary>
    public bool IsActive { get; init; } = true;
}
