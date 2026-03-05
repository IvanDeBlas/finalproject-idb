using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class RechazarPropuestaCommand : IRequest<ServiceResponse<RechazarPropuestaResultDto>>
{
    public Guid PropuestaId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Motivo { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class RechazarPropuestaCommandHandler : IRequestHandler<RechazarPropuestaCommand, ServiceResponse<RechazarPropuestaResultDto>>
{
    private readonly IPropuestaCrowdsourcingService _propuestaService;
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<RechazarPropuestaCommand> _validator;
    private readonly ILogger<RechazarPropuestaCommandHandler> _logger;

    public RechazarPropuestaCommandHandler(
        IPropuestaCrowdsourcingService propuestaService,
        INecesidadCrowdsourcingService necesidadService,
        IArtistaService artistaService,
        IValidator<RechazarPropuestaCommand> validator,
        ILogger<RechazarPropuestaCommandHandler> logger)
    {
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RechazarPropuestaResultDto>> Handle(
        RechazarPropuestaCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<RechazarPropuestaResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var propuesta = await _propuestaService.GetByIdAsync(new PropuestaCrowdsourcingId(request.PropuestaId), ct);
            if (propuesta == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<RechazarPropuestaResultDto>(
                    "Propuesta no encontrada",
                    ServiceResponseMessageType.NotFound_Propuesta);
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            var necesidad = await _necesidadService.GetByIdAsync(propuesta.NecesidadId, ct);
            // NecesidadCrowdsourcingController stores UserId (not Artista entity PK) as ArtistaId
            if (necesidad == null || artista == null || necesidad.ArtistaId.Value.ToString() != artista.UserIdPropietario)
            {
                _logger.LogWarning("User {UserId} attempted to reject propuesta {PropuestaId} without ownership",
                    request.UserId, request.PropuestaId);
                return ValidateExtensions.ForbiddenServiceResponse<RechazarPropuestaResultDto>(
                    "No tienes permisos sobre esta necesidad",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            if (propuesta.EstadoPropuestaId != EstadoPropuestaConstants.Pendiente)
            {
                return ValidateExtensions.ConflictServiceResponse<RechazarPropuestaResultDto>(
                    "Solo se pueden rechazar propuestas en estado Pendiente",
                    ServiceResponseMessageType.BusinessRule_PropuestaNotAcceptable);
            }

            await _propuestaService.RechazarAsync(new PropuestaCrowdsourcingId(request.PropuestaId), request.Motivo, ct);

            var resultDto = new RechazarPropuestaResultDto
            {
                Id = request.PropuestaId,
                EstadoPropuestaNombre = "Rechazada"
            };

            return new ServiceResponse<RechazarPropuestaResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Propuesta rechazada"
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting propuesta {PropuestaId} by user {UserId}",
                request.PropuestaId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<RechazarPropuestaResultDto>(
                "Error inesperado al rechazar propuesta",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
