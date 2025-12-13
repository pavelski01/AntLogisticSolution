using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Dto;
using AntLogistics.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AntLogistics.Core.Tests;

[TestClass]
public class WarehouseServiceTests
{
    private static AntLogisticsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AntLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AntLogisticsDbContext(options);
    }

    private static ILogger<WarehouseService> CreateLogger() => new Mock<ILogger<WarehouseService>>().Object;

    [TestMethod]
    public async Task CreateWarehouseAsync_Throws_WhenCapacityNotPositive()
    {
        using var ctx = CreateContext();
        var svc = new WarehouseService(ctx, CreateLogger());

        var req = new CreateWarehouseRequest
        {
            Name = "Main",
            Code = "WH-1",
            AddressLine = "Addr",
            City = "City",
            CountryCode = "US",
            Capacity = 0,
            IsActive = true
        };

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateWarehouseAsync(req));
    }

    [TestMethod]
    public async Task CreateWarehouseAsync_Throws_WhenCountryCodeInvalid()
    {
        using var ctx = CreateContext();
        var svc = new WarehouseService(ctx, CreateLogger());

        var req = new CreateWarehouseRequest
        {
            Name = "Main",
            Code = "WH-1",
            AddressLine = "Addr",
            City = "City",
            CountryCode = "USA",
            Capacity = 10,
            IsActive = true
        };

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateWarehouseAsync(req));
    }

    [TestMethod]
    public async Task CreateWarehouseAsync_Throws_WhenDuplicateCode()
    {
        using var ctx = CreateContext();
        // Seed an existing warehouse with same code (normalized lowercase)
        ctx.Warehouses.Add(new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Existing",
            Code = "wh-1",
            AddressLine = "A",
            City = "C",
            CountryCode = "US",
            Capacity = 50,
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        var svc = new WarehouseService(ctx, CreateLogger());
        var req = new CreateWarehouseRequest
        {
            Name = "Another",
            Code = "WH-1", // different case, should normalize and conflict
            AddressLine = "B",
            City = "C",
            CountryCode = "US",
            Capacity = 25,
            IsActive = true
        };

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateWarehouseAsync(req));
    }

    [TestMethod]
    public async Task CreateWarehouseAsync_SetsDefaults_And_NormalizesFields()
    {
        using var ctx = CreateContext();
        var svc = new WarehouseService(ctx, CreateLogger());

        var req = new CreateWarehouseRequest
        {
            Name = "Main",
            Code = "Wh-Alpha",
            AddressLine = "Addr",
            City = "Seattle",
            CountryCode = "us",
            PostalCode = null,
            DefaultZone = "",
            Capacity = 100,
            IsActive = true
        };

        var res = await svc.CreateWarehouseAsync(req);

        Assert.AreEqual("wh-alpha", res.Code);
        Assert.AreEqual("US", res.CountryCode);
        Assert.AreEqual("DEFAULT", res.DefaultZone);
        Assert.IsTrue(res.IsActive);
        Assert.AreNotEqual(default(DateTime), res.CreatedAt);
        Assert.AreNotEqual(default(DateTime), res.UpdatedAt);
    }

    [TestMethod]
    public async Task GetWarehouseByCodeAsync_ReturnsNull_WhenEmptyOrMissing()
    {
        using var ctx = CreateContext();
        var svc = new WarehouseService(ctx, CreateLogger());

        var r1 = await svc.GetWarehouseByCodeAsync("");
        var r2 = await svc.GetWarehouseByCodeAsync("unknown");

        Assert.IsNull(r1);
        Assert.IsNull(r2);
    }

    [TestMethod]
    public async Task GetAllWarehousesAsync_FiltersInactive_WhenFlagFalse()
    {
        using var ctx = CreateContext();
        ctx.Warehouses.AddRange(
            new Warehouse { Name = "A", Code = "a", AddressLine = "x", City = "c", CountryCode = "US", Capacity = 10, IsActive = true },
            new Warehouse { Name = "B", Code = "b", AddressLine = "x", City = "c", CountryCode = "US", Capacity = 10, IsActive = false }
        );
        await ctx.SaveChangesAsync();

        var svc = new WarehouseService(ctx, CreateLogger());
        var onlyActive = await svc.GetAllWarehousesAsync();
        var withInactive = await svc.GetAllWarehousesAsync(includeInactive: true);

        Assert.AreEqual(1, onlyActive.Count());
        Assert.AreEqual(2, withInactive.Count());
    }
}
