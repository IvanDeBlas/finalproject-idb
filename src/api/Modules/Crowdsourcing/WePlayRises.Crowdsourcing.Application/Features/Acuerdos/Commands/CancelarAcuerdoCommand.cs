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
public class CancelarAcuerdoCommand : IRequest<ServiceResponse<CancelarAcuerdoResultDto>>
{
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Motivo { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CancelarAcuerdoCommandHandler : IRequestHandler<CancelarAcuerdoCommand, ServiceResponse<CancelarAcuerdoResultDto>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<CancelarAcuerdoCommand> _validator;
    private readonly ILogger<CancelarAcuerdoCommandHandler> _logger;

    public CancelarAcuerdoCommandHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IArtistaService artistaService,
        IValidator<CancelarAcuerdoCommand> validator,
        ILogger<CancelarAcuerdoCommandHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CancelarAcuerdoResultDto>> Handle(
        CancelarAcuerdoCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<CancelarAcuerdoResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CancelarAcuerdoResultDto>(
                    "Acuerdo no encontrado", ServiceResponseMessageType.NotFound_Acuerdo);
            }

            var esProveedor = acuerdo.UserIdProveedor == request.UserId;
            var esArtista = false;
            if (!esProveedor)
            {
                var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
                esArtista = artista != null && acuerdo.ArtistaId == artista.Id;
            }

            if (!esProveedor && !esArtista)
            {
                _logger.LogWarning("User {UserId} attempted to cancel acuerdo {AcuerdoId} without being a participant",
                    request.UserId, request.AcuerdoId);
                return ValidateExtensions.ForbiddenServiceResponse<CancelarAcuerdoResultDto>(
                    "Solo los participantes del acuerdo pueden cancelarlo", ServiceResponseMessageType.Auth_Forbidden);
            }

            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<CancelarAcuerdoResultDto>(
                    "Solo se pueden cancelar acuerdos activos", ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            await _acuerdoService.CancelarAsync(
                new AcuerdoCrowdsourcingId(request.AcuerdoId),
                request.Motivo,
                request.UserId,
                ct);

            var resultDto = new CancelarAcuerdoResultDto
            {
                Id = request.AcuerdoId,
                EstadoAcuerdoNombre = "Cancelado",
                FechaFinReal = DateTime.UtcNow,
                NecesidadEstadoNombre = "Abierta"
            };

            return new ServiceResponse<CancelarAcuerdoResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Acuerdo cancelado. La necesidad ha sido reabierta."
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling acuerdo {AcuerdoId}", request.AcuerdoId);
            return ValidateExtensions.InternalServerErrorServiceResponse<CancelarAcuerdoResultDto>(
                "Error inesperado al cancelar acuerdo", ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
