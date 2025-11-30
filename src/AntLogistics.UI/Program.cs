var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add HttpClient for proxying to Core API
builder.Services.AddHttpClient("core", client =>
{
    // This will be configured by Aspire service discovery
    client.BaseAddress = new Uri("https+http://core");
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

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

// Check if we should use the Astro dev server
var useDevServer = app.Environment.IsDevelopment() 
    && args.Contains("--use-astro-dev");

if (useDevServer)
{
    // Proxy to Astro dev server (only when explicitly requested)
    app.Use(async (context, next) =>
    {
        if (!context.Request.Path.StartsWithSegments("/api") && 
            !context.Request.Path.StartsWithSegments("/health") &&
            !context.Request.Path.StartsWithSegments("/alive"))
        {
            var httpClient = new HttpClient();
            var targetUri = $"http://localhost:4321{context.Request.Path}{context.Request.QueryString}";
            
            try
            {
                var response = await httpClient.GetAsync(targetUri);
                context.Response.StatusCode = (int)response.StatusCode;
                await response.Content.CopyToAsync(context.Response.Body);
                return;
            }
            catch
            {
                // Fall through to next middleware if dev server is not available
            }
        }
        
        await next();
    });
}

// Serve Astro built files
var clientPath = Path.Combine(app.Environment.ContentRootPath, "ClientApp", "dist");
if (Directory.Exists(clientPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(clientPath),
        RequestPath = ""
    });
}

// Fallback for SPA routing
app.MapFallback(async context =>
{
    var indexPath = Path.Combine(app.Environment.ContentRootPath, "ClientApp", "dist", "index.html");
    
    if (File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    }
    else
    {
        context.Response.StatusCode = 503;
        await context.Response.WriteAsync("UI is not built. Run 'npm run build' in ClientApp directory.");
    }
});

app.Run();
