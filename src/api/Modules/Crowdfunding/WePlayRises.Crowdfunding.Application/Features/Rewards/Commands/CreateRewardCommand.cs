using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateRewardCommand : IRequest<ServiceResponse<RewardDto>>
{
    public Guid CampaniaId { get; set; }
    public int TipoRewardId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public int MonedaId { get; set; }
    public bool EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; }
    public int? CantidadPorBacker { get; set; }
    public bool IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateRewardCommandHandler : IRequestHandler<CreateRewardCommand, ServiceResponse<RewardDto>>
{
    private readonly IRewardService _rewardService;
    private readonly ICampaniaService _campaniaService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateRewardCommand> _validator;
    private readonly ILogger<CreateRewardCommandHandler> _logger;

    public CreateRewardCommandHandler(
        IRewardService rewardService,
        ICampaniaService campaniaService,
        IMapper mapper,
        IValidator<CreateRewardCommand> validator,
        ILogger<CreateRewardCommandHandler> logger)
    {
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RewardDto>> Handle(
        CreateRewardCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateReward: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<RewardDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Verificar que la campania existe
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campaniaExists = await _campaniaService.ExistsAsync(campaniaId, cancellationToken);
            if (!campaniaExists)
            {
                return ValidateExtensions.NotFoundServiceResponse<RewardDto>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 3. Mapear a entidad de dominio
            var entity = _mapper.Map<CampaniaCrowdfundingReward>(request);
            entity.Id = CampaniaCrowdfundingRewardId.CreateNew();
            entity.EsActivo = true;
            entity.FechaCreacion = DateTime.UtcNow;

            // 3.1. Auto-incrementar Orden si no se proporciono o es 0
            if (entity.Orden == 0)
            {
                var maxOrden = await _rewardService.GetMaxOrdenAsync(campaniaId, cancellationToken);
                entity.Orden = maxOrden + 1;
            }

            // 4. Crear via servicio
            var id = await _rewardService.CreateAsync(entity, cancellationToken);

            // 5. Obtener entidad creada y mapear a DTO
            var created = await _rewardService.GetByIdAsync(id, cancellationToken);
            var dto = _mapper.Map<RewardDto>(created);

            _logger.LogInformation("Reward created with Id {Id} for Campania {CampaniaId}",
                id.Value, request.CampaniaId);

            // 6. Retornar respuesta exitosa
            return new ServiceResponse<RewardDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Reward creado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Reward for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<RewardDto>(
                "Error inesperado al crear reward",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
