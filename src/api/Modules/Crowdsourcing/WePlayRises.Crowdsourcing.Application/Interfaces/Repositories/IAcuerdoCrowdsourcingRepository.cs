using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IAcuerdoCrowdsourcingRepository
{
    Task<AcuerdoCrowdsourcing?> GetByIdAsync(AcuerdoCrowdsourcingId id, CancellationToken ct);
    Task<AcuerdoCrowdsourcing?> GetByIdWithDetailsAsync(AcuerdoCrowdsourcingId id, CancellationToken ct);
    Task<AcuerdoCrowdsourcingId> AddAsync(AcuerdoCrowdsourcing entity, CancellationToken ct);
    Task UpdateAsync(AcuerdoCrowdsourcing entity, CancellationToken ct);
    Task<bool> ExisteAcuerdoActivoParaNecesidadAsync(NecesidadCrowdsourcingId necesidadId, CancellationToken ct);
}
