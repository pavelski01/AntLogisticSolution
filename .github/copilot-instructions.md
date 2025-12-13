---
applyTo: '**'
---

# GitHub Copilot Instructions for AntLogisticSolution

## Project Overview
Warehouse management system (WMS) built with .NET 10 and Aspire 13.0. Architecture: backend API (`AntLogistics.Core`), Astro+React frontend (`AntLogistics.UI`), PostgreSQL database, orchestrated via Aspire AppHost.

## Running the Application

**Primary workflow** (from solution root):
```powershell
dotnet run --project src/AntLogistics.AppHost
```
This starts the entire stack: PostgreSQL, Core API, and Astro dev server.

**Frontend-only development**:
```powershell
cd src/AntLogistics.UI
npm run dev  # Astro dev server on port 4321
```

**Database migrations** (auto-applied on Core startup, or manually):
```powershell
cd src/AntLogistics.Core
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Architecture Patterns

### Service Communication
- **Aspire service discovery**: Astro dev server proxies `/api` requests to Core API using Aspire-injected environment variables `services__core__http__0`
- **Vite proxy configuration**: Configured in `astro.config.mjs` to handle API requests during development

### Project Structure (Actual)
```
src/
├── AntLogistics.AppHost/          # Aspire orchestration (PostgreSQL + services)
├── AntLogistics.ServiceDefaults/  # Shared: OpenTelemetry, health checks, resilience
├── AntLogistics.Core/             # Backend API (Minimal APIs + EF Core)
│   ├── Data/Models/               # Domain entities (Warehouse, Commodity, etc.)
│   ├── Dto/                       # Request/response DTOs
│   └── Services/                  # Business logic (interface + implementation)
└── AntLogistics.UI/               # Astro + React frontend (TypeScript, Tailwind v4)
```

### Key Technical Details
- **Target Framework**: .NET 10 (`net10.0` in all projects)
- **Central Package Management**: Versions in `Directory.Packages.props`
- **Nullable enabled**: All projects have `<Nullable>enable</Nullable>`
- **Implicit usings**: Enabled globally via `Directory.Build.props`
- **API versioning**: All endpoints use `/api/v1/` prefix
- **Soft deletes**: Entities use `IsActive` + `DeactivatedAt` pattern (see `Warehouse.cs`)

## Coding Standards

### C# Patterns (As Actually Used)

**Minimal APIs** (see `AntLogistics.Core/Program.cs`):
```csharp
app.MapPost("/api/v1/warehouses", async (CreateWarehouseRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
{
    var warehouse = await service.CreateWarehouseAsync(request, cancellationToken);
    return Results.Created($"/api/v1/warehouses/{warehouse.Id}", warehouse);
});
```

**Service layer** (see `WarehouseService.cs`):
- Constructor injection with null checks
- Structured logging with parameters: `_logger.LogInformation("Creating warehouse with code {Code}", request.Code)`
- Throw `InvalidOperationException` for business rule violations
- Use `AsNoTracking()` for read-only queries

**Database context** (see `AntLogisticsDbContext.cs`):
- Automatic timestamp management via `SaveChangesAsync` override
- Configure entities in `OnModelCreating` with Fluent API
- Use `DbSet<T>` properties with `= null!` (EF Core initialization pattern)

### Frontend Patterns

**Astro + React hybrid**:
- Static pages in `.astro` files (server-rendered)
- Interactive components in `.tsx` with `client:load` directive
- API calls via `/api` proxy (configured in `astro.config.mjs`)

**Styling**:
- Tailwind CSS v4 via `@tailwindcss/vite` plugin
- Use `@layer` directives for custom utilities

### Extension Method Conventions
Custom extensions in `Extensions/` folders:
- `AddServiceDefaults()` - Aspire integration (OpenTelemetry, health, resilience) used in Core API

## Configuration & Environment

**Connection strings**: Injected by Aspire AppHost as `antlogistics`, fallback to `DefaultConnection`

**AppHost orchestration** (see `AppHost/Program.cs`):
```csharp
var database = builder.AddPostgres("postgres").WithPgAdmin().AddDatabase("antlogistics");
var core = builder.AddProject<Projects.AntLogistics_Core>("core").WithReference(database);
var ui = builder.AddNpmApp("ui", "../AntLogistics.UI", "dev")
    .WithReference(core).WithHttpEndpoint(targetPort: 4321, isProxied: false)
    .WithExternalHttpEndpoints();
```

**Startup behavior**:
- Core API auto-applies migrations on startup (see `Program.cs` lines 48-65)
- CORS enabled for development with `AllowAnyOrigin`

## Entity & Data Patterns

**Domain models** inherit timestamps:
- `CreatedAt` / `UpdatedAt` managed automatically in `SaveChangesAsync`
- Soft deletes via `IsActive` + `DeactivatedAt?`
- Lowercase codes enforced in property setters (e.g., `Code.ToLowerInvariant()`)

**Validation**: Business rules in services, throw `InvalidOperationException` for violations

**DTOs**: Separate request/response types (e.g., `CreateWarehouseRequest` → `WarehouseResponse`)

## Testing & Debugging

**Health checks**: `/health` and `/alive` endpoints auto-configured by ServiceDefaults

**Aspire Dashboard**: Access telemetry, logs, and traces when running via AppHost

**Common issues**:
- File locking during build: Stop all .NET processes (Ctrl+C on AppHost terminal)
- Connection errors: Ensure PostgreSQL container is running (check Aspire dashboard)
- Astro proxy not working: Verify environment variables `services__core__http__0` are set

## Frontend-Specific Guidelines

See `.github/instructions/astro.instructions.md` and `react.instructions.md` for detailed Astro/React patterns.

**Key conventions**:
- Functional components with hooks (no classes)
- TypeScript interfaces over types
- Named exports for components
- Tailwind for styling, Zod for validation, Zustand for state

## Git & Development

**Commit format**: Conventional commits (`feat:`, `fix:`, `chore:`, `docs:`)

**Documentation**: Express intent through code, not comments. Don't use XML docs for public APIs.

**Dependencies**: Add packages via `Directory.Packages.props` (Central Package Management)

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
throw new InvalidOperationException("Order cannot be processed");

_logger.LogError(ex, "Failed to process order {OrderId}", orderId);

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