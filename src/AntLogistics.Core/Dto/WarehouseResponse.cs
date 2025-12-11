namespace AntLogistics.Core.Dto;

/// <summary>
/// Response DTO for warehouse information.
/// </summary>
public record WarehouseResponse
{
    /// <summary>
    /// Gets the unique identifier for the warehouse.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the warehouse name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the warehouse code.
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
    /// Gets the country code where the warehouse is located.
    /// </summary>
    public required string CountryCode { get; init; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    /// Gets the default operating zone for the warehouse.
    /// </summary>
    public required string DefaultZone { get; init; }

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
    /// Gets the timestamp when the warehouse was deactivated.
    /// </summary>
    public DateTime? DeactivatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when the warehouse was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; init; }
}
