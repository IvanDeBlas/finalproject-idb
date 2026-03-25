namespace WePlayRises.Crowdfunding.Application.Interfaces.Services;

/// <summary>
/// Cross-module service that checks whether a ProyectoArtistico has
/// crowdsourcing needs or crowdpromotion programs.
/// Uses raw SQL since the data lives in other module schemas but same DB.
/// </summary>
public interface ICrowdFlagsService
{
    /// <summary>
    /// For a set of ProyectoArtisticoIds, returns which ones have crowdsourcing and/or crowdpromotion.
    /// </summary>
    Task<Dictionary<Guid, CrowdFlags>> GetFlagsForProyectosAsync(
        IEnumerable<Guid> proyectoArtisticoIds,
        CancellationToken ct);
}

/// <summary>
/// Holds the crowd flags for a single ProyectoArtistico.
/// </summary>
public record CrowdFlags(bool TieneCrowdsourcing, bool TieneCrowdpromotion);
