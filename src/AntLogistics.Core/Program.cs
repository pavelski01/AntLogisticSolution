using AntLogistics.Core.Data;
using AntLogistics.Core.Dto;
using AntLogistics.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults for Aspire integration
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddOpenApi();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure PostgreSQL database context
builder.Services.AddDbContext<AntLogisticsDbContext>(options =>
{
    // Connection string will be provided by Aspire orchestration
    // or fall back to configuration for standalone running
    var connectionString = builder.Configuration.GetConnectionString("antlogistics")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string not found.");
    
    options.UseNpgsql(connectionString);
    
    // Enable detailed errors and sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register business services
builder.Services.AddScoped<IWarehouseService, WarehouseService>();

var app = builder.Build();

// Apply migrations on startup
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(); // Enable CORS in development
}

app.UseHttpsRedirection();

// Map default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Ping endpoint
app.MapGet("/api/v1/ping", () => 
{
    return Results.Ok(new { message = "Pong! AntLogistics.Core API is running." });
})
.WithName("Ping");

// Warehouse endpoints
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

app.MapGet("/api/v1/warehouses/{id:int}", async (int id, IWarehouseService service, CancellationToken cancellationToken) =>
{
    var warehouse = await service.GetWarehouseByIdAsync(id, cancellationToken);
    return Results.Ok(warehouse);
})
.WithName("GetWarehouseById")
.Produces<WarehouseResponse?>(StatusCodes.Status200OK);

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
    return Results.Ok(warehouse);
})
.WithName("GetWarehouseByCode")
.Produces<WarehouseResponse?>(StatusCodes.Status200OK);

app.Run();
