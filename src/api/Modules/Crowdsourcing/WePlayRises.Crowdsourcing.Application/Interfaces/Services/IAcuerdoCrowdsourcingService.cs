using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IAcuerdoCrowdsourcingService
{
    Task<AcuerdoCrowdsourcing?> GetByIdAsync(AcuerdoCrowdsourcingId id, CancellationToken ct);
    Task<AcuerdoCrowdsourcing?> GetByIdWithDetailsAsync(AcuerdoCrowdsourcingId id, CancellationToken ct);
    Task<bool> ExisteAcuerdoActivoParaNecesidadAsync(NecesidadCrowdsourcingId necesidadId, CancellationToken ct);
    Task<(AcuerdoCrowdsourcingId AcuerdoId, int PropuestasRechazadas, Guid ConversacionId)> AceptarPropuestaAsync(
        AcuerdoCrowdsourcing acuerdo,
        PropuestaCrowdsourcing propuesta,
        NecesidadCrowdsourcing necesidad,
        string userIdArtista,
        CancellationToken ct);
    Task CompletarAsync(AcuerdoCrowdsourcingId id, CancellationToken ct);
    Task CancelarAsync(AcuerdoCrowdsourcingId id, string motivo, string canceladoPorUserId, CancellationToken ct);
}
