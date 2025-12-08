var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume();

var antLogisticsDb = postgres.AddDatabase("antlogistics");

// Add Core API service with database reference
var coreApi = builder.AddProject<Projects.AntLogistics_Core>("core")
    .WithReference(antLogisticsDb)
    .WaitFor(antLogisticsDb);

// Add UI SPA application with reference to Core API
var ui = builder.AddProject<Projects.AntLogistics_UI>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(coreApi)
    .WaitFor(coreApi);

// Add Astro dev server for frontend development
var astroDev = builder.AddNpmApp("astro-dev", "../AntLogistics.UI/ClientApp", "dev")
    .WithReference(coreApi)
    .WaitFor(coreApi)
    .WithHttpEndpoint(targetPort: 4321, isProxied: false)
    .WithExternalHttpEndpoints();

// Example: Add your services here and reference ServiceDefaults
// var apiService = builder.AddProject<Projects.AntLogistics_Api>("apiservice")
//     .WithReference(builder.AddProject<Projects.AntLogistics_ServiceDefaults>("servicedefaults"));

// Example: Add more services as needed
// var webApp = builder.AddProject<Projects.AntLogistics_Web>("webapp")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

builder.Build().Run();
