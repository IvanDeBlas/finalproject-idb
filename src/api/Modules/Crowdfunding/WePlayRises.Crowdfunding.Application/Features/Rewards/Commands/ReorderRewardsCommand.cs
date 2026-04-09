using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;

// -----------------------------------------------------------------------------
// DTO
// -----------------------------------------------------------------------------
public class RewardOrderDto
{
    public Guid RewardId { get; set; }
    public int Orden { get; set; }
}

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class ReorderRewardsCommand : IRequest<ServiceResponse<bool>>
{
    public Guid CampaniaId { get; set; }
    public List<RewardOrderDto> RewardOrders { get; set; } = new();
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class ReorderRewardsCommandHandler : IRequestHandler<ReorderRewardsCommand, ServiceResponse<bool>>
{
    private readonly IRewardService _rewardService;
    private readonly ICampaniaService _campaniaService;
    private readonly IValidator<ReorderRewardsCommand> _validator;
    private readonly ILogger<ReorderRewardsCommandHandler> _logger;

    public ReorderRewardsCommandHandler(
        IRewardService rewardService,
        ICampaniaService campaniaService,
        IValidator<ReorderRewardsCommand> validator,
        ILogger<ReorderRewardsCommandHandler> logger)
    {
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        ReorderRewardsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for ReorderRewards: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<bool>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Verificar que la campania existe
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campaniaExists = await _campaniaService.ExistsAsync(campaniaId, cancellationToken);
            if (!campaniaExists)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 3. Validar que todos los rewards existen y pertenecen a la campania
            foreach (var order in request.RewardOrders)
            {
                var rewardId = new CampaniaCrowdfundingRewardId(order.RewardId);
                var reward = await _rewardService.GetByIdAsync(rewardId, cancellationToken);

                if (reward == null)
                {
                    return ValidateExtensions.NotFoundServiceResponse<bool>(
                        $"Reward {order.RewardId} no encontrado",
                        ServiceResponseMessageType.NotFound_Reward);
                }

                if (reward.CampaniaId.Value != request.CampaniaId)
                {
                    return ValidateExtensions.BadRequestServiceResponse<bool>(
                        "Uno o mas rewards no pertenecen a la campania especificada",
                        ServiceResponseMessageType.BusinessRule_OperationNotAllowed);
                }
            }

            // 4. Convertir a tuplas y reordenar via servicio
            var updates = request.RewardOrders
                .Select(o => (new CampaniaCrowdfundingRewardId(o.RewardId), o.Orden))
                .ToList();

            await _rewardService.ReorderAsync(campaniaId, updates, cancellationToken);

            _logger.LogInformation("Rewards reordenados exitosamente para Campania {CampaniaId}", request.CampaniaId);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Recompensas reordenadas correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering rewards for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al reordenar rewards",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
