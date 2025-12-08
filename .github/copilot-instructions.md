# GitHub Copilot Instructions for AntLogisticSolution

## Project Overview
This is the Ant Logistics solution built with .NET 9 and Aspire 13.0 for cloud-native microservices orchestration.

## Architecture
- <technology>.NET Aspire</technology>: Used for service orchestration and observability
- <pattern>Clean Architecture</pattern>: Separation of concerns across layers
- <pattern>Microservices</pattern>: Distributed services pattern
- <component>ServiceDefaults</component>: Shared configurations for resilience, telemetry, and service discovery

## Coding Standards

### C# Guidelines
- Use <version>C# 12</version> features and <version>.NET 9</version> runtime
- Enable <feature>nullable reference types</feature> in all projects
- Prefer <feature>primary constructors</feature> for simple classes
- Use <feature>record types</feature> for DTOs and immutable data
- Apply <feature>file-scoped namespaces</feature> to reduce indentation
- Use <feature>implicit usings</feature> where appropriate

### Naming Conventions
- <convention>PascalCase</convention>: Classes, methods, properties, interfaces (prefix with `I`)
- <convention>camelCase</convention>: Local variables, parameters, private fields
- <convention>Descriptive names</convention>: Convey intent clearly (e.g., `GetOrdersByCustomerId` not `GetOrders`)
- <convention>Abbreviations</convention>: Treat 2-letter abbreviations as words (e.g., `Io`, `Id`), capitalize 3+ letters only first letter (e.g., `Dto`, `Api`, `Html`, `Xml`)

### Code Organization
```
AntLogisticSolution/
├── src/
│   ├── AntLogistics.AppHost/          # Aspire orchestration
│   ├── AntLogistics.ServiceDefaults/  # Shared Aspire config
│   ├── AntLogistics.Api/              # RESTful API services
│   ├── AntLogistics.Domain/           # Business logic
│   └── AntLogistics.Infrastructure/   # Data access
```

## .NET Aspire Best Practices

### ServiceDefaults Integration
Always add ServiceDefaults to service projects:
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();
```

### AppHost Configuration
Register all services in AppHost:
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AntLogistics_Api>("api");
var web = builder.AddProject<Projects.AntLogistics_Web>("web")
    .WithReference(api);

builder.Build().Run();
```

### Observability
- Use OpenTelemetry for distributed tracing
- Add structured logging with `ILogger<T>`
- Implement health checks for all services
- Use metrics for performance monitoring

### Resilience
- Apply retry policies for transient failures
- Use circuit breakers for external dependencies
- Implement timeouts for all HTTP calls
- Handle failures gracefully

## Code Quality

### SOLID Principles
- <principle>S</principle>ingle Responsibility: One class, one purpose
- <principle>O</principle>pen/Closed: Open for extension, closed for modification
- <principle>L</principle>iskov Substitution: Subtypes must be substitutable
- <principle>I</principle>nterface Segregation: Many specific interfaces over one general
- <principle>D</principle>ependency Inversion: Depend on abstractions, not concretions

### Testing
- Write <testType>unit tests</testType> for business logic
- Create <testType>integration tests</testType> for APIs
- Use <framework>xUnit</framework> as the testing framework
- Mock dependencies with <library>Moq</library> or <library>NSubstitute</library>
- Aim for <metric>80%+ code coverage</metric> on critical paths

### Error Handling
```csharp
// Use specific exceptions
throw new InvalidOperationException("Order cannot be processed");

// Log exceptions with context
_logger.LogError(ex, "Failed to process order {OrderId}", orderId);

// Return appropriate HTTP status codes
return Results.NotFound($"Order {id} not found");
```

## Security

### Best Practices
- <warning>Never</warning> hardcode secrets or connection strings
- Use <tool>User Secrets</tool> for local development
- Store secrets in <service>Azure Key Vault</service> for production
- Validate and sanitize all user inputs
- Use <practice>parameterized queries</practice> to prevent SQL injection
- Implement proper <security>authentication</security> (JWT, OAuth2)
- Apply <security>authorization</security> policies for protected resources

### Example
```csharp
// Don't do this
var connectionString = "Server=myserver;Database=mydb;User=sa;Password=123";

// Do this
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

## Performance

### Async/Await
- Use `async/await` for all I/O operations
- Avoid `async void` (use `async Task` instead)
- Don't block on async code with `.Result` or `.Wait()`

### Database
- Use <technology>EF Core</technology> with async methods
- Implement <pattern>pagination</pattern> for large datasets
- Add <optimization>indexes</optimization> on frequently queried columns
- Use <optimization>compiled queries</optimization> for repeated operations
- Consider <optimization>NoTracking</optimization> for read-only queries

### Caching
- Use <pattern>distributed caching</pattern> (Redis) for shared data
- Implement <pattern>in-memory caching</pattern> for frequently accessed data
- Set appropriate <configuration>cache expiration</configuration> policies

## API Design

### REST Conventions
- Use proper HTTP verbs: GET, POST, PUT, PATCH, DELETE
- Return appropriate status codes (200, 201, 204, 400, 404, 500)
- Version your APIs (`/api/v1/orders`)
- Use plural nouns for resources (`/orders` not `/order`)

### Example
```csharp
app.MapGet("/api/v1/orders/{id}", async (int id, IOrderService service) =>
{
    var order = await service.GetOrderAsync(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

app.MapPost("/api/v1/orders", async (CreateOrderRequest request, IOrderService service) =>
{
    var orderId = await service.CreateOrderAsync(request);
    return Results.Created($"/api/v1/orders/{orderId}", new { id = orderId });
});
```

## Dependencies

### Package Management
- Use <feature>Central Package Management</feature> (Directory.Packages.props)
- Keep packages up to date
- Prefer <namespace>Microsoft.Extensions.*</namespace> packages
- Use official <component>Aspire components</component> for integrations

### Common Packages
- Aspire.Hosting (AppHost)
- Microsoft.Extensions.Http.Resilience
- Microsoft.Extensions.ServiceDiscovery
- OpenTelemetry.* (observability)

## Documentation

### Code Comments
- Do not add comments in code  
- Express intent through clear naming, simple structure, and tests.

## Git Practices

### Commit Messages
- Use conventional commits: `feat:`, `fix:`, `chore:`, `docs:`
- Keep first line under 50 characters
- Add detailed description if needed

### Branch Strategy
- `master`: Production-ready code
- `develop`: Integration branch

## Additional Guidelines

### Logging
```csharp
_logger.LogInformation("Processing order {OrderId} for customer {CustomerId}", 
    orderId, customerId);
```

### Configuration
```csharp
// Use strongly-typed configuration
builder.Services.Configure<OrderSettings>(
    builder.Configuration.GetSection("OrderSettings"));
```

### Dependency Injection
```csharp
// Register services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IOrderCache, RedisOrderCache>();
```

---

<reminder>Remember: Write clean, maintainable, and testable code that follows these guidelines. When in doubt, prioritize readability and simplicity over cleverness.</reminder>