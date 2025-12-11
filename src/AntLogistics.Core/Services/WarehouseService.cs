using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Dto;
using Microsoft.EntityFrameworkCore;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service implementation for warehouse operations.
/// </summary>
public class WarehouseService : IWarehouseService
{
    private readonly AntLogisticsDbContext _context;
    private readonly ILogger<WarehouseService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WarehouseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public WarehouseService(AntLogisticsDbContext context, ILogger<WarehouseService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<WarehouseResponse> CreateWarehouseAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating warehouse with code {Code}", request.Code);

        var normalizedCode = request.Code.ToLowerInvariant();

        if (request.Capacity <= 0)
        {
            _logger.LogWarning("Capacity {Capacity} is invalid for warehouse code {Code}", request.Capacity, request.Code);
            throw new InvalidOperationException("Warehouse capacity must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.CountryCode) || request.CountryCode.Length != 2)
        {
            _logger.LogWarning("Invalid country code {CountryCode} supplied for warehouse code {Code}", request.CountryCode, request.Code);
            throw new InvalidOperationException("Country code must be a two-letter ISO 3166-1 alpha-2 value.");
        }

        var existingWarehouse = await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Code == normalizedCode, cancellationToken);

        if (existingWarehouse is not null)
        {
            _logger.LogWarning("Warehouse with code {Code} already exists", request.Code);
            throw new InvalidOperationException($"Warehouse with code '{request.Code}' already exists.");
        }

        var warehouse = new Warehouse
        {
            Name = request.Name,
            Code = normalizedCode,
            AddressLine = request.AddressLine,
            City = request.City,
            CountryCode = request.CountryCode,
            PostalCode = request.PostalCode,
            DefaultZone = string.IsNullOrWhiteSpace(request.DefaultZone) ? "DEFAULT" : request.DefaultZone,
            Capacity = request.Capacity,
            IsActive = request.IsActive
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created warehouse {WarehouseId} with code {Code}", warehouse.Id, warehouse.Code);

        return MapToResponse(warehouse);
    }

    /// <inheritdoc/>
    public async Task<WarehouseResponse?> GetWarehouseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving warehouse with ID {WarehouseId}", id);

        var warehouse = await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (warehouse is null)
        {
            _logger.LogInformation("Warehouse with ID {WarehouseId} not found", id);
            return null;
        }

        return MapToResponse(warehouse);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<WarehouseResponse>> GetAllWarehousesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all warehouses (includeInactive: {IncludeInactive})", includeInactive);

        IQueryable<Warehouse> query = _context.Warehouses
            .AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(w => w.IsActive);
        }

        var warehouses = await query
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} warehouses", warehouses.Count);

        return warehouses.Select(MapToResponse);
    }

    /// <inheritdoc/>
    public async Task<WarehouseResponse?> GetWarehouseByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving warehouse with code {Code}", code);

        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogWarning("Warehouse code was not provided");
            return null;
        }

        var normalizedCode = code.ToLowerInvariant();

        var warehouse = await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Code == normalizedCode, cancellationToken);

        if (warehouse is null)
        {
            _logger.LogInformation("Warehouse with code {Code} not found", code);
            return null;
        }

        return MapToResponse(warehouse);
    }

    /// <summary>
    /// Maps a Warehouse entity to a WarehouseResponse DTO.
    /// </summary>
    /// <param name="warehouse">The warehouse entity.</param>
    /// <returns>The warehouse response DTO.</returns>
    private static WarehouseResponse MapToResponse(Warehouse warehouse)
    {
        return new WarehouseResponse
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            AddressLine = warehouse.AddressLine,
            City = warehouse.City,
            CountryCode = warehouse.CountryCode,
            PostalCode = warehouse.PostalCode,
            DefaultZone = warehouse.DefaultZone,
            Capacity = warehouse.Capacity,
            IsActive = warehouse.IsActive,
            DeactivatedAt = warehouse.DeactivatedAt,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt
        };
    }
}
