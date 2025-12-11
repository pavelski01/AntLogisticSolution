var builder = DistributedApplication.CreateBuilder(args);

var database = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume()
    .AddDatabase("antlogistics");

var core = builder
    .AddProject<Projects.AntLogistics_Core>("core")
    .WithReference(database)
    .WaitFor(database);

var ui = builder
    .AddNpmApp("ui", "../AntLogistics.UI", "dev")
    .WithReference(core)
    .WaitFor(core)
    .WithHttpEndpoint(targetPort: 4321, isProxied: false)
    .WithExternalHttpEndpoints();

builder.Build().Run();
