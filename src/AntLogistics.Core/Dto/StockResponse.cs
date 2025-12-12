namespace AntLogistics.Core.Dto;

/// <summary>
/// Response DTO for a stock record.
/// </summary>
public record StockResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the stock record.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Gets or sets the warehouse identifier.
    /// </summary>
    public required Guid WarehouseId { get; init; }

    /// <summary>
    /// Gets or sets the commodity identifier.
    /// </summary>
    public required Guid CommodityId { get; init; }

    /// <summary>
    /// Gets or sets the SKU code.
    /// </summary>
    public required string Sku { get; init; }

    /// <summary>
    /// Gets or sets the unit of measure at the time of stock recording.
    /// </summary>
    public required string UnitOfMeasure { get; init; }

    /// <summary>
    /// Gets or sets the stock quantity.
    /// </summary>
    public required decimal Quantity { get; init; }

    /// <summary>
    /// Gets or sets the warehouse zone where the stock was recorded.
    /// </summary>
    public required string WarehouseZone { get; init; }

    /// <summary>
    /// Gets or sets the operator identifier who created the stock record.
    /// </summary>
    public Guid? OperatorId { get; init; }

    /// <summary>
    /// Gets or sets the operator label/name.
    /// </summary>
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Gets or sets the source of the stock record.
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when the stock was recorded physically.
    /// </summary>
    public required DateTime OccurredAt { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when the stock record was persisted to the database.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets free-form metadata as JSON string.
    /// </summary>
    public required string Metadata { get; init; }
}
