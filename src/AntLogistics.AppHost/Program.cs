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
    .AddProject<Projects.AntLogistics_UI>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(core)
    .WaitFor(core);

builder
    .AddNpmApp("astro-dev", "../AntLogistics.UI/ClientApp", "dev")
    .WithReference(core)
    .WaitFor(core)
    .WithHttpEndpoint(targetPort: 4321, isProxied: false)
    .WithExternalHttpEndpoints()
    .WithParentRelationship(ui);

builder.Build().Run();
