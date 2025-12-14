using System.Net;
using System.Net.Http.Json;
using AntLogistics.Core.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntLogistics.Core.Tests;

[TestClass]
public class ApiIntegrationTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [TestMethod]
    public async Task Unauthorized_Endoints_Return_401()
    {
        var res = await _client.GetAsync("/api/v1/commodities");
        Assert.AreEqual(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [TestMethod]
    public async Task Login_SetsCookie_And_Allows_Authorized_Calls()
    {
        var login = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Username = "tester", Password = "pass" });
        Assert.AreEqual(HttpStatusCode.OK, login.StatusCode);
        var body = await login.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.IsNotNull(body);
        Assert.IsTrue(body!.Success);
        Assert.IsFalse(string.IsNullOrWhiteSpace(body.Token));

        var me = await _client.GetAsync("/api/v1/auth/me");
        Assert.AreEqual(HttpStatusCode.OK, me.StatusCode);
    }

    [TestMethod]
    public async Task Create_Warehouse_Returns_Created()
    {
        // login first
        var login = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Username = "tester", Password = "pass" });
        Assert.AreEqual(HttpStatusCode.OK, login.StatusCode);

        var req = new CreateWarehouseRequest
        {
            Name = "Main",
            Code = "WH-1",
            AddressLine = "Addr",
            City = "City",
            CountryCode = "US",
            Capacity = 100,
            IsActive = true
        };

        var res = await _client.PostAsJsonAsync("/api/v1/warehouses", req);
        Assert.AreEqual(HttpStatusCode.Created, res.StatusCode);
    }

    [TestMethod]
    public async Task Create_Commodity_Returns_Created()
    {
        // login first
        var login = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest { Username = "tester", Password = "pass" });
        Assert.AreEqual(HttpStatusCode.OK, login.StatusCode);

        var req = new CreateCommodityRequest
        {
            Sku = "SKU-ALPHA",
            Name = "Alpha",
            UnitOfMeasure = "pcs"
        };

        var res = await _client.PostAsJsonAsync("/api/v1/commodities", req);
        Assert.AreEqual(HttpStatusCode.Created, res.StatusCode);
    }
}
