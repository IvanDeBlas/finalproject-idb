using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IAcuerdoCrowdsourcingMilestoneRepository
{
    Task<AcuerdoCrowdsourcingMilestone?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<AcuerdoCrowdsourcingMilestone>> GetByAcuerdoIdAsync(AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct);
    Task<decimal> GetSumaImportesByAcuerdoIdAsync(AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct);
    Task<decimal> GetSumaImportesExcluyendoAsync(AcuerdoCrowdsourcingId acuerdoId, Guid excludeMilestoneId, CancellationToken ct);
    Task<int> GetMaxOrdenAsync(AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct);
    Task<bool> TieneEntregablesAsync(Guid milestoneId, CancellationToken ct);
    Task<Guid> AddAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct);
    Task UpdateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
