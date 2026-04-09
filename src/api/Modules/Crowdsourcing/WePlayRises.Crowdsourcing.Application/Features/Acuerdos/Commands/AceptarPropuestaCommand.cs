using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class AceptarPropuestaCommand : IRequest<ServiceResponse<AceptarPropuestaResultDto>>
{
    public Guid PropuestaId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TituloInterno { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFinPrevista { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class AceptarPropuestaCommandHandler : IRequestHandler<AceptarPropuestaCommand, ServiceResponse<AceptarPropuestaResultDto>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IPropuestaCrowdsourcingService _propuestaService;
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<AceptarPropuestaCommand> _validator;
    private readonly ILogger<AceptarPropuestaCommandHandler> _logger;

    public AceptarPropuestaCommandHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IPropuestaCrowdsourcingService propuestaService,
        INecesidadCrowdsourcingService necesidadService,
        IArtistaService artistaService,
        IValidator<AceptarPropuestaCommand> validator,
        ILogger<AceptarPropuestaCommandHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<AceptarPropuestaResultDto>> Handle(
        AceptarPropuestaCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<AceptarPropuestaResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null)
            {
                return ValidateExtensions.ForbiddenServiceResponse<AceptarPropuestaResultDto>(
                    "No tienes permisos para aceptar propuestas",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            var propuesta = await _propuestaService.GetByIdAsync(new PropuestaCrowdsourcingId(request.PropuestaId), ct);
            if (propuesta == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<AceptarPropuestaResultDto>(
                    "Propuesta no encontrada",
                    ServiceResponseMessageType.NotFound_Propuesta);
            }

            var necesidad = await _necesidadService.GetByIdAsync(propuesta.NecesidadId, ct);
            // NecesidadCrowdsourcingController stores UserId (not Artista entity PK) as ArtistaId
            if (necesidad == null || necesidad.ArtistaId.Value.ToString() != artista.UserIdPropietario)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to accept propuesta {PropuestaId} without ownership",
                    request.UserId, request.PropuestaId);
                return ValidateExtensions.ForbiddenServiceResponse<AceptarPropuestaResultDto>(
                    "No tienes permisos sobre esta necesidad",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            var entity = new AcuerdoCrowdsourcing
            {
                Id = AcuerdoCrowdsourcingId.CreateNew(),
                NecesidadId = propuesta.NecesidadId,
                PropuestaId = propuesta.Id,
                ArtistaId = artista.Id,
                UserIdProveedor = propuesta.UserId,
                PerfilProfesionalId = propuesta.PerfilProfesionalId,
                ImporteTotalPactado = propuesta.PrecioPropuesto,
                MonedaId = propuesta.MonedaId,
                EstadoAcuerdoId = EstadoAcuerdoConstants.Activo,
                TituloInterno = request.TituloInterno,
                FechaInicio = request.FechaInicio,
                FechaFinPrevista = request.FechaFinPrevista,
                FechaCreacion = DateTime.UtcNow
            };

            var (acuerdoId, propuestasRechazadas, conversacionId) =
                await _acuerdoService.AceptarPropuestaAsync(entity, propuesta, necesidad, request.UserId, ct);

            var resultDto = new AceptarPropuestaResultDto
            {
                AcuerdoId = acuerdoId.Value,
                TituloInterno = request.TituloInterno,
                EstadoAcuerdoNombre = "Activo",
                ImporteTotalPactado = propuesta.PrecioPropuesto,
                MonedaNombre = string.Empty,
                ConversacionId = conversacionId,
                PropuestasRechazadas = propuestasRechazadas
            };

            return new ServiceResponse<AceptarPropuestaResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional.",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting propuesta {PropuestaId} by user {UserId}",
                request.PropuestaId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<AceptarPropuestaResultDto>(
                "Error inesperado al aceptar propuesta",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
