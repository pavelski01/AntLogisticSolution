using AntLogistics.Core.Data;
using AntLogistics.Core.Dto;
using AntLogistics.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AntLogisticsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("antlogistics")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string not found.");

    options.UseNpgsql(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ICommodityService, CommodityService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
    else
    {
        throw new InvalidOperationException("JWT key is not configured. Set configuration key 'Jwt:Key'.");
    }
}
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AntLogistics";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "AntLogisticsClients";
var jwtExpiresMinutes = builder.Configuration.GetValue<int?>("Jwt:ExpiresMinutes") ?? 30;

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (ctx.Request.Cookies.TryGetValue("als_auth", out var token) && !string.IsNullOrWhiteSpace(token))
                {
                    ctx.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AntLogisticsDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/v1/warehouses", async (CreateWarehouseRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
{
    try
    {
        var warehouse = await service.CreateWarehouseAsync(request, cancellationToken);
        return Results.Created($"/api/v1/warehouses/{warehouse.Id}", warehouse);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while creating the warehouse");
    }
})
.WithName("CreateWarehouse")
.Produces<WarehouseResponse>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status500InternalServerError);

app.MapPost("/api/v1/auth/login", async (LoginRequest request, IAuthorizationService auth, HttpResponse httpResponse, CancellationToken ct) =>
{
    var success = await auth.ValidateCredentialsAsync(request.Username, request.Password, ct);
    if (!success)
    {
        return Results.Ok(new LoginResponse { Success = false });
    }

    var normalized = request.Username.Trim().ToLowerInvariant();
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, normalized),
        new Claim(JwtRegisteredClaimNames.Sub, normalized),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    var jwt = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(jwtExpiresMinutes),
        signingCredentials: creds);

    var token = new JwtSecurityTokenHandler().WriteToken(jwt);

    var cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = app.Environment.IsProduction(),
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddMinutes(jwtExpiresMinutes),
        Path = "/"
    };
    httpResponse.Cookies.Append("als_auth", token, cookieOptions);

    return Results.Ok(new { success = true, username = normalized });
})
.WithName("OperatorLogin")
.Produces<LoginResponse>(StatusCodes.Status200OK);

app.MapGet("/api/v1/auth/me", (HttpContext ctx) =>
{
    if (ctx.User?.Identity?.IsAuthenticated == true)
    {
        var name = ctx.User.FindFirstValue(ClaimTypes.Name) ?? ctx.User.Identity?.Name ?? "";
        return Results.Ok(new { username = name });
    }
    return Results.Unauthorized();
})
.RequireAuthorization()
.WithName("OperatorMe")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized);

app.MapPost("/api/v1/auth/logout", (HttpResponse res) =>
{
    res.Cookies.Append("als_auth", string.Empty, new CookieOptions
    {
        HttpOnly = true,
        Secure = app.Environment.IsProduction(),
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddDays(-1),
        Path = "/"
    });
    return Results.Ok(new { success = true });
})
.WithName("OperatorLogout")
.Produces(StatusCodes.Status200OK);

app.MapGet("/api/v1/warehouses/{id:guid}", async (Guid id, IWarehouseService service, CancellationToken cancellationToken) =>
{
    var warehouse = await service.GetWarehouseByIdAsync(id, cancellationToken);
    return warehouse is not null ? Results.Ok(warehouse) : Results.NotFound();
})
.WithName("GetWarehouseById")
.Produces<WarehouseResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/v1/warehouses", async (bool includeInactive, IWarehouseService service, CancellationToken cancellationToken) =>
{
    var warehouses = await service.GetAllWarehousesAsync(includeInactive, cancellationToken);
    return Results.Ok(warehouses);
})
.WithName("GetAllWarehouses")
.Produces<IEnumerable<WarehouseResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/v1/warehouses/by-code/{code}", async (string code, IWarehouseService service, CancellationToken cancellationToken) =>
{
    var warehouse = await service.GetWarehouseByCodeAsync(code, cancellationToken);
    return warehouse is not null ? Results.Ok(warehouse) : Results.NotFound();
})
.WithName("GetWarehouseByCode")
.Produces<WarehouseResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/v1/commodities", async (CreateCommodityRequest request, ICommodityService service, CancellationToken cancellationToken) =>
{
    try
    {
        var commodity = await service.CreateCommodityAsync(request, cancellationToken);
        return Results.Created($"/api/v1/commodities/{commodity.Id}", commodity);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while creating the commodity");
    }
})
.WithName("CreateCommodity")
.Produces<CommodityResponse>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status500InternalServerError);

app.MapGet("/api/v1/commodities/{id:guid}", async (Guid id, ICommodityService service, CancellationToken cancellationToken) =>
{
    var commodity = await service.GetCommodityByIdAsync(id, cancellationToken);
    return commodity is not null ? Results.Ok(commodity) : Results.NotFound();
})
.WithName("GetCommodityById")
.Produces<CommodityResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/v1/commodities", async (bool includeInactive, ICommodityService service, CancellationToken cancellationToken) =>
{
    var commodities = await service.GetAllCommoditiesAsync(includeInactive, cancellationToken);
    return Results.Ok(commodities);
})
.WithName("GetAllCommodities")
.Produces<IEnumerable<CommodityResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/v1/commodities/by-sku/{sku}", async (string sku, ICommodityService service, CancellationToken cancellationToken) =>
{
    var commodity = await service.GetCommodityBySkuAsync(sku, cancellationToken);
    return commodity is not null ? Results.Ok(commodity) : Results.NotFound();
})
.WithName("GetCommodityBySku")
.Produces<CommodityResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/v1/stocks", async (CreateStockRequest request, IStockService service, CancellationToken cancellationToken) =>
{
    try
    {
        var stock = await service.CreateStockAsync(request, cancellationToken);
        return Results.Created($"/api/v1/stocks/{stock.Id}", stock);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while creating the stock record");
    }
})
.WithName("CreateStock")
.Produces<StockResponse>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status500InternalServerError);

app.MapGet("/api/v1/stocks/{id:long}", async (long id, IStockService service, CancellationToken cancellationToken) =>
{
    var stock = await service.GetStockByIdAsync(id, cancellationToken);
    return stock is not null ? Results.Ok(stock) : Results.NotFound();
})
.WithName("GetStockById")
.Produces<StockResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/v1/stocks", async (
    Guid? warehouseId,
    Guid? commodityId,
    DateTime? from,
    DateTime? to,
    int? limit,
    IStockService service,
    CancellationToken cancellationToken) =>
{
    var stocks = await service.GetStocksAsync(
        warehouseId,
        commodityId,
        from,
        to,
        limit ?? 100,
        cancellationToken);
    return Results.Ok(stocks);
})
.WithName("GetStocks")
.Produces<IEnumerable<StockResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/v1/warehouses/{warehouseId:guid}/stocks", async (
    Guid warehouseId,
    DateTime? from,
    DateTime? to,
    int? limit,
    IStockService service,
    CancellationToken cancellationToken) =>
{
    var stocks = await service.GetStocksByWarehouseAsync(
        warehouseId,
        from,
        to,
        limit ?? 100,
        cancellationToken);
    return Results.Ok(stocks);
})
.WithName("GetStocksByWarehouse")
.Produces<IEnumerable<StockResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/v1/commodities/{commodityId:guid}/stocks", async (
    Guid commodityId,
    DateTime? from,
    DateTime? to,
    int? limit,
    IStockService service,
    CancellationToken cancellationToken) =>
{
    var stocks = await service.GetStocksByCommodityAsync(
        commodityId,
        from,
        to,
        limit ?? 100,
        cancellationToken);
    return Results.Ok(stocks);
})
.WithName("GetStocksByCommodity")
.Produces<IEnumerable<StockResponse>>(StatusCodes.Status200OK);

app.Run();
