using System.Net.Http.Headers;

namespace AntLogistics.UI.Extensions;

public static class EndpointRouteBuilderExtensions
{
    private const string CoreClientName = "core";
    private const string ApiRoutePattern = "/api/{**path}";
    private const string HostHeaderPrefix = "Host";

    public static IEndpointRouteBuilder MapCoreApiProxy(this IEndpointRouteBuilder endpoints)
    {
        endpoints.Map(ApiRoutePattern, ProxyToCoreApiAsync);
        return endpoints;
    }

    private static async Task ProxyToCoreApiAsync(
        HttpContext context,
        IHttpClientFactory httpClientFactory,
        string path)
    {
        var coreClient = httpClientFactory.CreateClient(CoreClientName);
        var targetUrl = $"/api/{path}{context.Request.QueryString}";

        using var requestMessage = CreateProxyRequest(context, targetUrl);

        try
        {
            using var response = await coreClient.SendAsync(
                requestMessage,
                HttpCompletionOption.ResponseHeadersRead);

            await CopyResponseAsync(context, response);
        }
        catch (HttpRequestException ex)
        {
            await WriteServiceUnavailableResponseAsync(context, ex);
        }
    }

    private static HttpRequestMessage CreateProxyRequest(HttpContext context, string targetUrl)
    {
        var requestMessage = new HttpRequestMessage(
            new HttpMethod(context.Request.Method),
            targetUrl);

        CopyRequestHeaders(context.Request, requestMessage);
        CopyRequestBody(context.Request, requestMessage);

        return requestMessage;
    }

    private static void CopyRequestHeaders(HttpRequest request, HttpRequestMessage requestMessage)
    {
        foreach (var header in request.Headers)
        {
            var isHostHeader = header.Key.StartsWith(HostHeaderPrefix, StringComparison.OrdinalIgnoreCase);
            if (!isHostHeader)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
    }

    private static void CopyRequestBody(HttpRequest request, HttpRequestMessage requestMessage)
    {
        if (request.ContentLength is not > 0)
        {
            return;
        }

        var streamContent = new StreamContent(request.Body);

        if (request.ContentType is not null)
        {
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
        }

        requestMessage.Content = streamContent;
    }

    private static async Task CopyResponseAsync(HttpContext context, HttpResponseMessage response)
    {
        context.Response.StatusCode = (int)response.StatusCode;

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

    private static async Task WriteServiceUnavailableResponseAsync(HttpContext context, HttpRequestException ex)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Core API is unavailable",
            details = ex.Message
        });
    }
}
