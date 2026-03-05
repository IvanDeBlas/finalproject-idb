using Microsoft.Extensions.DependencyInjection;

namespace WePlayRises.Core.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register Core module services here
        // Maestra repositories and services will be registered here

        return services;
    }
}
