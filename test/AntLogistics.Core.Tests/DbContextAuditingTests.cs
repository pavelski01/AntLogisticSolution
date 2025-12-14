using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntLogistics.Core.Tests;

[TestClass]
public class DbContextAuditingTests
{
    private static AntLogisticsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AntLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AntLogisticsDbContext(options);
    }

    [TestMethod]
    public async Task SaveChangesAsync_Sets_CreatedAndUpdated_On_Add()
    {
        using var ctx = CreateContext();
        var wh = new Warehouse
        {
            Name = "WH",
            Code = "wh",
            AddressLine = "a",
            City = "c",
            CountryCode = "US",
            Capacity = 10,
            IsActive = true
        };
        ctx.Warehouses.Add(wh);
        await ctx.SaveChangesAsync();

        Assert.AreNotEqual(default, wh.CreatedAt);
        Assert.AreNotEqual(default, wh.UpdatedAt);
    }

    [TestMethod]
    public async Task SaveChangesAsync_Updates_UpdatedAt_On_Modify()
    {
        using var ctx = CreateContext();
        var wh = new Warehouse
        {
            Name = "WH",
            Code = "wh",
            AddressLine = "a",
            City = "c",
            CountryCode = "US",
            Capacity = 10,
            IsActive = true
        };
        ctx.Warehouses.Add(wh);
        await ctx.SaveChangesAsync();

        var originalUpdated = wh.UpdatedAt;
        wh.Name = "WH2";
        ctx.Warehouses.Update(wh);
        await Task.Delay(5);
        await ctx.SaveChangesAsync();

        Assert.IsTrue(wh.UpdatedAt >= originalUpdated);
    }

    [TestMethod]
    public async Task SaveChangesAsync_Sets_StockOccurredAt_When_Default()
    {
        using var ctx = CreateContext();
        var wh = new Warehouse { Name = "WH", Code = "wh", AddressLine = "a", City = "c", CountryCode = "US", Capacity = 10, IsActive = true };
        var cm = new Commodity { Sku = "sku", Name = "Item", UnitOfMeasure = "kg", IsActive = true };
        ctx.Warehouses.Add(wh);
        ctx.Commodities.Add(cm);
        await ctx.SaveChangesAsync();

        var st = new Stock
        {
            WarehouseId = wh.Id,
            CommodityId = cm.Id,
            Sku = cm.Sku,
            UnitOfMeasure = cm.UnitOfMeasure,
            Quantity = 1,
            CreatedBy = "tester"
        };
        ctx.Stocks.Add(st);
        await ctx.SaveChangesAsync();

        Assert.AreNotEqual(default, st.CreatedAt);
        Assert.AreNotEqual(default, st.OccurredAt);
        Assert.AreEqual(st.CreatedAt, st.OccurredAt);
    }
}
