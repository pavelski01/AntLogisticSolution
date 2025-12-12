using AntLogistics.Core.Dto;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service interface for managing stock records.
/// </summary>
public interface IStockService
{
    /// <summary>
    /// Creates a new stock record.
    /// </summary>
    /// <param name="request">The stock creation request.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The created stock response.</returns>
    Task<StockResponse> CreateStockAsync(CreateStockRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a stock record by its identifier.
    /// </summary>
    /// <param name="id">The stock identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The stock response or null if not found.</returns>
    Task<StockResponse?> GetStockByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all stock records with optional filtering.
    /// </summary>
    /// <param name="warehouseId">Optional warehouse filter.</param>
    /// <param name="commodityId">Optional commodity filter.</param>
    /// <param name="from">Optional start date filter (inclusive).</param>
    /// <param name="to">Optional end date filter (inclusive).</param>
    /// <param name="limit">Maximum number of stock records to return (default 100, max 1000).</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of stock responses.</returns>
    Task<IEnumerable<StockResponse>> GetStocksAsync(
        Guid? warehouseId = null,
        Guid? commodityId = null,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets stock records for a specific warehouse.
    /// </summary>
    /// <param name="warehouseId">The warehouse identifier.</param>
    /// <param name="from">Optional start date filter (inclusive).</param>
    /// <param name="to">Optional end date filter (inclusive).</param>
    /// <param name="limit">Maximum number of stock records to return (default 100, max 1000).</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of stock responses.</returns>
    Task<IEnumerable<StockResponse>> GetStocksByWarehouseAsync(
        Guid warehouseId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets stock records for a specific commodity.
    /// </summary>
    /// <param name="commodityId">The commodity identifier.</param>
    /// <param name="from">Optional start date filter (inclusive).</param>
    /// <param name="to">Optional end date filter (inclusive).</param>
    /// <param name="limit">Maximum number of stock records to return (default 100, max 1000).</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of stock responses.</returns>
    Task<IEnumerable<StockResponse>> GetStocksByCommodityAsync(
        Guid commodityId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default);
}
