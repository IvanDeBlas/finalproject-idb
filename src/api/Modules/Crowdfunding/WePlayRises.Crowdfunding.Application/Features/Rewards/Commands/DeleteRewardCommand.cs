using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class DeleteRewardCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; }

    public DeleteRewardCommand(Guid id)
    {
        Id = id;
    }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class DeleteRewardCommandHandler : IRequestHandler<DeleteRewardCommand, ServiceResponse<bool>>
{
    private readonly IRewardService _service;
    private readonly ILogger<DeleteRewardCommandHandler> _logger;

    public DeleteRewardCommandHandler(
        IRewardService service,
        ILogger<DeleteRewardCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        DeleteRewardCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar existencia
            var rewardId = new CampaniaCrowdfundingRewardId(request.Id);
            var entity = await _service.GetByIdAsync(rewardId, cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Reward no encontrado",
                    ServiceResponseMessageType.NotFound_Reward);
            }

            // 2. Validar que reward NO tiene backings
            var hasBackings = await _service.HasBackingsAsync(rewardId, cancellationToken);
            if (hasBackings)
            {
                return ValidateExtensions.ConflictServiceResponse<bool>(
                    "No se puede eliminar una recompensa con aportes existentes",
                    ServiceResponseMessageType.BusinessRule_RewardHasBackings);
            }

            // 3. Soft delete (desactivar)
            entity.EsActivo = false;
            entity.FechaActualizacion = DateTime.UtcNow;
            await _service.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Reward soft-deleted with Id {Id}", request.Id);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Reward eliminado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Reward with Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al eliminar reward",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
