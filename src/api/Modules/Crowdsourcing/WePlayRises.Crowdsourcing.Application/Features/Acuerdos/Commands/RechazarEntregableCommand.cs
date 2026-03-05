using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
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
public class RechazarEntregableCommand : IRequest<ServiceResponse<RechazarEntregableResultDto>>
{
    public Guid EntregableId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Comentario { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class RechazarEntregableCommandHandler : IRequestHandler<RechazarEntregableCommand, ServiceResponse<RechazarEntregableResultDto>>
{
    private readonly IAcuerdoCrowdsourcingEntregableService _entregableService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<RechazarEntregableCommand> _validator;
    private readonly ILogger<RechazarEntregableCommandHandler> _logger;

    public RechazarEntregableCommandHandler(
        IAcuerdoCrowdsourcingEntregableService entregableService,
        IArtistaService artistaService,
        IValidator<RechazarEntregableCommand> validator,
        ILogger<RechazarEntregableCommandHandler> logger)
    {
        _entregableService = entregableService ?? throw new ArgumentNullException(nameof(entregableService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RechazarEntregableResultDto>> Handle(
        RechazarEntregableCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<RechazarEntregableResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var entregable = await _entregableService.GetByIdWithAcuerdoAsync(request.EntregableId, ct);
            if (entregable == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<RechazarEntregableResultDto>(
                    "Entregable no encontrado", ServiceResponseMessageType.NotFound_Entregable);
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null || entregable.Acuerdo.ArtistaId != artista.Id)
            {
                return ValidateExtensions.ForbiddenServiceResponse<RechazarEntregableResultDto>(
                    "Solo el artista del acuerdo puede rechazar entregables", ServiceResponseMessageType.Auth_Forbidden);
            }

            if (entregable.Acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<RechazarEntregableResultDto>(
                    "Solo se pueden rechazar entregables de acuerdos activos", ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            if (entregable.EstadoEntregableId != EstadoEntregableConstants.Entregado)
            {
                return ValidateExtensions.ConflictServiceResponse<RechazarEntregableResultDto>(
                    "Solo se pueden rechazar entregables en estado Entregado", ServiceResponseMessageType.BusinessRule_EntregableNotReviewable);
            }

            await _entregableService.RechazarAsync(request.EntregableId, request.Comentario, ct);

            var resultDto = new RechazarEntregableResultDto
            {
                Id = request.EntregableId,
                EstadoEntregableNombre = "Rechazado"
            };

            return new ServiceResponse<RechazarEntregableResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Entregable rechazado. El profesional sera notificado."
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting entregable {EntregableId}", request.EntregableId);
            return ValidateExtensions.InternalServerErrorServiceResponse<RechazarEntregableResultDto>(
                "Error inesperado al rechazar entregable", ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
