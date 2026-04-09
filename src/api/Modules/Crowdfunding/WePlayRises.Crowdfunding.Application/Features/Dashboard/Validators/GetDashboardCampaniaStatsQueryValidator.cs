using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Dashboard.Validators;

public class GetDashboardCampaniaStatsQueryValidator : AbstractValidator<GetDashboardCampaniaStatsQuery>
{
    public GetDashboardCampaniaStatsQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("CampaniaId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
