using AntLogistics.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddCoreApiClient();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapDefaultEndpoints();
app.MapCoreApiProxy();
app.UseAstroDevServerProxy(args);
app.UseAstroStaticFiles();
app.MapSpaFallback();

app.Run();

