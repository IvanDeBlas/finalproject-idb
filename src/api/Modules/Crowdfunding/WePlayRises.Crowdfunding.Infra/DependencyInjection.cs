using Microsoft.Extensions.DependencyInjection;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Infra.Repositories;
using WePlayRises.Crowdfunding.Infra.Services;

namespace WePlayRises.Crowdfunding.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddCrowdfundingServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ICampaniaRepository, CampaniaRepository>();
        services.AddScoped<IRewardRepository, RewardRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IAportacionRepository, AportacionRepository>();

        // Register services
        services.AddScoped<ICampaniaService, CampaniaService>();
        services.AddScoped<IRewardService, RewardService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IBackingService, BackingService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICrowdFlagsService, CrowdFlagsService>();

        return services;
    }
}
