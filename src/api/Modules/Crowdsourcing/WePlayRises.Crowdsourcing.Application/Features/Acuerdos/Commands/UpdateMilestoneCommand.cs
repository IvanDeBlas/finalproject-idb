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
public class UpdateMilestoneCommand : IRequest<ServiceResponse<MilestoneCreatedResultDto>>
{
    public Guid MilestoneId { get; set; }
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteParcial { get; set; }
    public DateTime? FechaLimite { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class UpdateMilestoneCommandHandler : IRequestHandler<UpdateMilestoneCommand, ServiceResponse<MilestoneCreatedResultDto>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IAcuerdoCrowdsourcingMilestoneService _milestoneService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<UpdateMilestoneCommand> _validator;
    private readonly ILogger<UpdateMilestoneCommandHandler> _logger;

    public UpdateMilestoneCommandHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IAcuerdoCrowdsourcingMilestoneService milestoneService,
        IArtistaService artistaService,
        IValidator<UpdateMilestoneCommand> validator,
        ILogger<UpdateMilestoneCommandHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _milestoneService = milestoneService ?? throw new ArgumentNullException(nameof(milestoneService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<MilestoneCreatedResultDto>> Handle(
        UpdateMilestoneCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<MilestoneCreatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<MilestoneCreatedResultDto>(
                    "Acuerdo no encontrado",
                    ServiceResponseMessageType.NotFound_Acuerdo);
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null || acuerdo.ArtistaId != artista.Id)
            {
                return ValidateExtensions.ForbiddenServiceResponse<MilestoneCreatedResultDto>(
                    "No tienes permisos sobre este acuerdo",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<MilestoneCreatedResultDto>(
                    "Solo se pueden editar milestones de acuerdos activos",
                    ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            var milestone = await _milestoneService.GetByIdAsync(request.MilestoneId, ct);
            if (milestone == null || milestone.AcuerdoId != acuerdo.Id)
            {
                return ValidateExtensions.NotFoundServiceResponse<MilestoneCreatedResultDto>(
                    "Milestone no encontrado",
                    ServiceResponseMessageType.NotFound_Milestone);
            }

            if (milestone.FechaCompletado != null)
            {
                return ValidateExtensions.ConflictServiceResponse<MilestoneCreatedResultDto>(
                    "No se puede editar un milestone completado",
                    ServiceResponseMessageType.BusinessRule_MilestoneCompleted);
            }

            milestone.Titulo = request.Titulo;
            milestone.Descripcion = request.Descripcion;
            milestone.ImporteParcial = request.ImporteParcial;
            milestone.FechaLimite = request.FechaLimite;

            await _milestoneService.UpdateAsync(milestone, ct);

            var importeAsignadoTotal = await _milestoneService.GetSumaImportesAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            var porcentajeParcial = acuerdo.ImporteTotalPactado > 0
                ? Math.Round((request.ImporteParcial / acuerdo.ImporteTotalPactado) * 100, 2)
                : 0;

            var resultDto = new MilestoneCreatedResultDto
            {
                Id = request.MilestoneId,
                Titulo = request.Titulo,
                Orden = milestone.Orden,
                ImporteParcial = request.ImporteParcial,
                PorcentajeParcial = porcentajeParcial,
                ImporteAsignadoTotal = importeAsignadoTotal
            };

            return new ServiceResponse<MilestoneCreatedResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Milestone actualizado correctamente"
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating milestone {MilestoneId}", request.MilestoneId);
            return ValidateExtensions.InternalServerErrorServiceResponse<MilestoneCreatedResultDto>(
                "Error inesperado al actualizar milestone",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
