using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Dto;
using AntLogistics.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace AntLogistics.Core.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["SkipDbInit"] = "true"
            };
            config.AddInMemoryCollection(dict);
        });

        builder.ConfigureServices(services =>
        {
            // Replace services with in-memory stubs to avoid DB usage
            services.RemoveAll<IAuthorizationService>();
            services.RemoveAll<IWarehouseService>();
            services.RemoveAll<ICommodityService>();
            services.RemoveAll<IStockService>();

            services.AddSingleton<IAuthorizationService>(sp => new StubAuthorizationService());
            services.AddSingleton<IWarehouseService>(sp => new StubWarehouseService());
            services.AddSingleton<ICommodityService>(sp => new StubCommodityService());
            services.AddSingleton<IStockService>(sp => new StubStockService());

            // No DB initialization required due to SkipDbInit=true
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }
    }
}

file static class Stubs
{
}

public class StubAuthorizationService : IAuthorizationService
{
    public Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
        => Task.FromResult(!string.IsNullOrWhiteSpace(username)
                           && !string.IsNullOrWhiteSpace(password)
                           && username.Trim().Equals("tester", StringComparison.OrdinalIgnoreCase)
                           && password == "pass");
}

public class StubWarehouseService : IWarehouseService
{
    public Task<WarehouseResponse> CreateWarehouseAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new WarehouseResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code.ToLowerInvariant(),
            AddressLine = request.AddressLine,
            City = request.City,
            CountryCode = request.CountryCode.ToUpperInvariant(),
            PostalCode = request.PostalCode,
            DefaultZone = string.IsNullOrWhiteSpace(request.DefaultZone) ? "DEFAULT" : request.DefaultZone,
            Capacity = request.Capacity,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

    public Task<IEnumerable<WarehouseResponse>> GetAllWarehousesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<WarehouseResponse>>(Array.Empty<WarehouseResponse>());

    public Task<WarehouseResponse?> GetWarehouseByCodeAsync(string code, CancellationToken cancellationToken = default)
        => Task.FromResult<WarehouseResponse?>(null);

    public Task<WarehouseResponse?> GetWarehouseByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult<WarehouseResponse?>(null);
}

public class StubCommodityService : ICommodityService
{
    public Task<CommodityResponse> CreateCommodityAsync(CreateCommodityRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new CommodityResponse
        {
            Id = Guid.NewGuid(),
            Sku = request.Sku.ToLowerInvariant(),
            Name = request.Name,
            UnitOfMeasure = request.UnitOfMeasure,
            ControlParameters = string.IsNullOrWhiteSpace(request.ControlParameters) ? "{}" : request.ControlParameters,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

    public Task<IEnumerable<CommodityResponse>> GetAllCommoditiesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<CommodityResponse>>(Array.Empty<CommodityResponse>());

    public Task<CommodityResponse?> GetCommodityByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult<CommodityResponse?>(null);

    public Task<CommodityResponse?> GetCommodityBySkuAsync(string sku, CancellationToken cancellationToken = default)
        => Task.FromResult<CommodityResponse?>(null);
}

public class StubStockService : IStockService
{
    public Task<StockResponse> CreateStockAsync(CreateStockRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new StockResponse
        {
            Id = 1,
            WarehouseId = request.WarehouseId,
            CommodityId = request.CommodityId,
            Sku = "sku",
            UnitOfMeasure = "pcs",
            Quantity = request.Quantity,
            WarehouseZone = string.IsNullOrWhiteSpace(request.WarehouseZone) ? "DEFAULT" : request.WarehouseZone,
            CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy) ? "system" : request.CreatedBy,
            Source = string.IsNullOrWhiteSpace(request.Source) ? "manual" : request.Source,
            OccurredAt = request.OccurredAt ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Metadata = string.IsNullOrWhiteSpace(request.Metadata) ? "{}" : request.Metadata
        });

    public Task<StockResponse?> GetStockByIdAsync(long id, CancellationToken cancellationToken = default)
        => Task.FromResult<StockResponse?>(null);

    public Task<IEnumerable<StockResponse>> GetStocksAsync(Guid? warehouseId = null, Guid? commodityId = null, DateTime? from = null, DateTime? to = null, int limit = 100, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<StockResponse>>(Array.Empty<StockResponse>());

    public Task<IEnumerable<StockResponse>> GetStocksByCommodityAsync(Guid commodityId, DateTime? from = null, DateTime? to = null, int limit = 100, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<StockResponse>>(Array.Empty<StockResponse>());

    public Task<IEnumerable<StockResponse>> GetStocksByWarehouseAsync(Guid warehouseId, DateTime? from = null, DateTime? to = null, int limit = 100, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<StockResponse>>(Array.Empty<StockResponse>());
}
