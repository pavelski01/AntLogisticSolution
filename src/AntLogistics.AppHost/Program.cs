var builder = DistributedApplication.CreateBuilder(args);

var database = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume()
    .AddDatabase("antlogistics");

var coreApi = builder
    .AddProject<Projects.AntLogistics_Core>("core")
    .WithReference(database)
    .WaitFor(database);

builder
    .AddProject<Projects.AntLogistics_UI>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(coreApi)
    .WaitFor(coreApi);

builder
    .AddNpmApp("astro-dev", "../AntLogistics.UI/ClientApp", "dev")
    .WithReference(coreApi)
    .WaitFor(coreApi)
    .WithHttpEndpoint(targetPort: 4321, isProxied: false)
    .WithExternalHttpEndpoints();

builder.Build().Run();
