namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Represents a warehouse in the logistics system.
/// </summary>
public class Warehouse
{
    private string _code = string.Empty;
    private string _countryCode = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the warehouse.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the warehouse code - unique human-friendly identifier (max 50 characters, lowercase).
    /// </summary>
    public string Code
    {
        get => _code;
        set => _code = (value ?? throw new ArgumentNullException(nameof(value))).ToLowerInvariant();
    }

    /// <summary>
    /// Gets or sets the warehouse name (max 200 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the warehouse street address (text field).
    /// </summary>
    public string AddressLine { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city where the warehouse is located (max 100 characters).
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code (ISO 3166-1 alpha-2).
    /// </summary>
    public string CountryCode
    {
        get => _countryCode;
        set => _countryCode = (value ?? throw new ArgumentNullException(nameof(value))).ToUpperInvariant();
    }

    /// <summary>
    /// Gets or sets the postal/zip code (max 20 characters, optional).
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the default operational zone for this warehouse (max 100 characters).
    /// </summary>
    public string DefaultZone { get; set; } = "DEFAULT";

    /// <summary>
    /// Gets or sets the warehouse capacity in configured units (must be > 0).
    /// </summary>
    public decimal Capacity { get; set; }

    /// <summary>
    /// Gets or sets whether the warehouse is currently active (soft-delete flag).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the timestamp when the warehouse was deactivated (soft-delete).
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
    /// Gets or sets the collection of inventory readings for this warehouse (1:N relationship).
    /// </summary>
    public ICollection<Reading> Readings { get; set; } = new List<Reading>();
}
