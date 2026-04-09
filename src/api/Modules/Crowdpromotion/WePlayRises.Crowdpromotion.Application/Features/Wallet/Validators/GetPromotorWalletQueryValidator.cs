using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;

public class GetPromotorWalletQueryValidator : AbstractValidator<GetPromotorWalletQuery>
{
    public GetPromotorWalletQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
