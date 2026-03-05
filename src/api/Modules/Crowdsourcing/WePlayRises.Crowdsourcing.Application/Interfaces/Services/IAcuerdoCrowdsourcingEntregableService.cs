using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IAcuerdoCrowdsourcingEntregableService
{
    Task<AcuerdoCrowdsourcingEntregable?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AcuerdoCrowdsourcingEntregable?> GetByIdWithAcuerdoAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateAsync(AcuerdoCrowdsourcingEntregable entity, CancellationToken ct);
    Task<bool> AprobarAsync(Guid id, string? comentario, CancellationToken ct);
    Task<bool> RechazarAsync(Guid id, string comentario, CancellationToken ct);
    Task<bool> TodosAprobadosEnMilestoneAsync(Guid milestoneId, CancellationToken ct);
}
