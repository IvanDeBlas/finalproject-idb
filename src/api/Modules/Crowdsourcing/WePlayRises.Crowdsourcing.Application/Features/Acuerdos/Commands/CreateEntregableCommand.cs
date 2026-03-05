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

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateEntregableCommand : IRequest<ServiceResponse<EntregableCreatedResultDto>>
{
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? UrlRecurso { get; set; }
    public Guid? MilestoneId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateEntregableCommandHandler : IRequestHandler<CreateEntregableCommand, ServiceResponse<EntregableCreatedResultDto>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IAcuerdoCrowdsourcingMilestoneService _milestoneService;
    private readonly IAcuerdoCrowdsourcingEntregableService _entregableService;
    private readonly IValidator<CreateEntregableCommand> _validator;
    private readonly ILogger<CreateEntregableCommandHandler> _logger;

    public CreateEntregableCommandHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IAcuerdoCrowdsourcingMilestoneService milestoneService,
        IAcuerdoCrowdsourcingEntregableService entregableService,
        IValidator<CreateEntregableCommand> validator,
        ILogger<CreateEntregableCommandHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _milestoneService = milestoneService ?? throw new ArgumentNullException(nameof(milestoneService));
        _entregableService = entregableService ?? throw new ArgumentNullException(nameof(entregableService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<EntregableCreatedResultDto>> Handle(
        CreateEntregableCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<EntregableCreatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<EntregableCreatedResultDto>(
                    "Acuerdo no encontrado", ServiceResponseMessageType.NotFound_Acuerdo);
            }

            if (acuerdo.UserIdProveedor != request.UserId)
            {
                _logger.LogWarning("User {UserId} attempted to create entregable for acuerdo {AcuerdoId} without being the provider",
                    request.UserId, request.AcuerdoId);
                return ValidateExtensions.ForbiddenServiceResponse<EntregableCreatedResultDto>(
                    "Solo el profesional del acuerdo puede subir entregables", ServiceResponseMessageType.Auth_Forbidden);
            }

            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<EntregableCreatedResultDto>(
                    "Solo se pueden subir entregables a acuerdos activos", ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            if (request.MilestoneId.HasValue)
            {
                var milestone = await _milestoneService.GetByIdAsync(request.MilestoneId.Value, ct);
                if (milestone == null || milestone.AcuerdoId != acuerdo.Id)
                {
                    return ValidateExtensions.BadRequestServiceResponse<EntregableCreatedResultDto>(
                        "El milestone no pertenece a este acuerdo", ServiceResponseMessageType.Validation_ForeignKeyNotFound);
                }
            }

            var entity = new AcuerdoCrowdsourcingEntregable
            {
                Id = Guid.NewGuid(),
                AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId),
                MilestoneId = request.MilestoneId,
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                UrlRecurso = request.UrlRecurso,
                EstadoEntregableId = EstadoEntregableConstants.Entregado
            };

            var id = await _entregableService.CreateAsync(entity, ct);

            var resultDto = new EntregableCreatedResultDto
            {
                Id = id,
                Titulo = request.Titulo,
                EstadoEntregableNombre = "Entregado",
                FechaCreacion = entity.FechaCreacion
            };

            return new ServiceResponse<EntregableCreatedResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Entregable subido correctamente. El artista sera notificado.",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entregable for acuerdo {AcuerdoId}", request.AcuerdoId);
            return ValidateExtensions.InternalServerErrorServiceResponse<EntregableCreatedResultDto>(
                "Error inesperado al subir entregable", ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
