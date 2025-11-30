var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist/client";
});

var app = builder.Build();

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.MapDefaultEndpoints();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
    
    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4321");
    }
});

app.Run();
