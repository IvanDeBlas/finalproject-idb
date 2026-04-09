using Microsoft.Extensions.DependencyInjection;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Interfaces;
using WePlayRises.UserAccess.Infra.Repositories;
using WePlayRises.UserAccess.Infra.Services;

namespace WePlayRises.UserAccess.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddUserAccessServices(this IServiceCollection services)
    {
        // Register repositories
        // NOTA: UserAccessContext hereda de IdentityDbContext, no de CoreDbContext,
        // por lo que no usa IUnitOfWork generico. Repository hace SaveChanges directamente.
        services.AddScoped<IArtistaRepository, ArtistaRepository>();

        // Register services
        services.AddScoped<IArtistaService, ArtistaService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // PerfilProfesional
        services.AddScoped<IPerfilProfesionalRepository, PerfilProfesionalRepository>();
        services.AddScoped<IPerfilProfesionalService, PerfilProfesionalService>();

        return services;
    }
}
