using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class AcuerdoCrowdsourcingService : IAcuerdoCrowdsourcingService
{
    private readonly IAcuerdoCrowdsourcingRepository _repository;
    private readonly IPropuestaCrowdsourcingRepository _propuestaRepository;
    private readonly INecesidadCrowdsourcingRepository _necesidadRepository;
    private readonly IConversacionCrowdsourcingRepository _conversacionRepository;
    private readonly IRequestCacheService _requestCache;
    private readonly CrowdsourcingContext _context;
    private readonly ILogger<AcuerdoCrowdsourcingService> _logger;

    public AcuerdoCrowdsourcingService(
        IAcuerdoCrowdsourcingRepository repository,
        IPropuestaCrowdsourcingRepository propuestaRepository,
        INecesidadCrowdsourcingRepository necesidadRepository,
        IConversacionCrowdsourcingRepository conversacionRepository,
        IRequestCacheService requestCache,
        CrowdsourcingContext context,
        ILogger<AcuerdoCrowdsourcingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _propuestaRepository = propuestaRepository ?? throw new ArgumentNullException(nameof(propuestaRepository));
        _necesidadRepository = necesidadRepository ?? throw new ArgumentNullException(nameof(necesidadRepository));
        _conversacionRepository = conversacionRepository ?? throw new ArgumentNullException(nameof(conversacionRepository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AcuerdoCrowdsourcing?> GetByIdAsync(AcuerdoCrowdsourcingId id, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"acuerdo:{id.Value}",
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<AcuerdoCrowdsourcing?> GetByIdWithDetailsAsync(AcuerdoCrowdsourcingId id, CancellationToken ct)
    {
        return await _repository.GetByIdWithDetailsAsync(id, ct);
    }

    public async Task<bool> ExisteAcuerdoActivoParaNecesidadAsync(NecesidadCrowdsourcingId necesidadId, CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"acuerdo:activo:{necesidadId.Value}",
            async () => await _repository.ExisteAcuerdoActivoParaNecesidadAsync(necesidadId, ct));
    }

    public async Task<(AcuerdoCrowdsourcingId AcuerdoId, int PropuestasRechazadas, Guid ConversacionId)> AceptarPropuestaAsync(
        AcuerdoCrowdsourcing acuerdo,
        PropuestaCrowdsourcing propuesta,
        NecesidadCrowdsourcing necesidad,
        string userIdArtista,
        CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            // 1. Create acuerdo
            var acuerdoId = await _repository.AddAsync(acuerdo, ct);

            // 2. Update accepted propuesta
            propuesta.EstadoPropuestaId = EstadoPropuestaConstants.Aceptada;
            propuesta.AcuerdoId = acuerdoId.Value;
            propuesta.FechaActualizacion = DateTime.UtcNow;
            await _propuestaRepository.UpdateAsync(propuesta, ct);

            // 3. Reject other pending propuestas
            var propuestasPendientes = await _propuestaRepository.GetPendientesByNecesidadIdAsync(necesidad.Id, ct);
            var rechazadas = 0;
            if (propuestasPendientes.Count > 0)
            {
                foreach (var p in propuestasPendientes)
                {
                    p.EstadoPropuestaId = EstadoPropuestaConstants.Rechazada;
                    p.FechaActualizacion = DateTime.UtcNow;
                }

                await _propuestaRepository.UpdateManyAsync(propuestasPendientes, ct);
                rechazadas = propuestasPendientes.Count;
            }

            // 4. Update necesidad to EnProgreso
            necesidad.EstadoNecesidadId = EstadoNecesidadConstants.EnProgreso;
            necesidad.FechaActualizacion = DateTime.UtcNow;
            await _necesidadRepository.UpdateAsync(necesidad, ct);

            // 5. Create conversacion linked to acuerdo
            var conversacion = new ConversacionCrowdsourcing
            {
                AcuerdoId = acuerdoId,
                NecesidadId = necesidad.Id,
                UserIdCreador = userIdArtista,
                UserIdDestinatario = acuerdo.UserIdProveedor,
                Asunto = $"Acuerdo: {acuerdo.TituloInterno ?? necesidad.Titulo}",
                FechaCreacion = DateTime.UtcNow
            };
            var conversacionId = await _conversacionRepository.AddAsync(conversacion, ct);

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Accepted propuesta {PropuestaId} creating acuerdo {AcuerdoId} with {RechazadasCount} proposals rejected and conversacion {ConversacionId}",
                propuesta.Id.Value,
                acuerdoId.Value,
                rechazadas,
                conversacionId);

            return (acuerdoId, rechazadas, conversacionId);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task CompletarAsync(AcuerdoCrowdsourcingId id, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            var acuerdo = await _repository.GetByIdAsync(id, ct);
            if (acuerdo == null)
            {
                throw new InvalidOperationException($"Acuerdo {id.Value} not found");
            }

            acuerdo.EstadoAcuerdoId = EstadoAcuerdoConstants.Completado;
            acuerdo.FechaFinReal = DateTime.UtcNow;
            acuerdo.FechaActualizacion = DateTime.UtcNow;
            await _repository.UpdateAsync(acuerdo, ct);

            var necesidad = await _necesidadRepository.GetByIdAsync(acuerdo.NecesidadId, ct);
            if (necesidad != null)
            {
                necesidad.EstadoNecesidadId = EstadoNecesidadConstants.Cerrada;
                necesidad.FechaActualizacion = DateTime.UtcNow;
                await _necesidadRepository.UpdateAsync(necesidad, ct);
            }

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Completed acuerdo {AcuerdoId}",
                id.Value);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task CancelarAsync(AcuerdoCrowdsourcingId id, string motivo, string canceladoPorUserId, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            var acuerdo = await _repository.GetByIdAsync(id, ct);
            if (acuerdo == null)
            {
                throw new InvalidOperationException($"Acuerdo {id.Value} not found");
            }

            acuerdo.EstadoAcuerdoId = EstadoAcuerdoConstants.Cancelado;
            acuerdo.FechaFinReal = DateTime.UtcNow;
            acuerdo.MotivoCancelacion = motivo;
            acuerdo.CanceladoPor = canceladoPorUserId;
            acuerdo.FechaActualizacion = DateTime.UtcNow;
            await _repository.UpdateAsync(acuerdo, ct);

            var necesidad = await _necesidadRepository.GetByIdAsync(acuerdo.NecesidadId, ct);
            if (necesidad != null)
            {
                necesidad.EstadoNecesidadId = EstadoNecesidadConstants.Abierta;
                necesidad.FechaActualizacion = DateTime.UtcNow;
                await _necesidadRepository.UpdateAsync(necesidad, ct);
            }

            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Cancelled acuerdo {AcuerdoId} by user {UserId}",
                id.Value,
                canceladoPorUserId);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
