using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Dto;
using Microsoft.EntityFrameworkCore;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service implementation for stock operations.
/// </summary>
public class StockService : IStockService
{
    private readonly AntLogisticsDbContext _context;
    private readonly ILogger<StockService> _logger;
    private const int MaxLimit = 1000;
    private const int DefaultLimit = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="StockService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public StockService(AntLogisticsDbContext context, ILogger<StockService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<StockResponse> CreateStockAsync(CreateStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating stock record for warehouse {WarehouseId} and commodity {CommodityId}", 
            request.WarehouseId, request.CommodityId);

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("Invalid quantity {Quantity} for stock", request.Quantity);
            throw new InvalidOperationException("Stock quantity must be greater than zero.");
        }

        var warehouse = await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.IsActive, cancellationToken);

        if (warehouse is null)
        {
            _logger.LogWarning("Warehouse {WarehouseId} not found or inactive", request.WarehouseId);
            throw new InvalidOperationException($"Warehouse with ID '{request.WarehouseId}' not found or inactive.");
        }

        var commodity = await _context.Commodities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CommodityId && c.IsActive, cancellationToken);

        if (commodity is null)
        {
            _logger.LogWarning("Commodity {CommodityId} not found or inactive", request.CommodityId);
            throw new InvalidOperationException($"Commodity with ID '{request.CommodityId}' not found or inactive.");
        }

        if (request.OperatorId.HasValue)
        {
            var operatorExists = await _context.Operators
                .AsNoTracking()
                .AnyAsync(o => o.Id == request.OperatorId.Value, cancellationToken);

            if (!operatorExists)
            {
                _logger.LogWarning("Operator {OperatorId} not found", request.OperatorId);
                throw new InvalidOperationException($"Operator with ID '{request.OperatorId}' not found.");
            }
        }

        var stock = new Stock
        {
            WarehouseId = request.WarehouseId,
            CommodityId = request.CommodityId,
            Sku = commodity.Sku,
            UnitOfMeasure = commodity.UnitOfMeasure,
            Quantity = request.Quantity,
            WarehouseZone = string.IsNullOrWhiteSpace(request.WarehouseZone) ? warehouse.DefaultZone : request.WarehouseZone,
            OperatorId = request.OperatorId,
            CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy) ? "system" : request.CreatedBy,
            Source = string.IsNullOrWhiteSpace(request.Source) ? "manual" : request.Source,
            OccurredAt = request.OccurredAt ?? DateTime.UtcNow,
            Metadata = string.IsNullOrWhiteSpace(request.Metadata) ? "{}" : request.Metadata
        };

        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created stock record {StockId} for warehouse {WarehouseId}", 
            stock.Id, stock.WarehouseId);

        return MapToResponse(stock);
    }

    /// <inheritdoc/>
    public async Task<StockResponse?> GetStockByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving stock record with ID {StockId}", id);

        var stock = await _context.Stocks
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (stock is null)
        {
            _logger.LogInformation("Stock record with ID {StockId} not found", id);
            return null;
        }

        return MapToResponse(stock);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<StockResponse>> GetStocksAsync(
        Guid? warehouseId = null,
        Guid? commodityId = null,
        DateTime? from = null,
        DateTime? to = null,
        int limit = DefaultLimit,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving stock records with filters: WarehouseId={WarehouseId}, CommodityId={CommodityId}, From={From}, To={To}, Limit={Limit}",
            warehouseId, commodityId, from, to, limit);

        var effectiveLimit = Math.Min(limit > 0 ? limit : DefaultLimit, MaxLimit);

        IQueryable<Stock> query = _context.Stocks.AsNoTracking();

        if (warehouseId.HasValue)
        {
            query = query.Where(r => r.WarehouseId == warehouseId.Value);
        }

        if (commodityId.HasValue)
        {
            query = query.Where(r => r.CommodityId == commodityId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(r => r.OccurredAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(r => r.OccurredAt <= to.Value);
        }

        var stocks = await query
            .OrderByDescending(r => r.OccurredAt)
            .Take(effectiveLimit)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} stock records", stocks.Count);

        return stocks.Select(MapToResponse);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<StockResponse>> GetStocksByWarehouseAsync(
        Guid warehouseId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = DefaultLimit,
        CancellationToken cancellationToken = default)
    {
        return await GetStocksAsync(warehouseId, null, from, to, limit, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<StockResponse>> GetStocksByCommodityAsync(
        Guid commodityId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = DefaultLimit,
        CancellationToken cancellationToken = default)
    {
        return await GetStocksAsync(null, commodityId, from, to, limit, cancellationToken);
    }

    /// <summary>
    /// Maps a Stock entity to a StockResponse DTO.
    /// </summary>
    /// <param name="stock">The stock entity.</param>
    /// <returns>The stock response DTO.</returns>
    private static StockResponse MapToResponse(Stock stock)
    {
        return new StockResponse
        {
            Id = stock.Id,
            WarehouseId = stock.WarehouseId,
            CommodityId = stock.CommodityId,
            Sku = stock.Sku,
            UnitOfMeasure = stock.UnitOfMeasure,
            Quantity = stock.Quantity,
            WarehouseZone = stock.WarehouseZone,
            OperatorId = stock.OperatorId,
            CreatedBy = stock.CreatedBy,
            Source = stock.Source,
            OccurredAt = stock.OccurredAt,
            CreatedAt = stock.CreatedAt,
            Metadata = stock.Metadata
        };
    }
}
