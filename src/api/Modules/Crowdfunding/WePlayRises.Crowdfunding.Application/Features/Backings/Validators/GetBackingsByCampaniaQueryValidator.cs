using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Backings.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Backings.Validators;

public class GetBackingsByCampaniaQueryValidator : AbstractValidator<GetBackingsByCampaniaQuery>
{
    public GetBackingsByCampaniaQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El ID de campania es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100)
            .WithMessage("El limite debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
