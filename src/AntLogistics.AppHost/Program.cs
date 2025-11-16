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

// Example: Add your services here and reference ServiceDefaults
// var apiService = builder.AddProject<Projects.AntLogistics_Api>("apiservice")
//     .WithReference(builder.AddProject<Projects.AntLogistics_ServiceDefaults>("servicedefaults"));

// Example: Add more services as needed
// var webApp = builder.AddProject<Projects.AntLogistics_Web>("webapp")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

builder.Build().Run();
