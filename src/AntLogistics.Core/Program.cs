var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults for Aspire integration
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddOpenApi();

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
