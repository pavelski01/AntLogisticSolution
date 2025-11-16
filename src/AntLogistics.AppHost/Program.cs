var builder = DistributedApplication.CreateBuilder(args);

// Add Core API service
var coreApi = builder.AddProject<Projects.AntLogistics_Core>("core");

// Example: Add your services here and reference ServiceDefaults
// var apiService = builder.AddProject<Projects.AntLogistics_Api>("apiservice")
//     .WithReference(builder.AddProject<Projects.AntLogistics_ServiceDefaults>("servicedefaults"));

// Example: Add more services as needed
// var webApp = builder.AddProject<Projects.AntLogistics_Web>("webapp")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

builder.Build().Run();
