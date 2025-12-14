# AntLogistics

A modern logistics management solution built with .NET 10 and Astro/React. This microservices-based platform provides comprehensive tools for managing warehouses, inventory, orders, and shipments with built-in observability and resilience.

## Features

- **Warehouse Management**: Track and manage multiple warehouse locations
- **Inventory Control**: Real-time inventory tracking and management
- **Microservices Architecture**: Scalable, distributed services using .NET Aspire

## Getting Started

Follow these steps to run the application locally:

1. Clone the repository and navigate to the root directory:

   ```powershell
   git clone https://github.com/pavelski01/AntLogisticSolution.git
   cd AntLogisticSolution
   ```

2. Start the full application stack (PostgreSQL, Core API, and Astro UI) via Aspire AppHost:

   ```powershell
   dotnet run --project src/AntLogistics.AppHost/AntLogistics.AppHost.csproj
   ```

   - Core API will be available at http://localhost:5000
   - Astro UI will be proxied at http://localhost:4321

3. (Optional) Frontend-only mode:

   ```powershell
   cd src/AntLogistics.UI
   npm install
   npm run dev
   ```

4. (Optional) Database migrations (if model changes):

   ```powershell
   cd src/AntLogistics.Core
   dotnet ef migrations add MigrationName
   dotnet ef database update
   ```

5. Running tests:

   - Core unit tests: `dotnet test test/AntLogistics.Core.Tests/AntLogistics.Core.Tests.csproj`
   - UI tests: `npm run test --prefix src/AntLogistics.UI`

## Project Structure

```
AntLogisticSolution/
├── src/
│   ├── AntLogistics.AppHost/          # Aspire orchestration host
│   ├── AntLogistics.Core/             # Core API service
│   ├── AntLogistics.ServiceDefaults/  # Shared Aspire configurations
│   └── AntLogistics.UI/               # Astro + React frontend
├── test/
│   └── AntLogistics.Core.Tests/       # Unit tests for Core service
├── docs/                              # Project documentation
├── Directory.Build.props              # MSBuild properties
├── Directory.Packages.props           # Central package management
├── AntLogistics.slnx                  # Solution file
├── AntLogisticSolution.code-workspace # VS Code workspace settings
├── LICENSE                            # MIT License
└── README.md                          # Project overview
```

### Project Descriptions

- **AntLogistics.AppHost**: .NET Aspire orchestration host that starts and manages all services, including PostgreSQL with pgAdmin.
- **AntLogistics.Core**: Core API service with RESTful minimal APIs, Entity Framework Core, and PostgreSQL integration.
- **AntLogistics.ServiceDefaults**: Shared library providing OpenTelemetry, health checks, resilience patterns, and service discovery.
- **AntLogistics.UI**: Frontend application built with Astro and React, using TypeScript and Tailwind CSS, statically generated and proxying API requests to the backend.
- **AntLogistics.Core.Tests**: Unit test project using MSTest, NSubstitute for mocking, EF Core InMemory provider, and BCrypt for password hashing.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2025 Pavel Ski
