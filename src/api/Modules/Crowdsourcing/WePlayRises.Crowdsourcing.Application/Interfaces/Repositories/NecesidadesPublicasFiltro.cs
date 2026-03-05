namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public record NecesidadesPublicasFiltro(
    int[]? TiposNecesidadIds,
    int? ModalidadTrabajoId,
    decimal? PresupuestoMin,
    decimal? PresupuestoMax,
    string? Pais,
    string? Search,
    string OrderBy,
    int Page,
    int PageSize
);
