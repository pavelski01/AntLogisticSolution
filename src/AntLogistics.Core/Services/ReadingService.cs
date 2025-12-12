using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Dto;
using Microsoft.EntityFrameworkCore;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service implementation for reading operations.
/// </summary>
public class ReadingService : IReadingService
{
    private readonly AntLogisticsDbContext _context;
    private readonly ILogger<ReadingService> _logger;
    private const int MaxLimit = 1000;
    private const int DefaultLimit = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadingService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public ReadingService(AntLogisticsDbContext context, ILogger<ReadingService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<ReadingResponse> CreateReadingAsync(CreateReadingRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating reading for warehouse {WarehouseId} and commodity {CommodityId}", 
            request.WarehouseId, request.CommodityId);

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("Invalid quantity {Quantity} for reading", request.Quantity);
            throw new InvalidOperationException("Reading quantity must be greater than zero.");
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

        var reading = new Reading
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

        _context.Readings.Add(reading);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created reading {ReadingId} for warehouse {WarehouseId}", 
            reading.Id, reading.WarehouseId);

        return MapToResponse(reading);
    }

    /// <inheritdoc/>
    public async Task<ReadingResponse?> GetReadingByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving reading with ID {ReadingId}", id);

        var reading = await _context.Readings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reading is null)
        {
            _logger.LogInformation("Reading with ID {ReadingId} not found", id);
            return null;
        }

        return MapToResponse(reading);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReadingResponse>> GetReadingsAsync(
        Guid? warehouseId = null,
        Guid? commodityId = null,
        DateTime? from = null,
        DateTime? to = null,
        int limit = DefaultLimit,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving readings with filters: WarehouseId={WarehouseId}, CommodityId={CommodityId}, From={From}, To={To}, Limit={Limit}",
            warehouseId, commodityId, from, to, limit);

        var effectiveLimit = Math.Min(limit > 0 ? limit : DefaultLimit, MaxLimit);

        IQueryable<Reading> query = _context.Readings.AsNoTracking();

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

        var readings = await query
            .OrderByDescending(r => r.OccurredAt)
            .Take(effectiveLimit)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} readings", readings.Count);

        return readings.Select(MapToResponse);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReadingResponse>> GetReadingsByWarehouseAsync(
        Guid warehouseId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = DefaultLimit,
        CancellationToken cancellationToken = default)
    {
        return await GetReadingsAsync(warehouseId, null, from, to, limit, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReadingResponse>> GetReadingsByCommodityAsync(
        Guid commodityId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = DefaultLimit,
        CancellationToken cancellationToken = default)
    {
        return await GetReadingsAsync(null, commodityId, from, to, limit, cancellationToken);
    }

    /// <summary>
    /// Maps a Reading entity to a ReadingResponse DTO.
    /// </summary>
    /// <param name="reading">The reading entity.</param>
    /// <returns>The reading response DTO.</returns>
    private static ReadingResponse MapToResponse(Reading reading)
    {
        return new ReadingResponse
        {
            Id = reading.Id,
            WarehouseId = reading.WarehouseId,
            CommodityId = reading.CommodityId,
            Sku = reading.Sku,
            UnitOfMeasure = reading.UnitOfMeasure,
            Quantity = reading.Quantity,
            WarehouseZone = reading.WarehouseZone,
            OperatorId = reading.OperatorId,
            CreatedBy = reading.CreatedBy,
            Source = reading.Source,
            OccurredAt = reading.OccurredAt,
            CreatedAt = reading.CreatedAt,
            Metadata = reading.Metadata
        };
    }
}
