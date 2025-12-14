# AntLogisticSolution - Technical Specification

## Overview
AntLogistics WMS MVP is a web application for warehouse management, enabling teams to record foundational data about warehouses, items (commodities), and goods readings. Built with .NET 10 and .NET Aspire 13.0 for cloud-native microservices orchestration, using PostgreSQL and Entity Framework Core. The MVP delivers a browser interface where authenticated operators can manage master data and capture inventory readings.

## Project Structure

```
AntLogisticSolution/
├── src/
│   ├── AntLogistics.AppHost/          # Aspire orchestration host
│   ├── AntLogistics.ServiceDefaults/  # Shared Aspire configurations
│   ├── AntLogistics.Core/             # Backend API service
│   │   ├── Data/
│   │   ├── Dto/
│   │   ├── Services/
│   │   └── Migrations/
│   └── AntLogistics.UI/               # Frontend (Astro + React)
│       ├── astro.config.mjs
│       ├── eslint.config.js
│       ├── package.json
│       ├── README.md
│       ├── tsconfig.json
│       ├── vitest.config.ts
│       ├── public/
│       └── src/
├── docs/                              # Documentation
│   ├── api-plan.md
│   ├── db-plan.md
│   ├── prd.md
│   └── technical-specification.md
├── .ai/                               # AI-assisted development files
│   └── prd.md                         # Product Requirements Document
├── .github/
│   ├── copilot-instructions.md        # GitHub Copilot guidelines
│   ├── instructions/                  # Custom instruction files
│   └── prompts/                       # Custom Copilot prompts
├── test/                              # Test projects
│   └── AntLogistics.Core.Tests/
└── AntLogistics.slnx                  # Solution file
```

## Core Components

### 1. AntLogistics.AppHost
**Purpose**: Aspire orchestration and service management

**Responsibilities**:
- Service lifecycle management
- Database provisioning (PostgreSQL)
- Service discovery configuration
- Monitoring and observability setup

**Key Features**:
- PostgreSQL container with pgAdmin
- Data volume persistence
- Automatic connection string injection
- Health check monitoring

### 2. AntLogistics.ServiceDefaults
**Purpose**: Shared configurations for all services

**Responsibilities**:
- Resilience policies (retry, circuit breaker, timeout)
- OpenTelemetry configuration
- Health check endpoints
- Service discovery setup

**Features**:
- Standardized telemetry
- Distributed tracing
- Metrics collection
- Logging configuration

### 3. AntLogistics.Core
**Purpose**: Core API service for logistics operations

**Responsibilities**:
- RESTful API endpoints
- Business logic execution
- Data access via EF Core
- Request/response handling

**Current Entities**:
- **Warehouse**: Warehouse management with location and capacity tracking
- **Commodity**: Item/product management with SKU, unit of measure, and stock tracking

**Database Context**:
- `AntLogisticsDbContext` with automatic timestamp management
- PostgreSQL provider with migrations support
- Automatic migrations on startup

### 4. AntLogistics.UI
**Purpose**: Frontend web application

**Technology Stack**:
- Astro framework for content-driven pages
- React components for interactive UI
- Vite for build tooling
- TypeScript for type safety

## Data Model

### Warehouse Entity
```csharp
- Id: Guid (Primary Key)
- Code: string (max 50 chars, lowercase, required)
- Name: string (max 200 chars, required)
- AddressLine: string (text field, required)
- City: string (max 100 chars, required)
- CountryCode: string (ISO 3166-1 alpha-2, uppercase, required)
- PostalCode: string? (max 20 chars, optional)
- DefaultZone: string (default "DEFAULT")
- Capacity: decimal (required)
- IsActive: bool (default: true)
- DeactivatedAt: DateTime? (soft-delete timestamp)
- CreatedAt: DateTime (UTC, auto-set)
- UpdatedAt: DateTime (UTC, auto-set)
- Stocks: ICollection<Stock> (navigation property)
```

### Commodity Entity
```csharp
- Id: Guid (Primary Key)
- Sku: string (max 100 chars, lowercase, required)
- Name: string (max 200 chars, required)
- UnitOfMeasure: string (max 20 chars, required)
- ControlParameters: string (JSON, default "{}")
- IsActive: bool (default: true)
- DeactivatedAt: DateTime? (soft-delete timestamp)
- CreatedAt: DateTime (UTC, auto-set)
- UpdatedAt: DateTime (UTC, auto-set)
- Stocks: ICollection<Stock> (navigation property)
```

### Stock Entity
```csharp
- Id: long (Primary Key)
- WarehouseId: Guid (Foreign Key)
- CommodityId: Guid (Foreign Key)
- Sku: string (denormalized SKU, lowercase)
- UnitOfMeasure: string (required)
- Quantity: decimal (required)
- WarehouseZone: string (default "DEFAULT")
- OperatorId: Guid? (optional)
- CreatedBy: string (operator label/name, required)
- Source: string (default "manual")
- OccurredAt: DateTime (physical timestamp)
- CreatedAt: DateTime (UTC, auto-set)
- Metadata: string (JSON, default "{}")
- Warehouse: Warehouse (navigation property)
- Commodity: Commodity (navigation property)
- Operator: Operator? (navigation property)
```

## Configuration Management## Configuration Management

### Central Package Management
- **File**: `Directory.Packages.props`
- **Enabled**: `ManagePackageVersionsCentrally=true`
- **Benefits**: Consistent package versions across all projects

### Key Dependencies
- Aspire.Hosting.* (13.0.2)
- Microsoft.EntityFrameworkCore (9.0.2)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.4)
- OpenTelemetry.* (1.14.0)
- Microsoft.Extensions.* (10.0.0)

## Development Guidelines

### Coding Standards
- **C# 12 features**: Primary constructors, record types, file-scoped namespaces
- **Nullable reference types**: Enabled in all projects
- **Async/await**: Required for all I/O operations
- **XML documentation**: Required for public APIs
- **SOLID principles**: Applied throughout the codebase

### Naming Conventions
- **PascalCase**: Classes, methods, properties, interfaces (I-prefix)
- **camelCase**: Local variables, parameters, private fields
- **Descriptive names**: Clear intent (e.g., `GetWarehouseByCode`)

### Error Handling
- Use specific exception types
- Log exceptions with context using `ILogger<T>`
- Return appropriate HTTP status codes
- Validate and sanitize all inputs

## API Design

### REST Conventions
- **Versioning**: `/api/v1/{resource}`
- **HTTP Verbs**: GET, POST, PUT, PATCH, DELETE
- **Status Codes**: 200, 201, 204, 400, 404, 500
- **Plural Resources**: `/warehouses` not `/warehouse`

### Example Endpoints (Implemented)

#### Health Checks (Development only)
```
GET    /health                        - Health check endpoint (all checks)
GET    /alive                         - Liveness endpoint
```

#### Authentication
```
POST   /api/v1/auth/login             - Operator login
GET    /api/v1/auth/me                - Get current operator
POST   /api/v1/auth/logout            - Operator logout
```

#### Warehouse Endpoints
```
POST   /api/v1/warehouses             - Create new warehouse
GET    /api/v1/warehouses             - List all warehouses (includeInactive flag)
GET    /api/v1/warehouses/{id}        - Get warehouse by ID
GET    /api/v1/warehouses/by-code/{code} - Get warehouse by code
```

#### Commodity Endpoints
```
POST   /api/v1/commodities            - Create new commodity
GET    /api/v1/commodities            - List all commodities (includeInactive flag)
GET    /api/v1/commodities/{id}       - Get commodity by ID
GET    /api/v1/commodities/by-sku/{sku} - Get commodity by SKU
```

#### Stock Endpoints
```
POST   /api/v1/stocks                 - Record stock reading
GET    /api/v1/stocks/{id}            - Get stock record by ID
GET    /api/v1/stocks                 - List stock records with filters (warehouseId, commodityId, from, to, limit)
GET    /api/v1/warehouses/{warehouseId}/stocks - Get stocks for warehouse
GET    /api/v1/commodities/{commodityId}/stocks - Get stocks for commodity
```

### Planned Endpoints

#### Warehouse Updates
```
PUT    /api/v1/warehouses/{id}          - Update warehouse
DELETE /api/v1/warehouses/{id}          - Deactivate warehouse
```

#### Commodity Updates
```
PUT    /api/v1/commodities/{id}         - Update commodity
DELETE /api/v1/commodities/{id}         - Deactivate commodity
```

#### Inventory Summary
```
GET    /api/v1/inventory                - View aggregated inventory per warehouse
```

## Observability

### OpenTelemetry Integration
- **Tracing**: Distributed tracing across services
- **Metrics**: Performance and business metrics
- **Logging**: Structured logging with correlation IDs

### Health Checks
- Database connectivity
- Service availability
- Resource utilization

### Aspire Dashboard
- Real-time service monitoring
- Log aggregation
- Trace visualization
- Metrics dashboards

## Security

### Best Practices
- No hardcoded secrets or connection strings
- User Secrets for local development
- Azure Key Vault for production (planned)
- Input validation and sanitization
- Parameterized queries (EF Core)

### Authentication (MVP - Planned)
- Single user role system
- Login-based authentication (username + bcrypt-hashed password)
- Browser session management (token/cookie)
- Automatic logout after idle timeout (configurable, default 30 minutes)
- No self-service registration or password reset in MVP

## Database Management

### Entity Framework Core
- **Migrations**: Code-first approach
- **Providers**: Npgsql for PostgreSQL
- **Features**: 
  - Automatic timestamp management
  - Query optimization
  - Connection resilience
  - Transaction support

### Migration Commands
```bash
# Create migration
dotnet ef migrations add <MigrationName> --project src/AntLogistics.Core

# Apply migrations
dotnet ef database update --project src/AntLogistics.Core

# Remove last migration
dotnet ef migrations remove --project src/AntLogistics.Core
```

## Running the Application

### Prerequisites
- .NET 10 SDK
- Docker Desktop (for PostgreSQL)
- Visual Studio 2022 or VS Code with C# DevKit

### Local Development
```bash
# Run via Aspire orchestration (recommended)
dotnet run --project src/AntLogistics.AppHost

# Run Core API standalone
dotnet run --project src/AntLogistics.Core

# Run UI frontend
cd src/AntLogistics.UI
npm install
npm run dev
```

### Aspire Dashboard
- Access: `http://localhost:15888` (or as shown in console)
- Features: Service logs, traces, metrics, resource management

## Testing Strategy (Planned)

### Unit Tests
- Framework: xUnit
- Mocking: Moq or NSubstitute
- Coverage: 80%+ on critical paths

### Integration Tests
- Database: TestContainers for PostgreSQL
- API: WebApplicationFactory
- End-to-end scenarios

### Performance Tests
- Load testing: k6 or JMeter
- Benchmarks: BenchmarkDotNet

## Deployment (Planned)

### Containerization
- Docker images for all services
- Multi-stage builds
- Optimized image sizes

### Cloud Deployment
- Azure Container Apps (recommended)
- Azure Database for PostgreSQL
- Azure Application Insights
- Azure Key Vault

## Roadmap

### Phase 1 - MVP (Current)
- [x] Project setup with Aspire
- [x] PostgreSQL integration
- [x] Warehouse entity and DbContext
- [x] Commodity entity
- [x] Warehouse CRUD endpoints (partial)
- [x] Initial migrations
- [x] Frontend project structure (Astro + React)
- [ ] Complete Warehouse CRUD (PUT, DELETE)
- [ ] Commodity CRUD endpoints
- [x] Receipt entry functionality
- [x] Inventory view per warehouse
- [x] User authentication (bcrypt)
- [x] Session management
- [ ] UI validation with error messages
- [x] OpenAPI documentation

### Phase 2 (Post-MVP)
- [ ] Receipt history with filters
- [ ] Operator activity list
- [ ] Advanced search and filtering
- [ ] Reporting capabilities
- [ ] Detailed change audit logs

### Phase 3 (Future)
- [ ] Multi-role account system
- [ ] SSO, MFA integration
- [ ] Mobile apps (Android/iOS)
- [ ] ERP/e-commerce integrations
- [ ] Advanced warehouse processes (picking, shipping, relocations)
- [ ] BI reports and analytics dashboards

## Success Metrics (MVP)
- 95% of readings recorded within 2 minutes of physical intake
- 90% of active items and warehouses modeled within 4 weeks of launch
- No more than 1 critical error (HTTP 5xx) per 500 requests over a month
- Average API response time under 800 ms for receipt creation
- 85% of pilot users report improved stock visibility

## Contributing

## Documentation

### GitHub Copilot Integration
- Project guidelines in `.github/copilot-instructions.md`
- Custom commands in `.github/prompts/`
- Custom instructions in `.github/instructions/`

## License
See [LICENSE](../LICENSE) file for details.

