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
public class AprobarEntregableCommand : IRequest<ServiceResponse<AprobarEntregableResultDto>>
{
    public Guid EntregableId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Comentario { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class AprobarEntregableCommandHandler : IRequestHandler<AprobarEntregableCommand, ServiceResponse<AprobarEntregableResultDto>>
{
    private readonly IAcuerdoCrowdsourcingEntregableService _entregableService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<AprobarEntregableCommand> _validator;
    private readonly ILogger<AprobarEntregableCommandHandler> _logger;

    public AprobarEntregableCommandHandler(
        IAcuerdoCrowdsourcingEntregableService entregableService,
        IArtistaService artistaService,
        IValidator<AprobarEntregableCommand> validator,
        ILogger<AprobarEntregableCommandHandler> logger)
    {
        _entregableService = entregableService ?? throw new ArgumentNullException(nameof(entregableService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<AprobarEntregableResultDto>> Handle(
        AprobarEntregableCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<AprobarEntregableResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var entregable = await _entregableService.GetByIdWithAcuerdoAsync(request.EntregableId, ct);
            if (entregable == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<AprobarEntregableResultDto>(
                    "Entregable no encontrado", ServiceResponseMessageType.NotFound_Entregable);
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null || entregable.Acuerdo.ArtistaId != artista.Id)
            {
                _logger.LogWarning("User {UserId} attempted to approve entregable {EntregableId} without ownership",
                    request.UserId, request.EntregableId);
                return ValidateExtensions.ForbiddenServiceResponse<AprobarEntregableResultDto>(
                    "Solo el artista del acuerdo puede aprobar entregables", ServiceResponseMessageType.Auth_Forbidden);
            }

            if (entregable.Acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<AprobarEntregableResultDto>(
                    "Solo se pueden aprobar entregables de acuerdos activos", ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            if (entregable.EstadoEntregableId != EstadoEntregableConstants.Entregado)
            {
                return ValidateExtensions.ConflictServiceResponse<AprobarEntregableResultDto>(
                    "Solo se pueden aprobar entregables en estado Entregado", ServiceResponseMessageType.BusinessRule_EntregableNotReviewable);
            }

            await _entregableService.AprobarAsync(request.EntregableId, request.Comentario, ct);

            var todosAprobados = false;
            if (entregable.MilestoneId.HasValue)
            {
                todosAprobados = await _entregableService.TodosAprobadosEnMilestoneAsync(entregable.MilestoneId.Value, ct);
            }

            var resultDto = new AprobarEntregableResultDto
            {
                Id = request.EntregableId,
                EstadoEntregableNombre = "Aprobado",
                FechaAprobacion = DateTime.UtcNow,
                TodosAprobadosEnMilestone = todosAprobados
            };

            return new ServiceResponse<AprobarEntregableResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Entregable aprobado correctamente"
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving entregable {EntregableId}", request.EntregableId);
            return ValidateExtensions.InternalServerErrorServiceResponse<AprobarEntregableResultDto>(
                "Error inesperado al aprobar entregable", ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
