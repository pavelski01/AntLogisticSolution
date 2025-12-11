using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Dto;
using Microsoft.EntityFrameworkCore;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service implementation for commodity operations.
/// </summary>
public class CommodityService : ICommodityService
{
    private readonly AntLogisticsDbContext _context;
    private readonly ILogger<CommodityService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommodityService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public CommodityService(AntLogisticsDbContext context, ILogger<CommodityService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<CommodityResponse> CreateCommodityAsync(CreateCommodityRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating commodity with SKU {Sku}", request.Sku);

        var normalizedSku = request.Sku.ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            _logger.LogWarning("SKU cannot be empty");
            throw new InvalidOperationException("SKU is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Commodity name cannot be empty");
            throw new InvalidOperationException("Commodity name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            _logger.LogWarning("Unit of measure cannot be empty for SKU {Sku}", request.Sku);
            throw new InvalidOperationException("Unit of measure is required.");
        }

        var existingCommodity = await _context.Commodities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Sku == normalizedSku, cancellationToken);

        if (existingCommodity is not null)
        {
            _logger.LogWarning("Commodity with SKU {Sku} already exists", request.Sku);
            throw new InvalidOperationException($"Commodity with SKU '{request.Sku}' already exists.");
        }

        var commodity = new Commodity
        {
            Sku = normalizedSku,
            Name = request.Name,
            UnitOfMeasure = request.UnitOfMeasure,
            BatchRequired = request.BatchRequired,
            ControlParameters = string.IsNullOrWhiteSpace(request.ControlParameters) ? "{}" : request.ControlParameters,
            IsActive = request.IsActive
        };

        _context.Commodities.Add(commodity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created commodity {CommodityId} with SKU {Sku}", commodity.Id, commodity.Sku);

        return MapToResponse(commodity);
    }

    /// <inheritdoc/>
    public async Task<CommodityResponse?> GetCommodityByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving commodity with ID {CommodityId}", id);

        var commodity = await _context.Commodities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (commodity is null)
        {
            _logger.LogInformation("Commodity with ID {CommodityId} not found", id);
            return null;
        }

        return MapToResponse(commodity);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CommodityResponse>> GetAllCommoditiesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all commodities (includeInactive: {IncludeInactive})", includeInactive);

        IQueryable<Commodity> query = _context.Commodities
            .AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var commodities = await query
            .OrderBy(c => c.Sku)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} commodities", commodities.Count);

        return commodities.Select(MapToResponse);
    }

    /// <inheritdoc/>
    public async Task<CommodityResponse?> GetCommodityBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving commodity with SKU {Sku}", sku);

        var normalizedSku = sku.ToLowerInvariant();

        var commodity = await _context.Commodities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Sku == normalizedSku, cancellationToken);

        if (commodity is null)
        {
            _logger.LogInformation("Commodity with SKU {Sku} not found", sku);
            return null;
        }

        return MapToResponse(commodity);
    }

    private static CommodityResponse MapToResponse(Commodity commodity)
    {
        return new CommodityResponse
        {
            Id = commodity.Id,
            Sku = commodity.Sku,
            Name = commodity.Name,
            UnitOfMeasure = commodity.UnitOfMeasure,
            BatchRequired = commodity.BatchRequired,
            ControlParameters = commodity.ControlParameters,
            IsActive = commodity.IsActive,
            DeactivatedAt = commodity.DeactivatedAt,
            CreatedAt = commodity.CreatedAt,
            UpdatedAt = commodity.UpdatedAt
        };
    }
}
