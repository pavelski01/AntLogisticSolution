namespace AntLogistics.UI.Extensions;

public static class ServiceCollectionExtensions
{
    private const string CoreClientName = "core";
    private const string CoreServiceUri = "https+http://core";

    public static IServiceCollection AddCoreApiClient(this IServiceCollection services)
    {
        services.AddHttpClient(CoreClientName, client =>
        {
            client.BaseAddress = new Uri(CoreServiceUri);
        });

        return services;
    }
}
