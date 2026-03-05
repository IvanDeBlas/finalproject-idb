using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public interface IValoracionCrowdsourcingRepository
{
    Task<Guid> AddAsync(ValoracionCrowdsourcing entity, CancellationToken ct);

    Task<bool> ExisteValoracionAsync(AcuerdoCrowdsourcingId acuerdoId, string userIdAutor, CancellationToken ct);

    Task<ValoracionResumenData> GetResumenByUserIdAsync(string userId, CancellationToken ct);

    Task<(IReadOnlyList<ValoracionCrowdsourcing> Items, int TotalCount)> GetByUserIdPagedAsync(
        string userId, int page, int pageSize, CancellationToken ct);
}
