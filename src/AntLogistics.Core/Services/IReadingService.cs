using AntLogistics.Core.Dto;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service interface for managing readings.
/// </summary>
public interface IReadingService
{
    /// <summary>
    /// Creates a new reading.
    /// </summary>
    /// <param name="request">The reading creation request.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The created reading response.</returns>
    Task<ReadingResponse> CreateReadingAsync(CreateReadingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a reading by its identifier.
    /// </summary>
    /// <param name="id">The reading identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The reading response or null if not found.</returns>
    Task<ReadingResponse?> GetReadingByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all readings with optional filtering.
    /// </summary>
    /// <param name="warehouseId">Optional warehouse filter.</param>
    /// <param name="commodityId">Optional commodity filter.</param>
    /// <param name="from">Optional start date filter (inclusive).</param>
    /// <param name="to">Optional end date filter (inclusive).</param>
    /// <param name="limit">Maximum number of readings to return (default 100, max 1000).</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of reading responses.</returns>
    Task<IEnumerable<ReadingResponse>> GetReadingsAsync(
        Guid? warehouseId = null,
        Guid? commodityId = null,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets readings for a specific warehouse.
    /// </summary>
    /// <param name="warehouseId">The warehouse identifier.</param>
    /// <param name="from">Optional start date filter (inclusive).</param>
    /// <param name="to">Optional end date filter (inclusive).</param>
    /// <param name="limit">Maximum number of readings to return (default 100, max 1000).</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of reading responses.</returns>
    Task<IEnumerable<ReadingResponse>> GetReadingsByWarehouseAsync(
        Guid warehouseId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets readings for a specific commodity.
    /// </summary>
    /// <param name="commodityId">The commodity identifier.</param>
    /// <param name="from">Optional start date filter (inclusive).</param>
    /// <param name="to">Optional end date filter (inclusive).</param>
    /// <param name="limit">Maximum number of readings to return (default 100, max 1000).</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of reading responses.</returns>
    Task<IEnumerable<ReadingResponse>> GetReadingsByCommodityAsync(
        Guid commodityId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default);
}
