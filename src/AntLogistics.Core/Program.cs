using AntLogistics.Core.Data;
using AntLogistics.Core.Dto;
using AntLogistics.Core.Services;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<IReadingService, ReadingService>();

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

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

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

app.MapPost("/api/v1/readings", async (CreateReadingRequest request, IReadingService service, CancellationToken cancellationToken) =>
{
    try
    {
        var reading = await service.CreateReadingAsync(request, cancellationToken);
        return Results.Created($"/api/v1/readings/{reading.Id}", reading);
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
            title: "An error occurred while creating the reading");
    }
})
.WithName("CreateReading")
.Produces<ReadingResponse>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status500InternalServerError);

app.MapGet("/api/v1/readings/{id:long}", async (long id, IReadingService service, CancellationToken cancellationToken) =>
{
    var reading = await service.GetReadingByIdAsync(id, cancellationToken);
    return reading is not null ? Results.Ok(reading) : Results.NotFound();
})
.WithName("GetReadingById")
.Produces<ReadingResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/v1/readings", async (
    Guid? warehouseId,
    Guid? commodityId,
    DateTime? from,
    DateTime? to,
    int? limit,
    IReadingService service,
    CancellationToken cancellationToken) =>
{
    var readings = await service.GetReadingsAsync(
        warehouseId,
        commodityId,
        from,
        to,
        limit ?? 100,
        cancellationToken);
    return Results.Ok(readings);
})
.WithName("GetReadings")
.Produces<IEnumerable<ReadingResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/v1/warehouses/{warehouseId:guid}/readings", async (
    Guid warehouseId,
    DateTime? from,
    DateTime? to,
    int? limit,
    IReadingService service,
    CancellationToken cancellationToken) =>
{
    var readings = await service.GetReadingsByWarehouseAsync(
        warehouseId,
        from,
        to,
        limit ?? 100,
        cancellationToken);
    return Results.Ok(readings);
})
.WithName("GetReadingsByWarehouse")
.Produces<IEnumerable<ReadingResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/v1/commodities/{commodityId:guid}/readings", async (
    Guid commodityId,
    DateTime? from,
    DateTime? to,
    int? limit,
    IReadingService service,
    CancellationToken cancellationToken) =>
{
    var readings = await service.GetReadingsByCommodityAsync(
        commodityId,
        from,
        to,
        limit ?? 100,
        cancellationToken);
    return Results.Ok(readings);
})
.WithName("GetReadingsByCommodity")
.Produces<IEnumerable<ReadingResponse>>(StatusCodes.Status200OK);

app.Run();
