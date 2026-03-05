using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IAcuerdoCrowdsourcingEntregableRepository
{
    Task<AcuerdoCrowdsourcingEntregable?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AcuerdoCrowdsourcingEntregable?> GetByIdWithAcuerdoAsync(Guid id, CancellationToken ct);
    Task<bool> TodosAprobadosEnMilestoneAsync(Guid milestoneId, CancellationToken ct);
    Task<Guid> AddAsync(AcuerdoCrowdsourcingEntregable entity, CancellationToken ct);
    Task UpdateAsync(AcuerdoCrowdsourcingEntregable entity, CancellationToken ct);
}
