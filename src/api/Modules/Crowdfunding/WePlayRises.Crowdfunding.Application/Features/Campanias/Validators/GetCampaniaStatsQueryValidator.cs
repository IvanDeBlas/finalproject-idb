using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;

public class GetCampaniaStatsQueryValidator : AbstractValidator<GetCampaniaStatsQuery>
{
    public GetCampaniaStatsQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El ID de campania es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
