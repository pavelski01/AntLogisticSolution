### 1. 🎯 Project Goal and Introduction

The goal of this project is the design, development, and implementation of an integrated IT system of the **WMS (Warehouse Management System)** class. The system's main objective is the comprehensive management and optimization of **warehouse operations** within the organization, starting from goods receipt, through inventory tracking and storage, to the order fulfillment and dispatch (outbound) processes.

### 2. 📋 General Solution Description and Project Scope

The subject of the project is the delivery of dedicated software to support **warehouse management**. The system aims to automate key warehouse operations, provide full, real-time control over inventory levels, and streamline the flow of materials and information.

The solution will cover the following key areas of **warehouse management**:
* **Inbound Processes** (Advanced Shipping Notices, quantity and quality control).
* **Storage Management** (location assignment, space optimization).
* **Inventory Management** (stocktaking, internal warehouse transfers).
* **Outbound Processes** (order picking, packing, preparation for dispatch).
* **Reporting and Analytics**.

### 3. 📄 Functional Requirements – Key System Modules

The system will be built based on a modular architecture to ensure flexibility and scalability. The basic functional scope for each module is defined below.

#### 📦 Module 1: Goods Receipt Management

This module is responsible for handling all processes related to receiving goods into the warehouse.

* **Objective:** To ensure the correct registration and control of all goods entering the warehouse.
* **Key functionalities:**
    * Registration of Advanced Shipping Notices (ASN) from suppliers.
    * Verification of the physical delivery against the ASN document or Purchase Order (PO).
    * Handling of non-advised (unexpected) receipts.
    * Performing quantity checks and supporting quality control (QC) processes (e.g., directing goods to a QC zone).
    * Generating and printing logistic labels (e.g., pallet labels with a unique SSCC number or internal LPN - License Plate Number).
    * Management of the inbound buffer zone (e.g., receiving docks).
    * Recording batch/lot attributes (e.g., expiration date, batch/LOT number, serial number).

#### 🗺️ Module 2: Location Management and Put-away

This module is responsible for mapping the warehouse and intelligently managing the process of placing goods into storage locations.

* **Objective:** Optimization of warehouse space utilization and reduction of put-away operation time.
* **Key functionalities:**
    * Graphical or logical mapping of the warehouse structure (zones, aisles, racks, bin locations).
    * Defining location types (e.g., storage, picking, buffer, quarantine).
    * Automatic suggestion of a target storage location based on predefined rules (e.g., ABC strategy, product velocity, FIFO/LIFO/FEFO, dimensions, dedicated zones).
    * Registering operator confirmation of put-away (e.g., by scanning the location barcode).
    * Handling internal transfer operations (manual and system-directed).
    * Managing location optimization and defragmentation.

#### 📊 Module 3: Inventory Management and Stocktaking

This module serves as the central hub for real-time inventory records.

* **Objective:** To ensure 100% agreement between the system inventory and physical inventory, and to provide full traceability.
* **Key functionalities:**
    * Real-time presentation of inventory levels broken down by location, batch, expiration date, etc.
    * Management of inventory statuses (e.g., available, blocked, in quality control, reserved).
    * Full Lot (batch) and serial number tracking at every process stage (traceability).
    * Support for stocktaking (inventory count) processes:
        * Full (annual/periodic) inventory count.
        * Continuous (cycle) counting (by selected locations or products).
    * Generating count sheets and managing the reconciliation of inventory discrepancies.

#### 🚚 Module 4: Picking and Dispatch Management

This module manages the process of preparing goods for shipment according to customer orders.

* **Objective:** Minimization of picking time and errors, and ensuring timely preparation of shipments.
* **Key functionalities:**
    * Integration with the host system (e.g., ERP, e-commerce) to retrieve Sales Orders (SO).
    * Creating and assigning picking tasks to operators (picking lists).
    * Support for various picking strategies (e.g., single-order picking, batch picking, wave picking, multi-stage picking).
    * Optimization of the picking path (pathfinding) to shorten the operator's route.
    * Verification of picks (e.g., by scanning the product EAN code and/or location).
    * Support for packing processes (order consolidation, selection of packaging).
    * Generation of dispatch documents (e.g., Delivery Note) and shipping labels (integration with carriers).
    * Management of the outbound buffer zone (e.g., shipping docks).

# AntLogisticSolution - Technical Specification

## Overview
AntLogisticSolution is a modern logistics management system built with .NET 9 and .NET Aspire 13.0, designed to handle warehouse operations, order management, and supply chain logistics.

## Technology Stack

### Core Technologies
- **Runtime**: .NET 10
- **Language**: C# 12
- **Orchestration**: .NET Aspire 13.0
- **Database**: PostgreSQL with Entity Framework Core 9.0
- **API**: ASP.NET Core Minimal APIs with OpenAPI/Swagger

### Architecture Patterns
- **Clean Architecture**: Separation of concerns across layers
- **Microservices**: Distributed services pattern
- **Service Discovery**: Built-in with Aspire
- **Observability**: OpenTelemetry integration

## Project Structure

```
AntLogisticSolution/
├── src/
│   ├── AntLogistics.AppHost/          # Aspire orchestration host
│   ├── AntLogistics.ServiceDefaults/  # Shared Aspire configurations
│   ├── AntLogistics.Core/             # Core API service
│   ├── AntLogistics.Domain/           # Business logic (planned)
│   └── AntLogistics.Infrastructure/   # Data access (planned)
├── docs/                              # Documentation
├── .github/
│   ├── copilot-instructions.md        # GitHub Copilot guidelines
│   └── prompts/
│       └── custom-commands.prompt.md  # Custom Copilot commands
└── tests/                             # Test projects (planned)
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

**Database Context**:
- `AntLogisticsDbContext` with automatic timestamp management
- PostgreSQL provider with migrations support

## Data Model

### Warehouse Entity
```csharp
- Id: int (Primary Key)
- Name: string (200 chars, required)
- Code: string (50 chars, unique, required)
- Address: string (500 chars, required)
- City: string (100 chars, required)
- Country: string (100 chars, required)
- PostalCode: string? (20 chars, optional)
- Capacity: decimal (18,2 precision)
- IsActive: bool (default: true)
- CreatedAt: DateTime (UTC, auto-set)
- UpdatedAt: DateTime? (UTC, auto-set on modification)
```

## Configuration Management

### Central Package Management
- **File**: `Directory.Packages.props`
- **Enabled**: `ManagePackageVersionsCentrally=true`
- **Benefits**: Consistent package versions across all projects

### Key Dependencies
- Aspire.Hosting.* (13.0.0)
- Microsoft.EntityFrameworkCore (9.0.0)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.0)
- OpenTelemetry.* (1.10.0)
- Microsoft.Extensions.* (9.0.0)

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

### Example Endpoints (Planned)
```
GET    /api/v1/warehouses           - List all warehouses
GET    /api/v1/warehouses/{id}      - Get warehouse by ID
GET    /api/v1/warehouses/code/{code} - Get warehouse by code
POST   /api/v1/warehouses           - Create new warehouse
PUT    /api/v1/warehouses/{id}      - Update warehouse
DELETE /api/v1/warehouses/{id}      - Delete warehouse
GET    /api/v1/ping                 - Health check endpoint
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
- Authentication/Authorization (planned)

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
- .NET 9 SDK
- Docker Desktop (for PostgreSQL)
- Visual Studio 2022 or VS Code with C# DevKit

### Local Development
```bash
# Run via Aspire orchestration (recommended)
dotnet run --project src/AntLogistics.AppHost

# Run Core API standalone
dotnet run --project src/AntLogistics.Core
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

### Phase 1 (Current)
- [x] Project setup with Aspire
- [x] PostgreSQL integration
- [x] Warehouse entity and DbContext
- [ ] Warehouse CRUD endpoints
- [ ] Initial migrations

### Phase 2 (Planned)
- [ ] Customer entity and management
- [ ] Order entity and workflows
- [ ] OrderItem tracking
- [ ] Business validation rules
- [ ] Repository pattern implementation

### Phase 3 (Planned)
- [ ] Authentication/Authorization
- [ ] Advanced search and filtering
- [ ] Reporting capabilities
- [ ] Event-driven architecture
- [ ] Message queue integration

### Phase 4 (Planned)
- [ ] Web UI (Blazor or React)
- [ ] Mobile API support
- [ ] Analytics dashboard
- [ ] Third-party integrations

## Contributing

### Git Workflow
- **Branch**: `master` (production-ready)
- **Feature branches**: `feature/*`
- **Bug fixes**: `bugfix/*`
- **Hot fixes**: `hotfix/*`

### Commit Messages
- Use conventional commits: `feat:`, `fix:`, `chore:`, `docs:`
- Keep first line under 50 characters
- Add detailed description if needed

## Documentation

### Required Documentation
- XML comments for all public APIs
- README.md for each major component
- Architecture decision records (ADRs)
- API documentation via OpenAPI/Swagger

### GitHub Copilot Integration
- Custom commands in `.github/prompts/`
- Project guidelines in `.github/copilot-instructions.md`
- Commands: `/rewrite`, `/add-comments`, `/synchronize-dependencies`

## Support and Resources

### Internal Documentation
- [GitHub Copilot Instructions](../.github/copilot-instructions.md)
- [Custom Commands](../.github/prompts/custom-commands.prompt.md)

### External Resources
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

## License
See [LICENSE](../LICENSE) file for details.

---

**Last Updated**: November 16, 2025  
**Version**: 0.1.0-alpha  
**Status**: Active Development
