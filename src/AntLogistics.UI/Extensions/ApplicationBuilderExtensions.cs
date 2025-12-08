using Microsoft.Extensions.FileProviders;

namespace AntLogistics.UI.Extensions;

public static class ApplicationBuilderExtensions
{
    private const string AstroDevServerArg = "--use-astro-dev";
    private const string AstroDevServerUrl = "http://localhost:4321";
    private const string ClientAppFolder = "ClientApp";
    private const string DistFolder = "dist";
    private const string IndexFile = "index.html";
    private const string HtmlContentType = "text/html";

    private static readonly string[] ExcludedPathPrefixes = ["/api", "/health", "/alive"];

    public static IApplicationBuilder UseAstroDevServerProxy(this WebApplication app, string[] args)
    {
        var shouldUseDevServer = app.Environment.IsDevelopment() && args.Contains(AstroDevServerArg);

        if (!shouldUseDevServer)
        {
            return app;
        }

        app.Use(async (context, next) =>
        {
            if (IsExcludedPath(context.Request.Path))
            {
                await next();
                return;
            }

            var isProxied = await TryProxyToDevServerAsync(context);
            if (!isProxied)
            {
                await next();
            }
        });

        return app;
    }

    public static IApplicationBuilder UseAstroStaticFiles(this WebApplication app)
    {
        var clientDistPath = GetClientDistPath(app.Environment);

        if (!Directory.Exists(clientDistPath))
        {
            return app;
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(clientDistPath),
            RequestPath = string.Empty
        });

        return app;
    }

    public static IEndpointRouteBuilder MapSpaFallback(this WebApplication app)
    {
        app.MapFallback(async context =>
        {
            var indexPath = GetIndexPath(app.Environment);

            if (File.Exists(indexPath))
            {
                context.Response.ContentType = HtmlContentType;
                await context.Response.SendFileAsync(indexPath);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync(
                    "UI is not built. Run 'npm run build' in ClientApp directory.");
            }
        });

        return app;
    }

    private static bool IsExcludedPath(PathString path)
    {
        return ExcludedPathPrefixes.Any(prefix => path.StartsWithSegments(prefix));
    }

    private static async Task<bool> TryProxyToDevServerAsync(HttpContext context)
    {
        var targetUri = $"{AstroDevServerUrl}{context.Request.Path}{context.Request.QueryString}";

        try
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(targetUri);

            context.Response.StatusCode = (int)response.StatusCode;
            await response.Content.CopyToAsync(context.Response.Body);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string GetClientDistPath(IWebHostEnvironment environment)
    {
        return Path.Combine(environment.ContentRootPath, ClientAppFolder, DistFolder);
    }

    private static string GetIndexPath(IWebHostEnvironment environment)
    {
        return Path.Combine(GetClientDistPath(environment), IndexFile);
    }
}
