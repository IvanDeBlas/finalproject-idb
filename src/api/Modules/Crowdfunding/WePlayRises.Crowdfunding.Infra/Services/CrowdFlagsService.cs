using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Services;

/// <summary>
/// Queries NecesidadCrowdsourcing and PromoPrograma tables (other modules, same DB)
/// via raw SQL to determine crowd flags for ProyectoArtistico IDs.
/// </summary>
public class CrowdFlagsService : ICrowdFlagsService
{
    private readonly CrowdfundingContext _context;
    private readonly ILogger<CrowdFlagsService> _logger;

    public CrowdFlagsService(
        CrowdfundingContext context,
        ILogger<CrowdFlagsService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Dictionary<Guid, CrowdFlags>> GetFlagsForProyectosAsync(
        IEnumerable<Guid> proyectoArtisticoIds,
        CancellationToken ct)
    {
        var ids = proyectoArtisticoIds.Distinct().ToList();
        var result = new Dictionary<Guid, CrowdFlags>();

        if (ids.Count == 0)
        {
            return result;
        }

        try
        {
            var crowdsourcingIds = new HashSet<Guid>();
            var crowdpromotionIds = new HashSet<Guid>();

            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync(ct);

            try
            {
                using var command = connection.CreateCommand();

                // Build parameterized IN clause
                var paramNames = new List<string>();
                for (var i = 0; i < ids.Count; i++)
                {
                    var paramName = $"@p{i}";
                    paramNames.Add(paramName);
                    var param = new SqlParameter(paramName, ids[i]);
                    command.Parameters.Add(param);
                }

                var inClause = string.Join(", ", paramNames);

                // Query both tables in a single roundtrip
                command.CommandText = $@"
                    SELECT DISTINCT [ProyectoArtistico_Id], 'CS' AS Tipo
                    FROM [NecesidadCrowdsourcing]
                    WHERE [ProyectoArtistico_Id] IN ({inClause})
                    UNION ALL
                    SELECT DISTINCT [ProyectoArtistico_Id], 'CP' AS Tipo
                    FROM [PromoPrograma]
                    WHERE [ProyectoArtistico_Id] IN ({inClause})
                      AND [EsActivo] = 1";

                using var reader = await command.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    var proyectoId = reader.GetGuid(0);
                    var tipo = reader.GetString(1);

                    if (tipo == "CS")
                    {
                        crowdsourcingIds.Add(proyectoId);
                    }
                    else
                    {
                        crowdpromotionIds.Add(proyectoId);
                    }
                }
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            foreach (var id in ids)
            {
                result[id] = new CrowdFlags(
                    crowdsourcingIds.Contains(id),
                    crowdpromotionIds.Contains(id));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error querying crowd flags for {Count} ProyectoArtistico IDs. Returning empty flags.", ids.Count);

            // Return empty flags on error (non-critical feature)
            foreach (var id in ids)
            {
                result[id] = new CrowdFlags(false, false);
            }
        }

        return result;
    }
}
