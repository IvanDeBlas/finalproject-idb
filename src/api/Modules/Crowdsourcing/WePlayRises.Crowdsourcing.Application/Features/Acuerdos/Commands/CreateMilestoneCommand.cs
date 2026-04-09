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
public class CreateMilestoneCommand : IRequest<ServiceResponse<MilestoneCreatedResultDto>>
{
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
public class CreateMilestoneCommandHandler : IRequestHandler<CreateMilestoneCommand, ServiceResponse<MilestoneCreatedResultDto>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IAcuerdoCrowdsourcingMilestoneService _milestoneService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<CreateMilestoneCommand> _validator;
    private readonly ILogger<CreateMilestoneCommandHandler> _logger;

    public CreateMilestoneCommandHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IAcuerdoCrowdsourcingMilestoneService milestoneService,
        IArtistaService artistaService,
        IValidator<CreateMilestoneCommand> validator,
        ILogger<CreateMilestoneCommandHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _milestoneService = milestoneService ?? throw new ArgumentNullException(nameof(milestoneService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<MilestoneCreatedResultDto>> Handle(
        CreateMilestoneCommand request,
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
                _logger.LogWarning("User {UserId} attempted to create milestone for acuerdo {AcuerdoId} without ownership",
                    request.UserId, request.AcuerdoId);
                return ValidateExtensions.ForbiddenServiceResponse<MilestoneCreatedResultDto>(
                    "No tienes permisos sobre este acuerdo",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<MilestoneCreatedResultDto>(
                    "Solo se pueden agregar milestones a acuerdos activos",
                    ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            var entity = new AcuerdoCrowdsourcingMilestone
            {
                Id = Guid.NewGuid(),
                AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId),
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                ImporteParcial = request.ImporteParcial,
                FechaLimite = request.FechaLimite,
                FechaCompletado = null
            };

            var id = await _milestoneService.CreateAsync(entity, ct);

            var importeAsignadoTotal = await _milestoneService.GetSumaImportesAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            var porcentajeParcial = acuerdo.ImporteTotalPactado > 0
                ? Math.Round((request.ImporteParcial / acuerdo.ImporteTotalPactado) * 100, 2)
                : 0;

            var resultDto = new MilestoneCreatedResultDto
            {
                Id = id,
                Titulo = request.Titulo,
                Orden = entity.Orden,
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
                        Message = "Milestone creado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating milestone for acuerdo {AcuerdoId}", request.AcuerdoId);
            return ValidateExtensions.InternalServerErrorServiceResponse<MilestoneCreatedResultDto>(
                "Error inesperado al crear milestone",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
