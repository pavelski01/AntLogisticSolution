using AntLogistics.Core.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults for Aspire integration
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddOpenApi();

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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Ping endpoint
app.MapGet("/api/v1/ping", () => 
{
    return Results.Ok(new { message = "Pong! AntLogistics.Core API is running." });
})
.WithName("Ping")
.WithOpenApi();

app.Run();
