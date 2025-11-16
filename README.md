# AntLogistics

A modern logistics management solution built with .NET 9 and Aspire 13.0. This microservices-based platform provides comprehensive tools for managing warehouses, inventory, orders, and shipments with built-in observability and resilience.

## Features

- **Warehouse Management**: Track and manage multiple warehouse locations
- **Inventory Control**: Real-time inventory tracking and management
- **Order Processing**: Efficient order fulfillment workflows
- **Microservices Architecture**: Scalable, distributed services using .NET Aspire
- **Observability**: Built-in distributed tracing, metrics, and health monitoring
- **Resilience**: Automatic retry policies and circuit breakers

## Getting Started

TODO: Add setup and usage instructions.

## Project Structure

```
AntLogisticSolution/
├── src/
│   ├── AntLogistics.AppHost/          # Aspire orchestration host
│   │   └── Program.cs                 # Service orchestration and dependencies
│   ├── AntLogistics.Core/             # Core API service
│   │   ├── Program.cs                 # API endpoints and configuration
│   │   └── Data/
│   │       ├── AntLogisticsDbContext.cs
│   │       └── Models/
│   │           └── Warehouse.cs       # Domain models
│   └── AntLogistics.ServiceDefaults/  # Shared Aspire configurations
│       └── Extensions.cs              # OpenTelemetry, health checks, resilience
├── docs/                              # Documentation
├── Directory.Build.props              # MSBuild properties
├── Directory.Packages.props           # Central package management
└── AntLogistics.slnx                  # Solution file
```

### Project Descriptions

- **AntLogistics.AppHost**: .NET Aspire orchestration project that manages service dependencies, including PostgreSQL database with pgAdmin
- **AntLogistics.Core**: Core API service with RESTful endpoints, Entity Framework Core, and PostgreSQL integration
- **AntLogistics.ServiceDefaults**: Shared configuration for observability (OpenTelemetry), health checks, resilience patterns, and service discovery

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2025 Pavel Ski
