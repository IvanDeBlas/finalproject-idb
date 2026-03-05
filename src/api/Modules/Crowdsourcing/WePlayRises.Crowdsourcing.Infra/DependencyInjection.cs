using Microsoft.Extensions.DependencyInjection;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Infra.Repositories;
using WePlayRises.Crowdsourcing.Infra.Services;

namespace WePlayRises.Crowdsourcing.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddCrowdsourcingServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IPlantillaProyectoRepository, PlantillaProyectoRepository>();
        services.AddScoped<IPlantillaProyectoNecesidadRepository, PlantillaProyectoNecesidadRepository>();
        services.AddScoped<IMaestraRolProfesionalRepository, MaestraRolProfesionalRepository>();
        services.AddScoped<IMaestraCategoriaRolRepository, MaestraCategoriaRolRepository>();
        services.AddScoped<INecesidadCrowdsourcingRepository, NecesidadCrowdsourcingRepository>();
        services.AddScoped<IPropuestaCrowdsourcingRepository, PropuestaCrowdsourcingRepository>();
        services.AddScoped<IAcuerdoCrowdsourcingRepository, AcuerdoCrowdsourcingRepository>();
        services.AddScoped<IAcuerdoCrowdsourcingMilestoneRepository, AcuerdoCrowdsourcingMilestoneRepository>();
        services.AddScoped<IAcuerdoCrowdsourcingEntregableRepository, AcuerdoCrowdsourcingEntregableRepository>();
        services.AddScoped<IConversacionCrowdsourcingRepository, ConversacionCrowdsourcingRepository>();
        services.AddScoped<IMensajeCrowdsourcingRepository, MensajeCrowdsourcingRepository>();
        services.AddScoped<IValoracionCrowdsourcingRepository, ValoracionCrowdsourcingRepository>();

        // Services
        services.AddScoped<IPlantillaProyectoService, PlantillaProyectoService>();
        services.AddScoped<IRolProfesionalService, RolProfesionalService>();
        services.AddScoped<ICategoriaRolService, CategoriaRolService>();
        services.AddScoped<INecesidadCrowdsourcingService, NecesidadCrowdsourcingService>();
        services.AddScoped<IPropuestaCrowdsourcingService, PropuestaCrowdsourcingService>();
        services.AddScoped<IAcuerdoCrowdsourcingService, AcuerdoCrowdsourcingService>();
        services.AddScoped<IAcuerdoCrowdsourcingMilestoneService, AcuerdoCrowdsourcingMilestoneService>();
        services.AddScoped<IAcuerdoCrowdsourcingEntregableService, AcuerdoCrowdsourcingEntregableService>();
        services.AddScoped<IConversacionCrowdsourcingService, ConversacionCrowdsourcingService>();
        services.AddScoped<IMensajeCrowdsourcingService, MensajeCrowdsourcingService>();
        services.AddScoped<IValoracionCrowdsourcingService, ValoracionCrowdsourcingService>();

        return services;
    }
}
