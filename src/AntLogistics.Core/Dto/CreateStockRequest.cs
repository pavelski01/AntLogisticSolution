namespace AntLogistics.Core.Dto;

/// <summary>
/// Request DTO for creating a new stock record.
/// </summary>
public record CreateStockRequest
{
    /// <summary>
    /// Gets or sets the warehouse identifier.
    /// </summary>
    public required Guid WarehouseId { get; init; }

    /// <summary>
    /// Gets or sets the commodity identifier.
    /// </summary>
    public required Guid CommodityId { get; init; }

    /// <summary>
    /// Gets or sets the measured quantity.
    /// </summary>
    public required decimal Quantity { get; init; }

    /// <summary>
    /// Gets or sets the warehouse zone where the stock was recorded (optional, defaults to warehouse's default zone).
    /// </summary>
    public string? WarehouseZone { get; init; }

    /// <summary>
    /// Gets or sets the operator identifier who created the stock record (optional).
    /// </summary>
    public Guid? OperatorId { get; init; }

    /// <summary>
    /// Gets or sets the operator label/name for audit purposes (optional).
    /// </summary>
    public string? CreatedBy { get; init; }

    /// <summary>
    /// Gets or sets the source of the stock record (optional, defaults to "manual").
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when the stock was recorded physically (optional, defaults to current time).
    /// </summary>
    public DateTime? OccurredAt { get; init; }

    /// <summary>
    /// Gets or sets free-form metadata as JSON string (optional, defaults to "{}").
    /// </summary>
    public string? Metadata { get; init; }
}
