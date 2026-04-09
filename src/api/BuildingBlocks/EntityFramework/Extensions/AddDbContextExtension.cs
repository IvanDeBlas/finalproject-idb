using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace WePlayRises.BuildingBlocks.EntityFramework.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AddDbContextExtension
    {
        public static IServiceCollection AddSqlServerDbContext<TContext>(this IServiceCollection services, string connectionString)
            where TContext : CoreDbContext
        {
            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(connectionString);

#if DEBUG
                // -------------------------------------------------------------------
                // EF CORE QUERY LOGGING - Solo en DEBUG
                // Las queries aparecen en Output > Debug de Visual Studio
                // -------------------------------------------------------------------
                options.LogTo(
                    message => Debug.WriteLine(message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information)
                    .EnableSensitiveDataLogging()  // Muestra valores de parámetros
                    .EnableDetailedErrors();        // Errores más descriptivos
#endif
            });

            return services;
        }

        /// <summary>
        /// Añade un contexto de base de datos para SQL Server con una cadena de conexión obtenida de un proveedor de cadenas de conexión.
        /// Debe inicializarse AddConnectionStringProvider anteriormente.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="connectionStringProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSecretServerDbContext<TContext>(this IServiceCollection services,
                                                                Func<IServiceProvider, Task<string>> connectionStringProvider)
            where TContext : CoreDbContext
        {
            if (connectionStringProvider == null) throw new ArgumentNullException(nameof(connectionStringProvider));

            services.AddDbContext<TContext>((serviceProvider, options) =>
            {
                var connectionString = connectionStringProvider(serviceProvider).GetAwaiter().GetResult();
                options.UseSqlServer(connectionString);

#if DEBUG
                // -------------------------------------------------------------------
                // EF CORE QUERY LOGGING - Solo en DEBUG
                // Las queries aparecen en Output > Debug de Visual Studio
                // -------------------------------------------------------------------
                options.LogTo(
                    message => Debug.WriteLine(message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
#endif
            });

            return services;
        }
    }
}
