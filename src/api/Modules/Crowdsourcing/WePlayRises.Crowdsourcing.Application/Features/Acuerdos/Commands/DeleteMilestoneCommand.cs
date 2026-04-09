using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class DeleteMilestoneCommand : IRequest<ServiceResponse<bool>>
{
    public Guid MilestoneId { get; set; }
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class DeleteMilestoneCommandHandler : IRequestHandler<DeleteMilestoneCommand, ServiceResponse<bool>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IAcuerdoCrowdsourcingMilestoneService _milestoneService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<DeleteMilestoneCommand> _validator;
    private readonly ILogger<DeleteMilestoneCommandHandler> _logger;

    public DeleteMilestoneCommandHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IAcuerdoCrowdsourcingMilestoneService milestoneService,
        IArtistaService artistaService,
        IValidator<DeleteMilestoneCommand> validator,
        ILogger<DeleteMilestoneCommandHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _milestoneService = milestoneService ?? throw new ArgumentNullException(nameof(milestoneService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(DeleteMilestoneCommand request, CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<bool> { Messages = validationResult.GetServiceResponseMessages() };
            }

            var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>("Acuerdo no encontrado", ServiceResponseMessageType.NotFound_Acuerdo);
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null || acuerdo.ArtistaId != artista.Id)
            {
                return ValidateExtensions.ForbiddenServiceResponse<bool>("No tienes permisos sobre este acuerdo", ServiceResponseMessageType.Auth_Forbidden);
            }

            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<bool>("Solo se pueden eliminar milestones de acuerdos activos", ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            var milestone = await _milestoneService.GetByIdAsync(request.MilestoneId, ct);
            if (milestone == null || milestone.AcuerdoId != acuerdo.Id)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>("Milestone no encontrado", ServiceResponseMessageType.NotFound_Milestone);
            }

            if (milestone.FechaCompletado != null)
            {
                return ValidateExtensions.ConflictServiceResponse<bool>("No se puede eliminar un milestone completado", ServiceResponseMessageType.BusinessRule_MilestoneCompleted);
            }

            var tieneEntregables = await _milestoneService.TieneEntregablesAsync(request.MilestoneId, ct);
            if (tieneEntregables)
            {
                return ValidateExtensions.ConflictServiceResponse<bool>("No se puede eliminar un milestone que tiene entregables", ServiceResponseMessageType.BusinessRule_MilestoneHasEntregables);
            }

            await _milestoneService.DeleteAsync(request.MilestoneId, ct);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Milestone eliminado correctamente" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting milestone {MilestoneId}", request.MilestoneId);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>("Error inesperado al eliminar milestone", ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
