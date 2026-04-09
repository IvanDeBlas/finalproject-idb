using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IValoracionCrowdsourcingService
{
    Task<Guid> CreateAsync(ValoracionCrowdsourcing entity, CancellationToken ct);

    Task<bool> ExisteValoracionAsync(Guid acuerdoId, string userIdAutor, CancellationToken ct);

    Task<bool> UsuarioExisteAsync(string userId, CancellationToken ct);

    Task<ValoracionResumenDto> GetResumenByUserIdAsync(string userId, CancellationToken ct);

    Task<PaginatedResponse<ValoracionListItemDto>> GetByUserIdPagedAsync(
        string userId, int page, int pageSize, CancellationToken ct);
}
