using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IAcuerdoCrowdsourcingMilestoneService
{
    Task<AcuerdoCrowdsourcingMilestone?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<decimal> GetSumaImportesAsync(AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct);
    Task<decimal> GetSumaImportesExcluyendoAsync(AcuerdoCrowdsourcingId acuerdoId, Guid excludeId, CancellationToken ct);
    Task<Guid> CreateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct);
    Task<bool> UpdateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> TieneEntregablesAsync(Guid milestoneId, CancellationToken ct);
}
