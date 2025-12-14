using AntLogistics.Core.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntLogistics.Core.Tests;

[TestClass]
public class ModelNormalizationTests
{
    [TestMethod]
    public void Warehouse_Code_IsLowercased_And_Country_Uppercased()
    {
        var w = new Warehouse();
        w.Code = "Wh-Alpha";
        w.CountryCode = "us";

        Assert.AreEqual("wh-alpha", w.Code);
        Assert.AreEqual("US", w.CountryCode);
    }

    [TestMethod]
    public void Operator_Username_IsLowercased()
    {
        var op = new Operator();
        op.Username = "AdminUser";

        Assert.AreEqual("adminuser", op.Username);
    }

    [TestMethod]
    public void Stock_Sku_IsLowercased()
    {
        var s = new Stock();
        s.Sku = "SKU-123";

        Assert.AreEqual("sku-123", s.Sku);
    }

    [TestMethod]
    public void Warehouse_Setters_Throw_On_Null()
    {
        var w = new Warehouse();
        Assert.ThrowsException<ArgumentNullException>(() => w.Code = null!);
        Assert.ThrowsException<ArgumentNullException>(() => w.CountryCode = null!);
    }

    [TestMethod]
    public void Commodity_Sku_Throws_On_Null()
    {
        var c = new Commodity();
        Assert.ThrowsException<ArgumentNullException>(() => c.Sku = null!);
    }
}
