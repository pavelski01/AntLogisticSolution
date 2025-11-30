var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseRouting();

app.MapDefaultEndpoints();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
    
    // In development, you can run the Vite dev server separately with:
    // cd ClientApp && npm run dev
    // Then uncomment the line below to proxy to it:
    // if (app.Environment.IsDevelopment())
    // {
    //     spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
    // }
});

app.Run();
