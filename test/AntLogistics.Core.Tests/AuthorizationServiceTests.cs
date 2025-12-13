using AntLogistics.Core.Data;
using AntLogistics.Core.Data.Models;
using AntLogistics.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AntLogistics.Core.Tests;

[TestClass]
public class AuthorizationServiceTests
{
    private static AntLogisticsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AntLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AntLogisticsDbContext(options);
    }

    private static ILogger<AuthorizationService> CreateLogger() => Substitute.For<ILogger<AuthorizationService>>();

    [TestMethod]
    public async Task ValidateCredentialsAsync_ReturnsFalse_OnEmptyInput()
    {
        using var ctx = CreateContext();
        var svc = new AuthorizationService(ctx, CreateLogger());
        Assert.IsFalse(await svc.ValidateCredentialsAsync("", ""));
        Assert.IsFalse(await svc.ValidateCredentialsAsync("user", ""));
        Assert.IsFalse(await svc.ValidateCredentialsAsync("", "pass"));
    }

    [TestMethod]
    public async Task ValidateCredentialsAsync_ReturnsFalse_WhenUserNotFound()
    {
        using var ctx = CreateContext();
        var svc = new AuthorizationService(ctx, CreateLogger());
        Assert.IsFalse(await svc.ValidateCredentialsAsync("unknown", "pass"));
    }

    [TestMethod]
    public async Task ValidateCredentialsAsync_ReturnsTrue_OnValidCredentials_AndUpdatesLastLogin()
    {
        using var ctx = CreateContext();
        var password = "P@ssw0rd!";
        var op = new Operator
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            FullName = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = OperatorRole.Operator,
            IsActive = true
        };
        ctx.Operators.Add(op);
        await ctx.SaveChangesAsync();

        var svc = new AuthorizationService(ctx, CreateLogger());
        var ok = await svc.ValidateCredentialsAsync("TestUser", password); // case-insensitive username
        Assert.IsTrue(ok);

        var updated = await ctx.Operators.SingleAsync(o => o.Username == "testuser");
        Assert.IsNotNull(updated.LastLoginAt);
    }

    [TestMethod]
    public async Task ValidateCredentialsAsync_ReturnsFalse_OnInvalidPassword()
    {
        using var ctx = CreateContext();
        var op = new Operator
        {
            Id = Guid.NewGuid(),
            Username = "user",
            FullName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("right"),
            Role = OperatorRole.Operator,
            IsActive = true
        };
        ctx.Operators.Add(op);
        await ctx.SaveChangesAsync();

        var svc = new AuthorizationService(ctx, CreateLogger());
        Assert.IsFalse(await svc.ValidateCredentialsAsync("user", "wrong"));
    }
}
