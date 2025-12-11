namespace AntLogistics.Core.Dto;

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
    /// Gets the warehouse address line.
    /// </summary>
    public required string AddressLine { get; init; }

    /// <summary>
    /// Gets the city where the warehouse is located.
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Gets the country code (ISO 3166-1 alpha-2).
    /// </summary>
    public required string CountryCode { get; init; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    /// Gets the default operating zone for the warehouse.
    /// </summary>
    public string DefaultZone { get; init; } = "DEFAULT";

    /// <summary>
    /// Gets the warehouse capacity.
    /// </summary>
    public decimal Capacity { get; init; }

    /// <summary>
    /// Gets whether the warehouse is active.
    /// </summary>
    public bool IsActive { get; init; } = true;
}
