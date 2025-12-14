using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Dto;
using AntLogistics.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AntLogistics.Core.Tests;

[TestClass]
public class StockServiceTests
{
    private static AntLogisticsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AntLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AntLogisticsDbContext(options);
    }

    private static ILogger<StockService> CreateLogger() => Substitute.For<ILogger<StockService>>();

    private static (Warehouse wh, Commodity cm) SeedWarehouseAndCommodity(AntLogisticsDbContext ctx)
    {
        var wh = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "WH",
            Code = "wh",
            AddressLine = "a",
            City = "c",
            CountryCode = "US",
            Capacity = 100,
            DefaultZone = "Z1",
            IsActive = true
        };
        var cm = new Commodity
        {
            Id = Guid.NewGuid(),
            Sku = "sku",
            Name = "Item",
            UnitOfMeasure = "kg",
            IsActive = true
        };
        ctx.Warehouses.Add(wh);
        ctx.Commodities.Add(cm);
        ctx.SaveChanges();
        return (wh, cm);
    }

    [TestMethod]
    public async Task CreateStockAsync_Throws_WhenQuantityInvalid()
    {
        using var ctx = CreateContext();
        var svc = new StockService(ctx, CreateLogger());

        var (wh, cm) = SeedWarehouseAndCommodity(ctx);

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateStockAsync(new CreateStockRequest
        {
            WarehouseId = wh.Id,
            CommodityId = cm.Id,
            Quantity = 0
        }));
    }

    [TestMethod]
    public async Task CreateStockAsync_Throws_WhenWarehouseMissingOrInactive()
    {
        using var ctx = CreateContext();
        var svc = new StockService(ctx, CreateLogger());
        var cm = new Commodity { Id = Guid.NewGuid(), Sku = "sku", Name = "Item", UnitOfMeasure = "kg", IsActive = true };
        ctx.Commodities.Add(cm);
        await ctx.SaveChangesAsync();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateStockAsync(new CreateStockRequest
        {
            WarehouseId = Guid.NewGuid(),
            CommodityId = cm.Id,
            Quantity = 1
        }));

        var whInactive = new Warehouse { Id = Guid.NewGuid(), Name = "X", Code = "x", AddressLine = "a", City = "c", CountryCode = "US", Capacity = 10, IsActive = false };
        ctx.Warehouses.Add(whInactive);
        await ctx.SaveChangesAsync();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateStockAsync(new CreateStockRequest
        {
            WarehouseId = whInactive.Id,
            CommodityId = cm.Id,
            Quantity = 1
        }));
    }

    [TestMethod]
    public async Task CreateStockAsync_Throws_WhenCommodityMissingOrInactive()
    {
        using var ctx = CreateContext();
        var svc = new StockService(ctx, CreateLogger());
        var wh = new Warehouse { Id = Guid.NewGuid(), Name = "X", Code = "x", AddressLine = "a", City = "c", CountryCode = "US", Capacity = 10, IsActive = true };
        ctx.Warehouses.Add(wh);
        await ctx.SaveChangesAsync();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateStockAsync(new CreateStockRequest
        {
            WarehouseId = wh.Id,
            CommodityId = Guid.NewGuid(),
            Quantity = 1
        }));

        var cmInactive = new Commodity { Id = Guid.NewGuid(), Sku = "sku", Name = "Item", UnitOfMeasure = "kg", IsActive = false };
        ctx.Commodities.Add(cmInactive);
        await ctx.SaveChangesAsync();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateStockAsync(new CreateStockRequest
        {
            WarehouseId = wh.Id,
            CommodityId = cmInactive.Id,
            Quantity = 1
        }));
    }

    [TestMethod]
    public async Task CreateStockAsync_Throws_WhenOperatorMissing()
    {
        using var ctx = CreateContext();
        var svc = new StockService(ctx, CreateLogger());
        var (wh, cm) = SeedWarehouseAndCommodity(ctx);

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateStockAsync(new CreateStockRequest
        {
            WarehouseId = wh.Id,
            CommodityId = cm.Id,
            Quantity = 1,
            OperatorId = Guid.NewGuid()
        }));
    }

    [TestMethod]
    public async Task CreateStockAsync_SetsDefaults_And_UsesProvidedValues()
    {
        using var ctx = CreateContext();
        var svc = new StockService(ctx, CreateLogger());
        var (wh, cm) = SeedWarehouseAndCommodity(ctx);

        var when = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var res = await svc.CreateStockAsync(new CreateStockRequest
        {
            WarehouseId = wh.Id,
            CommodityId = cm.Id,
            Quantity = 5.5m,
            WarehouseZone = null, // should fall back to warehouse default
            CreatedBy = null,     // should default to "system"
            Source = null,
            OccurredAt = when,
            Metadata = null
        });

        Assert.AreEqual(wh.Id, res.WarehouseId);
        Assert.AreEqual(cm.Id, res.CommodityId);
        Assert.AreEqual("sku", res.Sku);
        Assert.AreEqual("kg", res.UnitOfMeasure);
        Assert.AreEqual(5.5m, res.Quantity);
        Assert.AreEqual(wh.DefaultZone, res.WarehouseZone);
        Assert.AreEqual("system", res.CreatedBy);
        Assert.AreEqual("manual", res.Source);
        Assert.AreEqual(when, res.OccurredAt);
        Assert.AreNotEqual(default(DateTime), res.CreatedAt);
        Assert.AreEqual("{}", res.Metadata);
    }

    [TestMethod]
    public async Task GetStockByIdAsync_ReturnsNull_WhenMissing_And_Found_WhenSeeded()
    {
        using var ctx = CreateContext();
        var svc = new StockService(ctx, CreateLogger());

        var missing = await svc.GetStockByIdAsync(999);
        Assert.IsNull(missing);

        var (wh, cm) = SeedWarehouseAndCommodity(ctx);
        var s = new Stock
        {
            WarehouseId = wh.Id,
            CommodityId = cm.Id,
            Sku = cm.Sku,
            UnitOfMeasure = cm.UnitOfMeasure,
            Quantity = 1,
            CreatedBy = "tester",
            OccurredAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        ctx.Stocks.Add(s);
        await ctx.SaveChangesAsync();

        var found = await svc.GetStockByIdAsync(s.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(s.Id, found!.Id);
    }

    [TestMethod]
    public async Task GetStocksAsync_AppliesFilters_And_Limit()
    {
        using var ctx = CreateContext();
        var svc = new StockService(ctx, CreateLogger());
        var (wh, cm) = SeedWarehouseAndCommodity(ctx);

        // Seed three records with different timestamps
        var t1 = new DateTime(2023, 12, 31, 23, 0, 0, DateTimeKind.Utc);
        var t2 = new DateTime(2024, 01, 02, 0, 0, 0, DateTimeKind.Utc);
        var t3 = new DateTime(2024, 01, 03, 0, 0, 0, DateTimeKind.Utc);
        ctx.Stocks.AddRange(
            new Stock { WarehouseId = wh.Id, CommodityId = cm.Id, Sku = cm.Sku, UnitOfMeasure = cm.UnitOfMeasure, Quantity = 1, CreatedBy = "t", OccurredAt = t1 },
            new Stock { WarehouseId = wh.Id, CommodityId = cm.Id, Sku = cm.Sku, UnitOfMeasure = cm.UnitOfMeasure, Quantity = 2, CreatedBy = "t", OccurredAt = t2 },
            new Stock { WarehouseId = wh.Id, CommodityId = cm.Id, Sku = cm.Sku, UnitOfMeasure = cm.UnitOfMeasure, Quantity = 3, CreatedBy = "t", OccurredAt = t3 }
        );
        await ctx.SaveChangesAsync();

        var filtered = await svc.GetStocksAsync(warehouseId: wh.Id, from: new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), to: new DateTime(2024, 01, 03, 0, 0, 0, DateTimeKind.Utc));
        Assert.AreEqual(2, filtered.Count());

        var limited = await svc.GetStocksAsync(warehouseId: wh.Id, limit: 1);
        Assert.AreEqual(1, limited.Count());
    }
}
