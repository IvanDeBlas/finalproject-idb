using Microsoft.Extensions.DependencyInjection;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Interfaces;
using WePlayRises.Crowdpromotion.Infra.Repositories;
using WePlayRises.Crowdpromotion.Infra.Services;

namespace WePlayRises.Crowdpromotion.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddCrowdpromotionServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IPromotorRepository, PromotorRepository>();
        services.AddScoped<IPromotorWalletRepository, PromotorWalletRepository>();
        services.AddScoped<IPromoProgramaRepository, PromoProgramaRepository>();
        services.AddScoped<IPromoProgramaPromotorRepository, PromoProgramaPromotorRepository>();
        services.AddScoped<IPromoTareaRepository, PromoTareaRepository>();
        services.AddScoped<IPromoTareaPromotorRepository, PromoTareaPromotorRepository>();
        services.AddScoped<IPromoEventoRepository, PromoEventoRepository>();
        services.AddScoped<IPromotorWalletTransaccionRepository, PromotorWalletTransaccionRepository>();

        // Services
        services.AddScoped<IPromotorService, PromotorService>();
        services.AddScoped<IPromotorWalletService, PromotorWalletService>();
        services.AddScoped<IPromoProgramaService, PromoProgramaService>();
        services.AddScoped<IInscripcionService, InscripcionService>();
        services.AddScoped<IPromoTareaService, PromoTareaService>();
        services.AddScoped<IPromoEventoService, PromoEventoService>();
        services.AddSingleton<ITrackingRateLimitService, TrackingRateLimitService>();

        return services;
    }
}
