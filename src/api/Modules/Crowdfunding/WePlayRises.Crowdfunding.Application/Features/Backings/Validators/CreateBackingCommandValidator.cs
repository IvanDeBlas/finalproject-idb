using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Features.Backings.Commands;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Backings.Validators;

public class CreateBackingCommandValidator : AbstractValidator<CreateBackingCommand>
{
    private readonly ICampaniaService _campaniaService;
    private readonly IRewardService _rewardService;

    public CreateBackingCommandValidator(
        ICampaniaService campaniaService,
        IRewardService rewardService)
    {
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));

        // 1. Synchronous validations
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El ID de campania es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Monto)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El monto debe ser al menos 1 EUR")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.Mensaje)
            .MaximumLength(500)
            .WithMessage("El mensaje no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.Mensaje));

        // 2. Validate campania exists
        RuleFor(x => x.CampaniaId)
            .MustAsync(async (campaniaId, ct) =>
            {
                var id = new CampaniaCrowdfundingId(campaniaId);
                var campania = await _campaniaService.GetByIdAsync(id, ct);
                return campania != null;
            })
            .WithMessage("Campania no encontrada")
            .WithErrorCode(ServiceResponseMessageType.NotFound_Campania);

        // 3. Validate campania is published (estado = 2)
        RuleFor(x => x.CampaniaId)
            .MustAsync(async (campaniaId, ct) =>
            {
                var id = new CampaniaCrowdfundingId(campaniaId);
                var campania = await _campaniaService.GetByIdAsync(id, ct);
                return campania?.EstadoCampaniaId == 2;
            })
            .WithMessage("Esta campania no esta activa")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_CampaniaNotActive);

        // 4. Validate campania hasn't ended
        RuleFor(x => x.CampaniaId)
            .MustAsync(async (campaniaId, ct) =>
            {
                var id = new CampaniaCrowdfundingId(campaniaId);
                var campania = await _campaniaService.GetByIdAsync(id, ct);
                if (campania == null) return true;
                return !campania.FechaFin.HasValue || DateTime.UtcNow <= campania.FechaFin.Value;
            })
            .WithMessage("Esta campania ya finalizo")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_CampaniaEnded);

        // 5. Validate anonymous auth
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                if (!string.IsNullOrEmpty(command.UserId)) return true;

                var id = new CampaniaCrowdfundingId(command.CampaniaId);
                var campania = await _campaniaService.GetByIdAsync(id, ct);
                return campania?.PermiteAportacionesAnonimas == true;
            })
            .WithMessage("Debes iniciar sesion para hacer un aporte")
            .WithErrorCode(ServiceResponseMessageType.Auth_UserNotAuthenticated);

        // 6. Validate reward (only if RewardId provided)
        When(x => x.RewardId.HasValue, () =>
        {
            RuleFor(x => x.RewardId!.Value)
                .MustAsync(async (rewardId, ct) =>
                {
                    var id = new CampaniaCrowdfundingRewardId(rewardId);
                    var reward = await _rewardService.GetByIdAsync(id, ct);
                    return reward != null && reward.EsActivo;
                })
                .WithMessage("Recompensa no encontrada")
                .WithErrorCode(ServiceResponseMessageType.NotFound_Reward);

            RuleFor(x => x.RewardId!.Value)
                .MustAsync(async (rewardId, ct) =>
                {
                    var id = new CampaniaCrowdfundingRewardId(rewardId);
                    return await _rewardService.HasStockAvailableAsync(id, ct);
                })
                .WithMessage("Esta recompensa esta agotada")
                .WithErrorCode(ServiceResponseMessageType.BusinessRule_RewardOutOfStock);

            RuleFor(x => x)
                .MustAsync(async (command, ct) =>
                {
                    var id = new CampaniaCrowdfundingRewardId(command.RewardId!.Value);
                    var reward = await _rewardService.GetByIdAsync(id, ct);
                    return reward == null || command.Monto >= reward.ImporteMinimo;
                })
                .WithMessage("El monto es menor al minimo requerido para esta recompensa")
                .WithErrorCode(ServiceResponseMessageType.BusinessRule_AmountBelowMinimum);
        });
    }
}
