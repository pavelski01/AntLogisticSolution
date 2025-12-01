var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add HttpClient for proxying to Core API
builder.Services.AddHttpClient("core", client =>
{
    // This will be configured by Aspire service discovery
    client.BaseAddress = new Uri("https+http://core");
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Proxy API calls to Core service
app.Map("/api/{**path}", async (HttpContext context, IHttpClientFactory httpClientFactory, string path) =>
{
    var coreClient = httpClientFactory.CreateClient("core");
    
    var targetUrl = $"/api/{path}{context.Request.QueryString}";
    var requestMessage = new HttpRequestMessage(
        new HttpMethod(context.Request.Method),
        targetUrl);

    // Copy request headers
    foreach (var header in context.Request.Headers)
    {
        if (!header.Key.StartsWith("Host", StringComparison.OrdinalIgnoreCase))
        {
            requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    // Copy request body for POST/PUT/PATCH
    if (context.Request.ContentLength > 0)
    {
        var streamContent = new StreamContent(context.Request.Body);
        if (context.Request.ContentType != null)
        {
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(context.Request.ContentType);
        }
        requestMessage.Content = streamContent;
    }

    try
    {
        var response = await coreClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        context.Response.StatusCode = (int)response.StatusCode;
        
        // Copy response headers
        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }
        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        await response.Content.CopyToAsync(context.Response.Body);
    }
    catch (HttpRequestException ex)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new { error = "Core API is unavailable", details = ex.Message });
    }
});

// Configure SPA - uses SpaProxy settings from .csproj in development
app.UseStaticFiles();
app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    // In production, serve the built Astro app
    app.UseSpaStaticFiles();
}

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        // The SpaProxy automatically launches and proxies to the dev server
        // based on SpaProxyServerUrl and SpaProxyLaunchCommand in .csproj
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4321");
    }
});

app.Run();
