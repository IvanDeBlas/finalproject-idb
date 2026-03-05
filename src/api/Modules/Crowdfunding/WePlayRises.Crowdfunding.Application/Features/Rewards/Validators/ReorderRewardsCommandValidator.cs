using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Validators;

public class ReorderRewardsCommandValidator : AbstractValidator<ReorderRewardsCommand>
{
    public ReorderRewardsCommandValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El CampaniaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.RewardOrders)
            .NotNull()
            .WithMessage("La lista de rewards es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .NotEmpty()
            .WithMessage("La lista de rewards es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleForEach(x => x.RewardOrders)
            .ChildRules(order =>
            {
                order.RuleFor(x => x.RewardId)
                    .NotEmpty()
                    .WithMessage("El RewardId es obligatorio")
                    .WithErrorCode(ServiceResponseMessageType.Validation_Required);

                order.RuleFor(x => x.Orden)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("El orden debe ser mayor o igual a 0")
                    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
            });
    }
}
