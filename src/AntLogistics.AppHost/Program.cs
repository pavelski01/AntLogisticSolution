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

// Add Astro dev server as npm app (automatically starts and manages the dev server)
var astroApp = builder.AddNpmApp("astro-dev", "../AntLogistics.UI/ClientApp", "dev")
    .WithHttpEndpoint(port: 4321, env: "PORT")
    .PublishAsDockerFile();

// Add UI SPA application
var ui = builder.AddProject<Projects.AntLogistics_UI>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(coreApi)
    .WithReference(astroApp)
    .WaitFor(astroApp);

// Example: Add your services here and reference ServiceDefaults
// var apiService = builder.AddProject<Projects.AntLogistics_Api>("apiservice")
//     .WithReference(builder.AddProject<Projects.AntLogistics_ServiceDefaults>("servicedefaults"));

// Example: Add more services as needed
// var webApp = builder.AddProject<Projects.AntLogistics_Web>("webapp")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

builder.Build().Run();
