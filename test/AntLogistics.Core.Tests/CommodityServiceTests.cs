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
public class CommodityServiceTests
{
    private static AntLogisticsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AntLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AntLogisticsDbContext(options);
    }

    private static ILogger<CommodityService> CreateLogger() => new Mock<ILogger<CommodityService>>().Object;

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
}
