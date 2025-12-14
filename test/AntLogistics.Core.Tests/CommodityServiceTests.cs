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
public class CommodityServiceTests
{
    private static AntLogisticsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AntLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AntLogisticsDbContext(options);
    }

    private static ILogger<CommodityService> CreateLogger() => Substitute.For<ILogger<CommodityService>>();

    [TestMethod]
    public async Task CreateCommodityAsync_Throws_OnMissingFields()
    {
        using var ctx = CreateContext();
        var svc = new CommodityService(ctx, CreateLogger());

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateCommodityAsync(new CreateCommodityRequest
        {
            Sku = "",
            Name = "Valid",
            UnitOfMeasure = "kg"
        }));

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateCommodityAsync(new CreateCommodityRequest
        {
            Sku = "sku-1",
            Name = "",
            UnitOfMeasure = "kg"
        }));

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateCommodityAsync(new CreateCommodityRequest
        {
            Sku = "sku-1",
            Name = "Item",
            UnitOfMeasure = ""
        }));
    }

    [TestMethod]
    public async Task CreateCommodityAsync_Throws_WhenDuplicateSku()
    {
        using var ctx = CreateContext();
        ctx.Commodities.Add(new Commodity
        {
            Id = Guid.NewGuid(),
            Sku = "sku-1",
            Name = "Existing",
            UnitOfMeasure = "kg",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        var svc = new CommodityService(ctx, CreateLogger());
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => svc.CreateCommodityAsync(new CreateCommodityRequest
        {
            Sku = "SKU-1",
            Name = "New",
            UnitOfMeasure = "kg"
        }));
    }

    [TestMethod]
    public async Task CreateCommodityAsync_NormalizesSku_And_Defaults()
    {
        using var ctx = CreateContext();
        var svc = new CommodityService(ctx, CreateLogger());

        var res = await svc.CreateCommodityAsync(new CreateCommodityRequest
        {
            Sku = "SKU-ALPHA",
            Name = "Alpha",
            UnitOfMeasure = "pcs",
            ControlParameters = ""
        });

        Assert.AreEqual("sku-alpha", res.Sku);
        Assert.AreEqual("{}", res.ControlParameters);
        Assert.IsTrue(res.IsActive);
        Assert.AreNotEqual(default(DateTime), res.CreatedAt);
    }

    [TestMethod]
    public async Task GetCommodityByIdAsync_ReturnsNull_WhenMissing_And_Found_WhenSeeded()
    {
        using var ctx = CreateContext();
        var svc = new CommodityService(ctx, CreateLogger());

        var missing = await svc.GetCommodityByIdAsync(Guid.NewGuid());
        Assert.IsNull(missing);

        var cm = new Commodity { Sku = "sku-2", Name = "Two", UnitOfMeasure = "pcs", IsActive = true };
        ctx.Commodities.Add(cm);
        await ctx.SaveChangesAsync();

        var found = await svc.GetCommodityByIdAsync(cm.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("sku-2", found!.Sku);
    }

    [TestMethod]
    public async Task GetAllCommoditiesAsync_FiltersInactive_ByDefault()
    {
        using var ctx = CreateContext();
        ctx.Commodities.AddRange(
            new Commodity { Sku = "a", Name = "A", UnitOfMeasure = "kg", IsActive = true },
            new Commodity { Sku = "b", Name = "B", UnitOfMeasure = "kg", IsActive = false }
        );
        await ctx.SaveChangesAsync();

        var svc = new CommodityService(ctx, CreateLogger());
        var onlyActive = await svc.GetAllCommoditiesAsync();
        var withInactive = await svc.GetAllCommoditiesAsync(includeInactive: true);

        Assert.AreEqual(1, onlyActive.Count());
        Assert.AreEqual(2, withInactive.Count());
    }
}
